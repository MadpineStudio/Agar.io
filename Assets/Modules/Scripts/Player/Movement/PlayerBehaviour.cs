using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : AbsorbableObject
{
    private PlayerActions playerActions; 
    public float speed;
    public Rigidbody2D playerRb;
    private Vector2 playerMovement;
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

    void FixedUpdate()
    {
        playerMovement = playerActions.PlayerMap.Move.ReadValue<Vector2>() * speed * Time.fixedDeltaTime;
        playerRb.MovePosition(playerRb.position + playerMovement);
    }
    void Update()
    {
    }
    private void Atack(InputAction.CallbackContext context){
        
    }
}
