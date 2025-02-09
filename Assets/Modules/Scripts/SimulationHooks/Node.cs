using UnityEngine;

namespace hooks
{
    using UnityEngine;

    public class Node
    {
        public Vector3 Position;
        public Vector3 PreviousPosition;
        public Vector3 AccumulatedForce;
        public float Mass;
        public bool IsFixed;

        public Node(Vector3 position, float mass, bool isFixed = false)
        {
            Position = position;
            PreviousPosition = position;
            Mass = mass;
            IsFixed = isFixed;
            AccumulatedForce = Vector3.zero;
        }

        public void ApplyForce(Vector3 force)
        {
            if (IsFixed) return;
            AccumulatedForce += force;
        }
    }
}
