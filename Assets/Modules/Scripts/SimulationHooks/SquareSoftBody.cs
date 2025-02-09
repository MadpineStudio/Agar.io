using System;

namespace hooks
{
    using UnityEngine;

    public class SquareSoftBody : MonoBehaviour
    {
        public Node[] nodes = new Node[4];
        public Spring[] springs = new Spring[6];
        public float gravity = 9.81f;
        public float stiffness = 10f;
        public float damping = 1f;
        public float nodeRadius = 0.2f;
        private Vector3 lastPivot;
        private Node draggedNode;

        void Start()
        {
            nodes[0] = new Node(new Vector3(-1, 1, 0), 1);
            nodes[1] = new Node(new Vector3(1, 1, 0), 1);
            nodes[2] = new Node(new Vector3(1, -1, 0), 1);
            nodes[3] = new Node(new Vector3(-1, -1, 0), 1);

            springs[0] = new Spring(nodes[0], nodes[1], stiffness, damping);
            springs[1] = new Spring(nodes[1], nodes[2], stiffness, damping);
            springs[2] = new Spring(nodes[2], nodes[3], stiffness, damping);
            springs[3] = new Spring(nodes[3], nodes[0], stiffness, damping);
            springs[4] = new Spring(nodes[0], nodes[2], stiffness, damping);
            springs[5] = new Spring(nodes[1], nodes[3], stiffness, damping);

            CenterNodes();
            lastPivot = transform.position;
        }


        void FixedUpdate()
        {
            if (transform.position != lastPivot)
            {
                Vector3 extDelta = transform.position - lastPivot;
                for (int i = 0; i < nodes.Length; i++)
                {
                    nodes[i].Position += extDelta;
                    nodes[i].PreviousPosition += extDelta;
                }

                lastPivot = transform.position;
            }

            for (int i = 0; i < nodes.Length; i++)
            {
                if (!nodes[i].IsFixed)
                    nodes[i].ApplyForce(Vector3.down * gravity * nodes[i].Mass);
            }

            for (int i = 0; i < springs.Length; i++)
                springs[i].ApplySpringForce();

            for (int i = 0; i < nodes.Length; i++)
            {
                if (!nodes[i].IsFixed)
                {
                    Vector3 newPos = nodes[i].Position +
                                     (nodes[i].Position - nodes[i].PreviousPosition) +
                                     (nodes[i].AccumulatedForce / nodes[i].Mass) *
                                     (Time.fixedDeltaTime * Time.fixedDeltaTime);
                    nodes[i].PreviousPosition = nodes[i].Position;
                    nodes[i].Position = newPos;
                    nodes[i].AccumulatedForce = Vector3.zero;
                }
            }

            CenterNodes();
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mp = Input.mousePosition;
                mp.z = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(mp);
                for (int i = 0; i < nodes.Length; i++)
                {
                    Vector3 globalPos = transform.position + nodes[i].Position;
                    if (Vector3.Distance(worldPos, globalPos) < nodeRadius)
                    {
                        draggedNode = nodes[i];
                        draggedNode.PreviousPosition = draggedNode.Position = worldPos - transform.position;
                        break;
                    }
                }
            }

            if (Input.GetMouseButton(0) && draggedNode != null)
            {
                Vector3 mp = Input.mousePosition;
                mp.z = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(mp);
                draggedNode.PreviousPosition = draggedNode.Position = worldPos - transform.position;
            }

            if (Input.GetMouseButtonUp(0))
                draggedNode = null;
        }

        private void CenterNodes()
        {
            // Calcula o centro de massa somente dos nós que NÃO estão sendo arrastados.
            Vector3 centerOfMass = Vector3.zero;
            int count = 0;
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i] != draggedNode)
                {
                    centerOfMass += nodes[i].Position;
                    count++;
                }
            }
            if (count == 0)
                return;

            centerOfMass /= count;
            Vector3 offset = transform.position - centerOfMass;

            // Aplica o offset apenas nos nós não arrastados.
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i] != draggedNode)
                {
                    nodes[i].Position += offset;
                    nodes[i].PreviousPosition += offset;
                }
            }
        }
        private void DrawSprings()
        {
            if (springs == null)
                return;

            Gizmos.color = Color.green;

            for (int i = 0; i < springs.Length; i++)
            {
                if (springs[i] == null)
                    continue;

                Vector3 nodeAPos = transform.position + springs[i].NodeA.Position;
                Vector3 nodeBPos = transform.position + springs[i].NodeB.Position;

                Gizmos.DrawLine(nodeAPos, nodeBPos);
            }
        }


        void OnDrawGizmos()
        {
            if (nodes == null) return;
            
            DrawSprings();
            Gizmos.color = Color.yellow;
            for (int i = 0; i < nodes.Length; i++)
                Gizmos.DrawSphere(transform.position + nodes[i].Position, nodeRadius);
        }
    }
}