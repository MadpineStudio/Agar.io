using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static QTreeEntryPoint;
using static PlayerManager;
public class PlayerBehaviour : AbsorbableObject
{
    private PlayerActions playerActions;
    public bool colliding;
    public GameObject playerRef;
    public float speed;
    public float radius = 1;
    public Rigidbody2D playerRb;
    private List<Point> nearbyPoints;
    // private Queue<IEnumerator> _coroutineQueue = new Queue<IEnumerator>();
    [SerializeField] private PlayerSettings playerSettings;
    [SerializeField] private GameObject playerSecondaryBody;
    public float currentSpeed;
    public float acceleration;
    public float maxSpeed;
    public Point oldPoint;
    void Awake()
    {
        playerActions = new();
        playerActions.Enable();
        if (playerRef == null) playerRef = gameObject;
        oldPoint = new Point(transform.position.x, transform.position.y,gameObject);
    }

    void OnEnable()
    {
        playerActions.PlayerMap.Enable();
        playerActions.PlayerMap.Move.ReadValue<Vector2>();
        playerActions.PlayerMap.Atacar.performed += Atack;
    }
    void OnDisable()
    {
        playerActions.PlayerMap.Atacar.performed -= Atack;
        playerActions.PlayerMap.Disable();
    }

    new void Start()
    {
        base.Start();
    }

    public void FixedUpdate()
    {
        HandleMovement();
    }
    public void HandleMovement()
    {
        Vector3 oldpos = transform.position;
        Vector2 direction = playerActions.PlayerMap.Move.ReadValue<Vector2>();
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButton(0))
        {
            direction = (mousePosition - (Vector2)transform.position).normalized;
        }
        if (direction != Vector2.zero)
        {
            currentSpeed += acceleration * Time.deltaTime;
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
        }
        else
        {
            currentSpeed = 0f;
        }

        transform.Translate(direction.normalized * (currentSpeed * Time.deltaTime));
        if (oldpos != transform.position)
        {
            Point newPoint = new Point(transform.position.x, transform.position.y, gameObject);
            PlayerManager.instance.quadtree.UpdatePosition(oldPoint, newPoint);
            oldPoint = newPoint;
        }

    }

    private void Update()
    {
        DetectCollisions();
        QueryNearbyPoints();
        QueryNearbyCircle();
    }
    public void QueryNearbyPoints()
    {
        if (QTreeEntryPoint.instance?.QuadTree == null) return;
        Vector2 pos = transform.position;
        Rectangle queryRange = new Rectangle(pos.x, pos.y, playerSettings.searchAreaWidth, playerSettings.searchAreaHeight);
        nearbyPoints = QTreeEntryPoint.instance.QuadTree.Query(queryRange);

    }
    // ReSharper disable once Unity.NoNullPropagation
    public void QueryNearbyCircle()
    {
        if (QTreeEntryPoint.instance?.QuadTree == null)
            return;
        Vector2 pos = transform.position;
        float radius = this.radius * transform.localScale.x;
        var nearbyPoints = QTreeEntryPoint.instance.QuadTree.QueryCircle(pos, radius);
        var nearbyPlayers = new List<Point>();
        if (PlayerManager.instance?.quadtree != null) nearbyPlayers = PlayerManager.instance.quadtree.QueryCircle(pos, radius);
        if (nearbyPoints.Count > 0)
        {
            // Debug.Log("Pontos encontrados na query circular: " + nearbyPoints.Count);
            foreach (Point point in nearbyPoints)
            {
                GameObject obj = point.data as GameObject;
                mass += obj.transform.GetComponent<AbsorbableObject>().mass;
                UpdateDiameter();
                Destroy(point.data as GameObject);
                QTreeEntryPoint.instance.QuadTree.Remove(point);
            }
        }
        if (nearbyPlayers.Count > 1)
        {
            Debug.Log("Pontos encontrados na query circular dos players: " + nearbyPlayers.Count);
            foreach (Point point in nearbyPlayers)
            {

                GameObject obj = point.data as GameObject;
                GameObject playerReference = obj.GetComponent<PlayerBehaviour>().playerRef;
                if (playerReference == playerRef)
                {
                    Debug.Log("COlidiu com o player");
                    colliding = true;
                }
                if (gameObject != playerRef && point.data as GameObject == gameObject)
                {

                    if (gameObject.GetComponent<PlayerSecondaryBody>().canMove)
                    {
                        PlayerBehaviour player = playerRef.GetComponent<PlayerBehaviour>();
                        player.mass += mass;
                        player.UpdateDiameter();
                        UpdateDiameter();
                        PlayerManager.instance.quadtree.Remove(point);
                        Destroy(gameObject);
                    }
                }
            }
        }
        if (nearbyPlayers.Count == 1)
        {
            colliding = false;
        }
    }

    private void Atack(InputAction.CallbackContext context)
    {
        if (mass > 500) Separate();
    }


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

    public void Separate()
    {
        mass /= 2;
        PlayerSecondaryBody playerSecondaryBodyLocal = playerSecondaryBody.GetComponent<PlayerSecondaryBody>();
        playerSecondaryBodyLocal.mass = mass;
        playerSecondaryBodyLocal.playerRef = playerRef;

        GameObject newPart = Instantiate(playerSecondaryBody, transform.position, transform.rotation);
        PlayerManager.instance.InsertPlayer(newPart.transform.position.x, newPart.transform.position.y, newPart);
        UpdateDiameter();
    }

    // Insira esta função em um MonoBehaviour que gerencia a simulação
    // ReSharper disable Unity.PerformanceAnalysis
    void DetectCollisions() {
        List<Point> pts = PlayerManager.instance.quadtree.GetPoints();
        for (int i = 0; i < pts.Count; i++) {
            pts[i].colliding = false;
        }
        for (int i = 0; i < pts.Count; i++) {
            Point a = pts[i];
            for (int j = i + 1; j < pts.Count; j++) {
                Point b = pts[j];
                float dx = b.X - a.X;
                float dy = b.Y - a.Y;
                float distance = Mathf.Sqrt(dx * dx + dy * dy);
                if (distance <= a.Radius() + b.Radius() && distance != 0) {
                    a.colliding = true;
                    b.colliding = true;
                    Debug.Log("Colisão detectada entre os pontos " + i + " e " + j);
                }
            }
        }
    }
    
    // public IEnumerator SizeUpdater(float newSize)
    // {
    //     float localSize = transform.localScale.x;
    //     while (localSize < newSize)
    //     {
    //         UpdateDiameter();
    //         yield return new WaitForEndOfFrame();
    //     }

    // }
    // public IEnumerator SizeQueueConsumer()
    // {

    //     while (_gameRunning)
    //     {
    //         if (_coroutineQueue.Count > 0)
    //             yield return StartCoroutine(_coroutineQueue.Dequeue());

    //     }
    // }


}
