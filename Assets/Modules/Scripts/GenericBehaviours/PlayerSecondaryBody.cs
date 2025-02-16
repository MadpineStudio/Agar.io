using System.Collections;
using UnityEngine;

public class PlayerSecondaryBody : PlayerBehaviour
{
    private float _initialRunningTime;
    private float _initialSpeed;
    private bool _canMove;
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
        if(_canMove) base.FixedUpdate();
    }
    new void OnDrawGizmos()
    {

    }
    private void HandleInitialMovement()
    {
        Vector2 initialDirection = (_mousePosition - (Vector2)transform.position).normalized;
        _initialSpeed -= acceleration * (0.2f * Time.deltaTime);
        _initialSpeed = Mathf.Max(_initialSpeed, minSpeed);
        transform.Translate(initialDirection * (_initialSpeed * (10 * Time.deltaTime)));
    }
    private IEnumerator InitialMovement()
    {
        while (Time.time < _initialRunningTime)
        {
            HandleInitialMovement();
            yield return new WaitForEndOfFrame();
        }
        _canMove = true;
        _initialSpeed = maxSpeed;
    }
}
