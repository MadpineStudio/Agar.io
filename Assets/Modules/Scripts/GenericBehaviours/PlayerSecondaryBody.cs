using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class PlayerSecondaryBody : PlayerBehaviour
{
    private float _initialRunningTime;
    private float _initialSpeed;
    public bool canMove;
    private Vector2 _mousePosition;
    public float minSpeed;

    // public GameObject playerRef;
    new void Start()
    {
        base.Start();
        _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _initialSpeed = maxSpeed;
        _initialRunningTime = Time.time + 1;
        StartCoroutine(InitialMovement());
    }
    void Update()
    {
        QueryNearbyCircle();
    }
    new void FixedUpdate()
    {
        // if(canMove) base.FixedUpdate();
        // DetectCollisions();
        if (!colliding && canMove) ReturningMovement();
    }
    new void OnDrawGizmos()
    {

    }
    private void ReturningMovement()
    {
        Vector3 oldpos = transform.position;
        Vector2 direction = playerRef.transform.position - transform.position;
        transform.Translate(direction.normalized * (_initialSpeed * Time.deltaTime));
        if (oldpos != transform.position)
        {
            Point newPoint = new Point(transform.position.x, transform.position.y, gameObject);
            PlayerManager.instance.quadtree.UpdatePosition(oldPoint, newPoint);
            oldPoint = newPoint;
        }
    }
    private void HandleInitialMovement()
    {
        Vector3 oldpos = transform.position;
        Vector2 initialDirection = (_mousePosition - (Vector2)Camera.main.transform.position).normalized;
        _initialSpeed -= acceleration * (0.2f * Time.deltaTime);
        _initialSpeed = Mathf.Max(_initialSpeed, minSpeed);
        transform.Translate(initialDirection * (_initialSpeed * (10 * Time.deltaTime)));
        if (oldpos != transform.position)
        {
            Point newPoint = new Point(transform.position.x, transform.position.y, gameObject);
            PlayerManager.instance.quadtree.UpdatePosition(oldPoint, newPoint);
            oldPoint = newPoint;
        }
    }
    private IEnumerator InitialMovement()
    {
        while (Time.time < _initialRunningTime || _initialSpeed > 0)
        {
            HandleInitialMovement();
            yield return new WaitForEndOfFrame();
        }

        canMove = true;
        _initialSpeed = maxSpeed;
    }
}
