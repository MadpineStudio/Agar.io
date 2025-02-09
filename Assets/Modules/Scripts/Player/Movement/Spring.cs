using UnityEngine;

public class Spring
{
    public SoftBodyPoint PointA { get; private set; }
    public SoftBodyPoint PointB { get; private set; }
    public float RestLength { get; private set; }
    public float Stiffness { get; private set; }

    public Spring(SoftBodyPoint pointA, SoftBodyPoint pointB, float stiffness)
    {
        PointA = pointA;
        PointB = pointB;
        RestLength = Vector2.Distance(pointA.Position, pointB.Position);
        Stiffness = stiffness;
    }

    public void Update()
    {
        Vector2 delta = PointB.Position - PointA.Position;
        float currentLength = delta.magnitude;
        float forceMagnitude = Stiffness * (currentLength - RestLength);
        Vector2 force = delta.normalized * forceMagnitude;

        PointA.ApplyForce(force);
        PointB.ApplyForce(-force);
    }
}