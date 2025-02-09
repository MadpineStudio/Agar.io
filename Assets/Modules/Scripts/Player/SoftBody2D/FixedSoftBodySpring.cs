using UnityEngine;

public class FixedSoftBodySpring {
    SoftBodyNode node;
    Vector2 fixedPoint;
    float restLength, stiffness, damping;
    public FixedSoftBodySpring(SoftBodyNode node, Vector2 fixedPoint, float stiffness, float damping) {
        this.node = node;
        this.fixedPoint = fixedPoint;
        this.stiffness = stiffness;
        this.damping = damping;
        restLength = (node.Position - fixedPoint).magnitude;
    }
    public void Update(float dt) {
        Vector2 delta = node.Position - fixedPoint;
        float dist = delta.magnitude;
        if (dist == 0f) return;
        Vector2 n = delta / dist;
        float springForce = -stiffness * (dist - restLength);
        float dampingForce = -damping * Vector2.Dot(node.Velocity, n);
        Vector2 force = n * (springForce + dampingForce);
        node.Velocity += force / node.Mass * dt;
    }
}