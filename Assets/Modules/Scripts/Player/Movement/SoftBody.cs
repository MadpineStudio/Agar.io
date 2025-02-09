using System.Collections.Generic;
using UnityEngine;

public class SoftBody
{
    public List<SoftBodyPoint> Points { get; private set; }
    public List<Spring> Springs { get; private set; }

    public SoftBody()
    {
        Points = new List<SoftBodyPoint>();
        Springs = new List<Spring>();
    }

    public void AddPoint(SoftBodyPoint point)
    {
        Points.Add(point);
    }

    public void AddSpring(Spring spring)
    {
        Springs.Add(spring);
    }

    public void ApplyGravity(float gravity)
    {
        foreach (var point in Points)
        {
            point.ApplyGravity(gravity);
        }
    }

    public void Update(float deltaTime)
    {
        foreach (var spring in Springs)
        {
            spring.Update();
        }

        foreach (var point in Points)
        {
            point.Update(deltaTime);
        }
    }

    // Método para verificar colisão com outro softbody
    public void CheckCollision(SoftBody other, float collisionRadius, float repulsionForce)
    {
        foreach (var pointA in this.Points)
        {
            foreach (var pointB in other.Points)
            {
                Vector2 delta = pointB.Position - pointA.Position;
                float distance = delta.magnitude;

                if (distance < collisionRadius && distance > 0)
                {
                    Vector2 direction = delta.normalized;
                    float penetrationDepth = collisionRadius - distance;

                    // Separação posicional para evitar interpenetração
                    float correctionFactor = 0.5f;
                    Vector2 correction = direction * (penetrationDepth * correctionFactor);

                    pointA.Position -= correction;
                    pointB.Position += correction;

                    // Aplicação de força de repulsão proporcional à profundidade
                    Vector2 force = direction * (repulsionForce * penetrationDepth);
                    pointA.ApplyForce(-force);
                    pointB.ApplyForce(force);
                }
            }
        }
    }

    public bool IsCollidingWith(SoftBody other)
    {
        Bounds aabbA = CalculateAABB();
        Bounds aabbB = other.CalculateAABB();

        return aabbA.Intersects(aabbB);
    }

    public Bounds CalculateAABB()
    {
        if (Points.Count == 0) return new Bounds(Vector2.zero, Vector2.zero);

        Vector2 min = Points[0].Position;
        Vector2 max = Points[0].Position;

        foreach (var point in Points)
        {
            min = Vector2.Min(min, point.Position);
            max = Vector2.Max(max, point.Position);
        }

        Vector2 size = max - min;
        return new Bounds((min + max) / 2, size);
    }
}