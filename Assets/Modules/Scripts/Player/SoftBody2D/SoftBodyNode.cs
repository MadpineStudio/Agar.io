using UnityEngine;

public class SoftBodyNode
{
    public Vector2 Position;
    public Vector2 Velocity;
    public float Mass;

    public SoftBodyNode(Vector2 pos, float mass)
    {
        Position = pos;
        Velocity = Vector2.zero;
        Mass = mass;
    }

    public void ApplyForce(Vector2 force)
    {
        Velocity += force / Mass * Time.fixedDeltaTime;
    }

    public void Update(float dt)
    {
        Position += Velocity * dt;
    }
}