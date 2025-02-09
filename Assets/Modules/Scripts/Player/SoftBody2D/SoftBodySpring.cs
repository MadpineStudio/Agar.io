using UnityEngine;

public class SoftBodySpring
{
    public SoftBodyNode A { get; private set; }
    public SoftBodyNode B { get; private set; }
    float restLength, stiffness, damping;

    public SoftBodySpring(SoftBodyNode a, SoftBodyNode b, float restLength, float stiffness, float damping)
    {
        this.A = a;
        this.B = b;
        this.restLength = restLength;
        this.stiffness = stiffness;
        this.damping = damping;
    }

    public void Update(float dt)
    {
        Vector2 delta = B.Position - A.Position;
        float dist = delta.magnitude;
        float forceMag = (dist - restLength) * stiffness;
        Vector2 force = delta.normalized * forceMag;
        A.Velocity += force / A.Mass * dt;
        B.Velocity -= force / B.Mass * dt;
    }
}