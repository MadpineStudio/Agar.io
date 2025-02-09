using UnityEngine;
using System.Collections.Generic;

public class SoftBody2D : MonoBehaviour
{
    public int nodesCount = 8;
    public float radius = 1f, mass = 1f, springStiffness = 500f, damping = 5f, nodeRadius = 0.1f;
    public float internalSpringStiffness = 100f; // Rigidez das springs internas
    public LayerMask collisionLayers;
    List<SoftBodyNode> nodes;
    List<SoftBodySpring> springs;
    PolygonCollider2D polyCollider;

    void Awake()
    {
        nodes = new List<SoftBodyNode>();
        springs = new List<SoftBodySpring>();
        for (int i = 0; i < nodesCount; i++)
        {
            float ang = i * Mathf.PI * 2 / nodesCount;
            Vector2 pos = new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * radius;
            nodes.Add(new SoftBodyNode(pos, mass));
        }
        for (int i = 0; i < nodesCount; i++)
        {
            int next = (i + 1) % nodesCount;
            float rest = Vector2.Distance(nodes[i].Position, nodes[next].Position);
            springs.Add(new SoftBodySpring(nodes[i], nodes[next], rest, springStiffness, damping));
        }

        // Adicionar springs internas conectando cada nó ao centro de massa
        Vector2 centerOfMass = CalculateCenterOfMass();
        for (int i = 0; i < nodesCount; i++)
        {
            float restLength = Vector2.Distance(nodes[i].Position, centerOfMass);
            springs.Add(new SoftBodySpring(nodes[i], new SoftBodyNode(centerOfMass, mass), restLength, internalSpringStiffness, damping));
        }

        polyCollider = gameObject.AddComponent<PolygonCollider2D>();
        UpdateCollider();
    }

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;
        foreach (var s in springs) s.Update(dt);
        foreach (var n in nodes)
        {
            n.ApplyForce(Physics2D.gravity * mass);
            n.Update(dt);
            SoftBodyCollisionHandler.ResolveCollision(n, (Vector2)transform.position, nodeRadius, collisionLayers);
        }
        UpdateCollider();
    }

    void UpdateCollider()
    {
        Vector2[] pts = new Vector2[nodes.Count];
        for (int i = 0; i < nodes.Count; i++)
            pts[i] = nodes[i].Position;
        polyCollider.SetPath(0, pts);
    }

    Vector2 CalculateCenterOfMass()
    {
        Vector2 sum = Vector2.zero;
        foreach (var node in nodes) sum += node.Position;
        return sum / nodes.Count;
    }

    void OnDrawGizmos()
    {
        if (nodes == null || nodes.Count == 0) return;

        // Desenhar os bounds do shape em preto
        Gizmos.color = Color.black;
        Vector3 center = transform.position;
        for (int i = 0; i < nodes.Count; i++)
        {
            Vector3 current = center + (Vector3)nodes[i].Position;
            Vector3 next = center + (Vector3)nodes[(i + 1) % nodes.Count].Position;
            Gizmos.DrawLine(current, next);
        }

        // Desenhar as springs em amarelo
        Gizmos.color = Color.yellow;
        foreach (var spring in springs)
        {
            Vector3 start = center + (Vector3)spring.A.Position;
            Vector3 end = center + (Vector3)spring.B.Position;
            Gizmos.DrawLine(start, end);
        }

        // Desenhar os nós
        Gizmos.color = Color.green;
        foreach (var node in nodes)
        {
            Gizmos.DrawSphere(center + (Vector3)node.Position, nodeRadius);
        }
    }
}