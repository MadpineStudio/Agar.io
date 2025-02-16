using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static QTreeEntryPoint;

public class PlayerBehaviour : AbsorbableObject
{
    private PlayerActions playerActions;
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

    void Awake()
    {
        playerActions = new();
        playerActions.Enable();
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
        // Vector2 previousPosition = playerRb.position;
        // playerMovement = playerActions.PlayerMap.Move.ReadValue<Vector2>() * (speed * Time.fixedDeltaTime);
        // Vector2 newPosition = previousPosition + playerMovement * (speed * Time.fixedDeltaTime);

        // playerRb.MovePosition(newPosition);
        HandleMovement();
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
            currentSpeed = 0f; // Reseta a velocidade ao soltar os botões
        }

        transform.Translate(direction.normalized * (currentSpeed * Time.deltaTime));
    }

    private void Update()
    {
        QueryNearbyPoints();
        QueryNearbyCircle();
    }
    // Em PlayerBehaviour.cs
    public void QueryNearbyPoints()
    {
        if (instance?.QuadTree == null) return;
        Vector2 pos = transform.position;
        Rectangle queryRange = new Rectangle(pos.x, pos.y, playerSettings.searchAreaWidth, playerSettings.searchAreaHeight);
        nearbyPoints = instance.QuadTree.Query(queryRange);
    }
    public void QueryNearbyCircle()
    {
        if (instance?.QuadTree == null)
            return;
        Vector2 pos = transform.position;
        float radius = this.radius * transform.localScale.x;
        var nearbyPoints = instance.QuadTree.QueryCircle(pos, radius);
        if (nearbyPoints.Count > 0)
        {
            Debug.Log("Pontos encontrados na query circular: " + nearbyPoints.Count);
            foreach (Point point in nearbyPoints)
            {
                GameObject obj = point.data as GameObject;
                mass += obj.transform.GetComponent<AbsorbableObject>().mass;
                UpdateDiameter();
                Destroy(point.data as GameObject);
                instance.QuadTree.Remove(point);
            }
        }
    }

    private void Atack(InputAction.CallbackContext context)
    {
        if(mass > 500) Separate();
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

    public void Separate(){
        mass /= 2;
        playerSecondaryBody.GetComponent<PlayerSecondaryBody>().mass = mass;

        Instantiate(playerSecondaryBody, transform.position, transform.rotation);
        UpdateDiameter();        
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
