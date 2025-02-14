using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : AbsorbableObject
{
    private PlayerActions playerActions; 
    [SerializeField] private float speed;
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

    void Update()
    {
        playerMovement = playerActions.PlayerMap.Move.ReadValue<Vector2>();
        playerRb.linearVelocity = playerMovement * speed * Time.deltaTime;
    }
    private void Atack(InputAction.CallbackContext context){
        
    }
}
