using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.Cinemachine;
using Unity.Netcode;
using TMPro;
public class PlayerBehaviour : AbsorbableObject
{

    private List<Point> points = new();
    private List<Point> nearbyPoints;
    private List<GameObject> newParts = new();
    private PlayerActions playerActions;
    private GameObject _newPart;
    private float _initialSpeed;
    [SerializeField] private PlayerSettings playerSettings;
    [SerializeField] private GameObject playerSecondaryBody;
    [SerializeField] private GameObject cameraPrefab;
    [SerializeField] private PlayerDataScriptable playerData;
    public float radius = 1;
    public float currentSpeed;
    public float acceleration;
    public float maxSpeed;
    public float offset;

    #region UnityFunctions
    void OnEnable()
    {
        playerActions.PlayerMap.Enable();
        playerActions.PlayerMap.Move.ReadValue<Vector2>();
        playerActions.PlayerMap.Atacar.performed += Attack;
    }
    void OnDisable()
    {
        playerActions.PlayerMap.Atacar.performed -= Attack;
        playerActions.PlayerMap.Disable();
    }
    void Awake()
    {
        playerActions = new();
        playerActions.Enable();


    }
    new void Start()
    {
        base.Start();
        Texture playerTex = transform.GetComponent<SpriteRenderer>().sharedMaterial.GetTexture("_MainTex0");
        playerSecondaryBody.GetComponent<SpriteRenderer>().sharedMaterial.SetTexture("_MainTex0", playerTex);
        PlayerManager.instance.InsertPlayer(transform.position.x, transform.position.y, gameObject);

    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Initialize();
    }

    private void Update()
    {
        QueryNearbyPoints();
        QueryNearbyCircle();
    }
    public void FixedUpdate()
    {
        // if (!IsOwner || !Application.isFocused) return;
        if (!Application.isFocused) return;
        if (points.Count > 0) HandleMovement();
    }
#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        if (!playerSettings.debug) return;

        if (nearbyPoints == null) return;
        foreach (var p in nearbyPoints)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(new Vector3(p.X, p.Y, 0), playerSettings.foundNearGizmoSize);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(playerSettings.searchAreaWidth * 2f, playerSettings.searchAreaHeight * 2f));
        Gizmos.DrawWireSphere(transform.position, radius * transform.localScale.x);
    }
#endif
    #endregion
    #region Network
    public void Initialize()
    {
        gameObject.GetComponent<SpriteRenderer>().material.SetTexture("_MainTex0", playerData.playerImage.texture);
        TMP_Text nickArea = transform.GetChild(1).GetComponent<TMP_Text>();
        nickArea.text = playerData.playerName;
        if (IsOwner)
        {
            CinemachineTargetGroup targetGroup = transform.GetChild(0).transform.GetComponent<CinemachineTargetGroup>();

            CinemachineVirtualCameraBase camera = Instantiate(cameraPrefab, Vector3.zero, Quaternion.identity).GetComponent<CinemachineCamera>();
            camera.Follow = targetGroup.transform;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ExplodeServerRpc(Vector3 position, Quaternion rotation, ulong clientId, ServerRpcParams rpcParams = default)
    {
        // O servidor instancia e sincroniza a nova parte
        GameObject newPart = Instantiate(playerSecondaryBody, position, rotation);
        NetworkObject networkObject = newPart.GetComponent<NetworkObject>();

        if (networkObject != null)
        {
            networkObject.SpawnWithOwnership(clientId, true); // Sincroniza com todos os clientes
            SyncNewPartClientRpc(networkObject.NetworkObjectId); // Sincroniza o ID com os clientes
        }
        else
        {
            Debug.LogError("NetworkObject não encontrado no prefab!");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestExplodeServerRpc(int pointIndex)
    {
        ExplodeClientRpc(pointIndex);
    }
    [ServerRpc(RequireOwnership = false)]
    private void RequestDespawnObjectServerRpc(ulong objectId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out NetworkObject networkObject))
        {
            networkObject.Despawn(true);
        }

    }
    [ClientRpc]
    private void SyncNewPartClientRpc(ulong objectId)
    {
        // Garante que o cliente adiciona a nova parte à lista correta
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out NetworkObject networkObject))
        {
            newParts.Add(networkObject.gameObject);
        }
    }

    [ClientRpc]
    private void ExplodeClientRpc(int pointIndex)
    {
        if (!IsOwner) return;

        List<Point> pointsToAdd = new();
        Point point = points[pointIndex];

        float mass = Mathf.PI * superficialDensity * Mathf.Pow(point.data.transform.localScale.x / 2, 2);
        UpdateDiameter(point.data.transform, mass / 5);
        if (point.data == gameObject) this.mass = mass / 5;

        for (int i = 0; i < 4; i++)
        {
            // Solicita ao servidor que crie e sincronize o objeto
            ExplodeServerRpc(points[pointIndex].data.transform.position, points[pointIndex].data.transform.rotation, NetworkManager.Singleton.LocalClientId);
        }

        // Esperar o servidor criar e sincronizar as novas partes antes de manipulá-las
        StartCoroutine(WaitForNewParts(pointsToAdd, mass / 5));
    }

    private IEnumerator WaitForNewParts(List<Point> pointsToAdd, float newMass)
    {
        // Aguarda um pequeno tempo para garantir que os objetos foram sincronizados
        yield return new WaitUntil(() => newParts.Count > 0);

        foreach (var newPart in newParts)
        {
            UpdateDiameter(newPart.transform, newMass);
            Point newPoint = PlayerManager.instance.InsertPlayer(newPart.transform.position.x, newPart.transform.position.y, newPart);
            pointsToAdd.Add(newPoint);
        }

        if (pointsToAdd.Count > 0)
        {
            StartCoroutine(EnableAbsorption(pointsToAdd, 20.0f));
            StartCoroutine(InitialMovement(pointsToAdd, true));
        }

        points.AddRange(pointsToAdd);
        newParts.Clear(); // Limpa a lista para a próxima explosão
    }
    #endregion
    #region MovementHandlers
    public void HandleMovement()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var direction = mousePosition - (Vector2)transform.position;
        if (direction != Vector2.zero)
        {
            currentSpeed += acceleration * Time.deltaTime;
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
        }
        else currentSpeed = 0f;
        for (int i = points.Count - 1; i >= 0; i--)
        {
            direction = mousePosition - (Vector2)points[i].data.transform.position;
            Vector2 changingDirection = Vector2.zero;
            if (points[i].hasFullySpawned)
            {
                float innertSpeed = 0f;
                // changingDirection = DetectCollisions(points[i]);
                DetectCollisions(points[i]);
                if (i >= points.Count) i = points.Count - 1;
                Point oldPoint = points[i];
                points[i].data.transform.Translate((direction + changingDirection).normalized * ((currentSpeed > 0 ? currentSpeed : innertSpeed) * Time.fixedDeltaTime));
                Vector2 pointPos = points[i].data.transform.position;
                points[i].X = pointPos.x;
                points[i].Y = pointPos.y;
                PlayerManager.instance.quadtree.UpdatePosition(oldPoint, points[i]);
            }
        }

    }
    private float HandleInitialMovement(Point point, Vector2 direction, float speed)
    {
        Point oldPoint = point;
        speed -= acceleration * (0.3f / points.Count * Time.deltaTime);
        speed = Mathf.Max(speed, 0);
        point.data.transform.Translate(direction * (speed * (10 * Time.deltaTime)));
        Vector2 pointPos = point.data.transform.position;
        point.X = pointPos.x;
        point.Y = pointPos.y;
        PlayerManager.instance.quadtree.UpdatePosition(oldPoint, point);
        return speed;
    }
    #endregion
    #region PlayerActions
    private void Explode(Point point)
    {
        // Debug.Log(IsOwner);
        // if (IsOwner) RequestExplodeServerRpc(points.IndexOf(point));
        List<Point> pointsToAdd = new();
        List<GameObject> newParts = new();
        Vector2[] directions = { Vector2.up * .2f, Vector2.down * .2f, Vector2.right * .2f, Vector2.left * .2f };
        float mass = Mathf.PI * superficialDensity * Mathf.Pow(point.data.transform.localScale.x / 2, 2);
        UpdateDiameter(point.data.transform, mass / 5);
        if (point.data == gameObject) this.mass = mass / 5;
        for (int i = 0; i < 4; i++)
        {
            GameObject newPart = Instantiate(playerSecondaryBody, point.data.transform.position, point.data.transform.rotation);
            newPart.name = "jorge" + points.Count + i * Time.time;
            newParts.Add(newPart);
        }
        foreach (var newPart in newParts)
        {

            UpdateDiameter(newPart.transform, mass / 5);
            Point newPoint = PlayerManager.instance.InsertPlayer(newPart.transform.position.x + directions[newParts.IndexOf(newPart)].x, newPart.transform.position.y + directions[newParts.IndexOf(newPart)].y, newPart);
            pointsToAdd.Add(newPoint);
        }
        if (pointsToAdd != null)
        {
            StartCoroutine(EnableAbsorption(pointsToAdd, 20.0f));
            StartCoroutine(InitialMovement(pointsToAdd, true));
        }
        points.AddRange(pointsToAdd);
    }
    private void Attack(InputAction.CallbackContext context)
    {
        List<Point> pointsToAdd = new();
        foreach (var point in points)
        {
            GameObject pointGo = point.data;
            float mass = Mathf.PI * superficialDensity * Mathf.Pow(pointGo.transform.localScale.x / 2, 2);
            if (mass > 40)
            {
                pointsToAdd.Add(Separate(pointGo, mass));
            }
        }
        if (pointsToAdd != null)
        {
            StartCoroutine(EnableAbsorption(pointsToAdd, 20.0f));
            StartCoroutine(InitialMovement(pointsToAdd, false));
        }
        points.AddRange(pointsToAdd);
    }
    public Point Separate(GameObject mainObject, float mass)
    {
        mass /= 2;
        GameObject newPart = Instantiate(playerSecondaryBody, mainObject.transform.position, mainObject.transform.rotation);
        newPart.name = "jorge" + points.Count + Random.Range(0, 5) * Time.time;
        Point point = PlayerManager.instance.InsertPlayer(newPart.transform.position.x, newPart.transform.position.y, newPart);
        UpdateDiameter(mainObject.transform, mass);
        UpdateDiameter(newPart.transform, mass);

        if (mainObject == gameObject) this.mass = mass;

        return point;
    }
    #endregion
    #region QuadTree Utils
    public void AddInitialPoint(Point point)
    {
        point.hasFullySpawned = true;
        point.canBeAbsorbed = true;
        points.Add(point);
    }
    public void QueryNearbyPoints()
    {
        if (QTreeEntryPoint.instance?.QuadTree == null) return;
        Vector2 pos = transform.position;
        Rectangle queryRange = new Rectangle(pos.x, pos.y, playerSettings.searchAreaWidth, playerSettings.searchAreaHeight);
        nearbyPoints = QTreeEntryPoint.instance.QuadTree.Query(queryRange);

    }
    public void QueryNearbyCircle()
    {
        if (QTreeEntryPoint.instance?.QuadTree == null) return;
        List<Point> clonedPoints = new(points);
        foreach (var playerPoint in clonedPoints)
        {
            Vector2 pos = playerPoint.data.transform.position;
            float radius = this.radius * playerPoint.data.transform.localScale.x;
            var nearbyPoints = QTreeEntryPoint.instance.QuadTree.QueryCircle(pos, radius);
            if (nearbyPoints.Count > 0)
            {
                foreach (Point point in nearbyPoints)
                {
                    float mass = Mathf.PI * superficialDensity * Mathf.Pow(playerPoint.data.transform.localScale.x / 2, 2);
                    float objMass = point.data.transform.GetComponent<AbsorbableObject>().mass;
                    if (mass >= objMass * 1.25f)
                    {
                        if (point.data.CompareTag("Bacteria"))
                        {
                            objMass *= 0.5f;
                            mass += objMass;
                            this.mass = mass;
                            UpdateDiameter(playerPoint.data.transform, mass);
                            Explode(playerPoint);
                        }
                        else
                        {
                            mass += objMass;
                            this.mass = mass;
                            UpdateDiameter(playerPoint.data.transform, mass);
                        }
                        Destroy(point.data);
                        QTreeEntryPoint.instance.QuadTree.Remove(point);
                    }
                }
            }
        }

    }
    public Vector2 DetectCollisions(Point a)
    {
        List<Point> collided = new();
        List<Point> pts = points;
        Vector2 avoidanceVector = Vector2.zero;
        if (a != null)
        {
            a.colliding = false;
            for (int j = pts.Count - 1; j >= 0; j--)
            {
                Point b = pts[j];
                if (a != b)
                {
                    b.colliding = false;
                    Vector2 dis = new Vector2(b.X - a.X, b.Y - a.Y);
                    float distance = Mathf.Sqrt(dis.x * dis.x + dis.y * dis.y);
                    if (a.hasFullySpawned && b.hasFullySpawned && distance <= a.Radius() + b.Radius() && distance != 0)
                    {
                        a.colliding = true;
                        Point oldPoint = b;
                        b.colliding = true;
                        Vector2 toObject = (Vector2)b.data.transform.position - (Vector2)a.data.transform.position;
                        float distanceBt = toObject.magnitude;
                        if (distanceBt > 0) avoidanceVector += toObject * distance;
                        b.data.transform.Translate(toObject.normalized * offset * -(distance - (a.Radius() + b.Radius())) * (maxSpeed * Time.fixedDeltaTime));
                        Vector2 pointPos = b.data.transform.position;
                        b.X = pointPos.x;
                        b.Y = pointPos.y;
                        PlayerManager.instance.quadtree.UpdatePosition(oldPoint, b); if (b.data == gameObject)
                        {
                            Point changer = a;
                            a = b;
                            b = changer;
                        }
                        if (b.canBeAbsorbed)
                        {
                            float mass = Mathf.PI * superficialDensity * Mathf.Pow(a.data.transform.localScale.x / 2, 2);
                            float bMass = Mathf.PI * superficialDensity * Mathf.Pow(b.data.transform.localScale.x / 2, 2);

                            UpdateDiameter(a.data.transform, mass + bMass);
                            PlayerManager.instance.quadtree.Remove(b);
                            pts.Remove(b);
                            points.Remove(b);

                            // Obtém o NetworkObject e despawna na rede
                            NetworkObject netObj = b.data.GetComponent<NetworkObject>();
                            if (netObj != null)
                            {
                                if (netObj.IsSpawned)
                                    RequestDespawnObjectServerRpc(netObj.NetworkObjectId);
                                else Destroy(b.data);
                            }
                            else
                            {
                                Debug.LogError("NetworkObject não encontrado ao tentar despawnar!");
                            }
                        }
                    }
                }
            }
        }
        return avoidanceVector;
    }
    #endregion
    #region Corroutines
    private IEnumerator InitialMovement(List<Point> newPoints, bool exploding)
    {
        Debug.Log("Ta rolando");
        Debug.Log(newPoints.Count);
        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.right, Vector2.left };
        _initialSpeed = maxSpeed;
        float speed = _initialSpeed;
        Vector2 localMousePos = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.transform.position).normalized;
        while (speed > 0)
        {
            foreach (var point in newPoints)
            {
                if (!exploding) speed = HandleInitialMovement(point, localMousePos, speed);
                else speed = HandleInitialMovement(point, directions[newPoints.IndexOf(point)], speed);
            }
            yield return new WaitForEndOfFrame();
        }

        _initialSpeed = maxSpeed;
        foreach (Point point in newPoints)
        {
            Point old = point;
            point.hasFullySpawned = true;
            PlayerManager.instance.quadtree.UpdatePosition(old, point);
        }
    }
    private IEnumerator EnableAbsorption(List<Point> points, float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach (var point in points)
        {
            // Point old = point;
            point.canBeAbsorbed = true;
            // PlayerManager.instance.quadtree.UpdatePosition(old, point);
        }
    }
    #endregion
}