using UnityEngine;

public class SoftBodyPoint
{
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public float Mass { get; set; }

    public SoftBodyPoint(Vector2 position, float mass)
    {
        Position = position;
        Velocity = Vector2.zero;
        Mass = mass;
    }

    public void ApplyForce(Vector2 force)
    {
        Velocity += force / Mass;
    }

    public void ApplyGravity(float gravity)
    {
        Velocity += Vector2.down * gravity;
    }

    public void Update(float deltaTime)
    {
        Position += Velocity * deltaTime;
    }
}