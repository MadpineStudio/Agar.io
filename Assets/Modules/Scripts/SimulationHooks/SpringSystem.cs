using UnityEngine;

namespace hooks
{
    public class SpringSystem : MonoBehaviour
    {
        //     public Node NodeA;
        //     public Node NodeB;
        //     public Spring Spring;
        //
        //     public float gravity = 9.81f;
        //     public float stiffness = 100f;
        //     public float damping = 5f;
        //
        //     void Start()
        //     {
        //         NodeA = new Node(new Vector3(-1, 0, 0), 1, true);
        //         NodeB = new Node(new Vector3(1, 0, 0), 1);
        //         Spring = new Spring(NodeA, NodeB, stiffness, damping);
        //     }
        //
        //     void FixedUpdate()
        //     {
        //         NodeB.ApplyForce(Vector3.down * gravity * NodeB.Mass);
        //         Spring.ApplySpringForce();
        //
        //         NodeA.UpdatePositionVerlet(Time.fixedDeltaTime);
        //         NodeB.UpdatePositionVerlet(Time.fixedDeltaTime);
        //
        //         Debug.DrawLine(NodeA.Position, NodeB.Position, Color.green);
        //     }
        // }
    }
}