using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static QTreeEntryPoint;
using static PlayerManager;
using System.Collections;
using Unity.VisualScripting;
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
    private float _initialSpeed;
    public Point oldPoint;


    private List<Point> points = new();
    private Vector2 _mousePosition = new();


    void Awake()
    {
        playerActions = new();
        playerActions.Enable();
        if (playerRef == null) playerRef = gameObject;
        // points.Add(new Point(transform.position.x, transform.position.y, gameObject));
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
        if (points.Count > 0) HandleMovement();
    }
    public void AddInitialPoint(Point point)
    {
        points.Add(point);
    }
    public void HandleMovement()
    {
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
        foreach (var point in points)
        {
            float innertSpeed = 0f;
            if (point.data != gameObject)
            {
                DetectCollisions(point);
                if (!point.colliding)
                {
                    direction = (Vector2)transform.position - (Vector2)point.data.transform.position + direction;
                    innertSpeed = 2f;
                }
            }
            oldPoint = point;
            GameObject pointGO = point.data;
            pointGO.transform.Translate(direction.normalized * ((currentSpeed > 0 ? currentSpeed : innertSpeed) * Time.fixedDeltaTime));
            // if()
            Vector2 pointPos = pointGO.transform.position;
            point.data = pointGO;
            point.X = pointPos.x;
            point.Y = pointPos.y;
            PlayerManager.instance.quadtree.UpdatePosition(oldPoint, point);
        }
        // if (oldpos != transform.position)
        // {
        //     Point newPoint = new Point(transform.position.x, transform.position.y, gameObject);
        //     oldPoint = newPoint;
        // }

    }

    private void Update()
    {
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
        foreach (var playerPoint in points)
        {
            Vector2 pos = playerPoint.data.transform.position;
            float radius = this.radius * playerPoint.data.transform.localScale.x;
            var nearbyPoints = QTreeEntryPoint.instance.QuadTree.QueryCircle(pos, radius);
            if (nearbyPoints.Count > 0)
            {
                // Debug.Log("Pontos encontrados na query circular: " + nearbyPoints.Count);
                foreach (Point point in nearbyPoints)
                {
                    GameObject obj = point.data;
                    float mass = Mathf.PI * superficialDensity * Mathf.Pow(playerPoint.data.transform.localScale.x / 2, 2);
                    mass += obj.transform.GetComponent<AbsorbableObject>().mass;
                    UpdateDiameter(playerPoint.data.transform, mass);
                    // UpdateDiameter();
                    Destroy(point.data);
                    QTreeEntryPoint.instance.QuadTree.Remove(point);
                }
            }
        }

    }

    private void Atack(InputAction.CallbackContext context)
    {
        Point newPoint = null;
        List<Point> pointsToAdd = new();
        foreach (var point in points)
        {
            GameObject pointGo = point.data;
            float mass = Mathf.PI * superficialDensity * Mathf.Pow(pointGo.transform.localScale.x / 2, 2);
            if (mass > 500)
            {
                newPoint = Separate(pointGo, mass);
                pointsToAdd.Add(newPoint);
            }
        }
        if (pointsToAdd != null) StartCoroutine(InitialMovement(pointsToAdd));
    }
    private void HandleInitialMovement(Point point)
    {
        oldPoint = point;
        Vector2 initialDirection = (_mousePosition - (Vector2)Camera.main.transform.position).normalized;
        _initialSpeed -= acceleration * (0.2f / points.Count * Time.deltaTime);
        _initialSpeed = Mathf.Max(_initialSpeed, 0);
        point.data.transform.Translate(initialDirection * (_initialSpeed * (10 * Time.deltaTime)));
        Vector2 pointPos = point.data.transform.position;
        point.X = pointPos.x;
        point.Y = pointPos.y;
        PlayerManager.instance.quadtree.UpdatePosition(oldPoint, point);

    }
    private IEnumerator InitialMovement(List<Point> newPoints)
    {
        _initialSpeed = maxSpeed;
        _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        while (_initialSpeed > 0)
        {
            foreach (var point in newPoints)
            {
                HandleInitialMovement(point);
            }
            yield return new WaitForEndOfFrame();
        }

        _initialSpeed = maxSpeed;
        points.AddRange(newPoints);
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

    public Point Separate(GameObject mainObject, float mass)
    {
        mass /= 2;
        // PlayerSecondaryBody playerSecondaryBodyLocal = playerSecondaryBody.GetComponent<PlayerSecondaryBody>();
        // playerSecondaryBodyLocal.mass = mass;
        // playerSecondaryBodyLocal.playerRef = playerRef;

        GameObject newPart = Instantiate(playerSecondaryBody, mainObject.transform.position, mainObject.transform.rotation);
        Point point = PlayerManager.instance.InsertPlayer(newPart.transform.position.x, newPart.transform.position.y, newPart);
        // points.Add(point);
        UpdateDiameter(mainObject.transform, mass);
        UpdateDiameter(newPart.transform, mass);

        if (mainObject == gameObject) this.mass = mass;

        return point;
    }

    // Insira esta função em um MonoBehaviour que gerencia a simulação
    // ReSharper disable Unity.PerformanceAnalysis
    public void DetectCollisions(Point point)
    {
        point.colliding = false;
        List<Point> pts = points;
        // for (int i = 0; i < pts.Count; i++)
        // {
        //     pts[i].colliding = false;
        // }
        Point a = point;
        if (a != null)
        {
            a.colliding = false;
            for (int j = 0; j < pts.Count; j++)
            {
                Point b = pts[j];
                if (a != b)
                {
                    b.colliding = false;
                    float dx = b.X - a.X;
                    float dy = b.Y - a.Y;
                    float distance = Mathf.Sqrt(dx * dx + dy * dy);
                    if (distance <= a.Radius() + b.Radius() && distance != 0)
                    {
                        a.colliding = true;
                        b.colliding = true;
                    }
                }


            }
        }
        // if (nearbyPlayers.Count > 1)
        //     {
        //         Debug.Log("Pontos encontrados na query circular dos players: " + nearbyPlayers.Count);
        //         foreach (Point point in nearbyPlayers)
        //         {

        //             GameObject obj = point.data;
        //             GameObject playerReference = obj.GetComponent<PlayerBehaviour>().playerRef;
        //             if (playerReference == playerRef)
        //             {
        //                 Debug.Log("COlidiu com o player");
        //                 colliding = true;
        //             }
        //             if (gameObject != playerRef && point.data == gameObject)
        //             {

        //                 if (gameObject.GetComponent<PlayerSecondaryBody>().canMove)
        //                 {
        //                     PlayerBehaviour player = playerRef.GetComponent<PlayerBehaviour>();
        //                     player.mass += mass;
        //                     player.UpdateDiameter();
        //                     UpdateDiameter();
        //                     PlayerManager.instance.quadtree.Remove(point);
        //                     Destroy(gameObject);
        //                 }
        //             }
        //         }
        //     }
        //     if (nearbyPlayers.Count == 1)
        //     {
        //         colliding = false;
        //     }
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
