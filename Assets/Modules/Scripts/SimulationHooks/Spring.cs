using UnityEngine;
namespace hooks
{
    public class Spring
    {
        public Node NodeA;
        public Node NodeB;
        public float RestLength;
        public float Stiffness;
        public float Damping;

        public Spring(Node nodeA, Node nodeB, float stiffness, float damping)
        {
            NodeA = nodeA;
            NodeB = nodeB;
            Stiffness = stiffness;
            Damping = damping;
            RestLength = Vector3.Distance(nodeA.Position, nodeB.Position);
        }

        public void ApplySpringForce()
        {
            Vector3 direction = NodeB.Position - NodeA.Position;
            float currentLength = direction.magnitude;
            Vector3 springForce = direction.normalized * ((currentLength - RestLength) * Stiffness);

            Vector3 velA = (NodeA.Position - NodeA.PreviousPosition) / Time.fixedDeltaTime;
            Vector3 velB = (NodeB.Position - NodeB.PreviousPosition) / Time.fixedDeltaTime;
            Vector3 dampingForce = (velB - velA) * Damping;

            NodeA.ApplyForce(springForce + dampingForce);
            NodeB.ApplyForce(-springForce - dampingForce);
        }
    }
}
