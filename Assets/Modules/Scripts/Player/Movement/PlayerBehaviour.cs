using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static QTreeEntryPoint;

public class PlayerBehaviour : AbsorbableObject
{
    private PlayerActions playerActions; 
    public float speed;
    public Rigidbody2D playerRb;
    private Vector2 playerMovement;
    private List<Point> nearbyPoints;
    
    [SerializeField] private PlayerSettings playerSettings;
    void Awake(){
        playerActions = new();
        playerActions.Enable();
    }

    void OnEnable(){
        playerActions.PlayerMap.Enable();
        playerActions.PlayerMap.Move.ReadValue<Vector2>();
        playerActions.PlayerMap.Atacar.performed += Atack;
    }
    void OnDisable(){
        playerActions.PlayerMap.Atacar.performed -= Atack;
        playerActions.PlayerMap.Disable();
    }

    new void Start()
    {
        base.Start();
    }

    public void FixedUpdate()
    {
        Vector2 previousPosition = playerRb.position; 
        playerMovement = playerActions.PlayerMap.Move.ReadValue<Vector2>() * (speed * Time.fixedDeltaTime);
        Vector2 newPosition = previousPosition + playerMovement * (speed * Time.fixedDeltaTime);

        playerRb.MovePosition(newPosition);

        // Point oldPoint = new Point(previousPosition.x, previousPosition.y, this);
        // Point newPoint = new Point(newPosition.x, newPosition.y, this);
        // instance.QuadTree.UpdatePosition(oldPoint, newPoint);
    }

    private void Update()
    {
        QueryNearbyPoints();
    }
    // Em PlayerBehaviour.cs
    public void QueryNearbyPoints()
    {
        if (instance?.QuadTree == null) return;
        Vector2 pos = playerRb.position;
        Rectangle queryRange = new Rectangle(pos.x, pos.y, playerSettings.searchAreaWidth, playerSettings.searchAreaHeight);
        nearbyPoints = instance.QuadTree.Query(queryRange);
    }
    
    private void Atack(InputAction.CallbackContext context){
        
    }

    private void OnDrawGizmos()
    {
        if (!playerSettings.debug) return;

        if (nearbyPoints == null) return;
        foreach (var p in nearbyPoints)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(new Vector3(p.X, p.Y, 0), playerSettings.foundNearGizmoSize);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(playerSettings.searchAreaWidth * 2f, playerSettings.searchAreaHeight* 2f));
    }
}
