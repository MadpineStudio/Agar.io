using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Diagnostics;
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
        playerActions.PlayerMap.Atacar.performed += Attack;
    }
    void OnDisable()
    {
        playerActions.PlayerMap.Atacar.performed -= Attack;
        playerActions.PlayerMap.Disable();
    }

    new void Start()
    {
        base.Start();
        Texture playerTex = transform.GetComponent<SpriteRenderer>().sharedMaterial.GetTexture("_MainTex0");
        playerSecondaryBody.GetComponent<SpriteRenderer>().sharedMaterial.SetTexture("_MainTex0", playerTex);
        points[0].hasFullySpawned = true;
        points[0].canBeAbsorbed = true;
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
        // Vector2 direction = playerActions.PlayerMap.Move.ReadValue<Vector2>();
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var direction = (mousePosition - (Vector2)transform.position).normalized;
        if (direction != Vector2.zero)
        {
            currentSpeed += acceleration * Time.deltaTime;
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
        }
        else
        {
            currentSpeed = 0f;
        }
        DetectCollisions(points);
        foreach (var point in points)
        {
            if (point.hasFullySpawned)
            {
                float innertSpeed = 0f;
                if (point.data != gameObject)
                {
                    if (!point.colliding)
                    {
                        direction = (Vector2)transform.position - (Vector2)point.data.transform.position + direction;
                        innertSpeed = 2f;
                    }
                  
                }
                oldPoint = point;
                point.data.transform.Translate(direction.normalized * ((currentSpeed > 0 ? currentSpeed : innertSpeed) * Time.fixedDeltaTime));
                // if()
                Vector2 pointPos = point.data.transform.position;
                point.X = pointPos.x;
                point.Y = pointPos.y;
                PlayerManager.instance.quadtree.UpdatePosition(oldPoint, point);
            }
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
        List<Point> clonedPoints = new(points);
        foreach (var playerPoint in clonedPoints)
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
                    float objMass = obj.transform.GetComponent<AbsorbableObject>().mass;
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
                    // UpdateDiameter();
                }
            }
        }

    }
    private void Explode(Point point)
    {
        List<Point> pointsToAdd = new();
        float mass = Mathf.PI * superficialDensity * Mathf.Pow(point.data.transform.localScale.x / 2, 2);
        List<GameObject> newParts = new();
        for (int i = 0; i < 4; i++)
        {
            newParts.Add(Instantiate(playerSecondaryBody, point.data.transform.position, point.data.transform.rotation));
        }
        // Point point = PlayerManager.instance.InsertPlayer(newPart.transform.position.x, newPart.transform.position.y, newPart);
        // points.Add(point);
        UpdateDiameter(point.data.transform, mass / 5);

        foreach (var newPart in newParts)
        {
            UpdateDiameter(newPart.transform, mass / 5);
            Point newPoint = PlayerManager.instance.InsertPlayer(newPart.transform.position.x, newPart.transform.position.y, newPart);
            pointsToAdd.Add(newPoint);
        }

        if (point.data == gameObject) this.mass = mass / 5;
        if (pointsToAdd != null)
        {
            StartCoroutine(EnableAbsorption(pointsToAdd, 10.0f));
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
            StartCoroutine(EnableAbsorption(pointsToAdd, 10.0f));
            StartCoroutine(InitialMovement(pointsToAdd, false));
        }
        points.AddRange(pointsToAdd);
    }
    private float HandleInitialMovement(Point point, float speed)
    {

        oldPoint = point;
        Vector2 initialDirection = (_mousePosition - (Vector2)Camera.main.transform.position).normalized;
        speed -= acceleration * (0.2f / points.Count * Time.deltaTime);
        speed = Mathf.Max(speed, 0);
        point.data.transform.Translate(initialDirection * (speed * (10 * Time.deltaTime)));
        Vector2 pointPos = point.data.transform.position;
        point.X = pointPos.x;
        point.Y = pointPos.y;
        PlayerManager.instance.quadtree.UpdatePosition(oldPoint, point);
        return speed;
    }
    private float HandleExplode(Point point, Vector2 direction, float speed)
    {
        oldPoint = point;
        speed -= acceleration * (0.3f * Time.deltaTime);
        speed = Mathf.Max(speed, 0);
        point.data.transform.Translate(direction * (speed * (30 * Time.deltaTime)));
        Vector2 pointPos = point.data.transform.position;
        point.X = pointPos.x;
        point.Y = pointPos.y;
        PlayerManager.instance.quadtree.UpdatePosition(oldPoint, point);
        return speed;
    }
    private IEnumerator InitialMovement(List<Point> newPoints, bool exploding)
    {
        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.right, Vector2.left };
        _initialSpeed = maxSpeed;
        float speed = _initialSpeed;
        _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        while (speed > 0)
        {
            foreach (var point in newPoints)
            {
                if (!exploding) speed = HandleInitialMovement(point, speed);
                else speed = HandleExplode(point, directions[newPoints.IndexOf(point)], speed);
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
            Point old = point;
            point.canBeAbsorbed = true;
            PlayerManager.instance.quadtree.UpdatePosition(old, point);
        }
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
    public void DetectCollisions(List<Point> points)
    {
        List<Point> toDestroy = new(points);
        for (int i = toDestroy.Count - 1; i >= 0; i--)
        {
            if (i > toDestroy.Count - 1) i = toDestroy.Count - 1;
            toDestroy[i].colliding = false;
            List<Point> pts = points;
            Point a = toDestroy[i];
            if (a != null)
            {
                a.colliding = false;
                for (int j = pts.Count - 1; j >= 0; j--)
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

                            if (b.data == gameObject)
                            {
                                Point changer = a;
                                a = b;
                                b = changer;
                            }
                            if (a.hasFullySpawned && b.hasFullySpawned && b.canBeAbsorbed)
                            {

                                float mass = Mathf.PI * superficialDensity * Mathf.Pow(a.data.transform.localScale.x / 2, 2);
                                float bMass = Mathf.PI * superficialDensity * Mathf.Pow(b.data.transform.localScale.x / 2, 2);
                                UpdateDiameter(a.data.transform, mass + bMass);
                                // this.mass += mass;
                                PlayerManager.instance.quadtree.Remove(b);
                                pts.Remove(b);
                                toDestroy.Remove(b);
                                this.points.Remove(b);
                                Destroy(b.data);
                            }
                        }

                    }



                }
            }
        }
    }


}
