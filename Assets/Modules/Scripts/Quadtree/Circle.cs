using UnityEngine;

public class Circle
{
    public float X, Y, R, RSquared;

    public Circle(float x, float y, float r)
    {
        X = x;
        Y = y;
        R = r;
        RSquared = r * r;
    }

    public bool Contains(Point point)
    {
        float d = Mathf.Pow(point.X - X, 2) + Mathf.Pow(point.Y - Y, 2);
        return d <= RSquared;
    }

    public bool Intersects(Rectangle range)
    {
        float xDist = Mathf.Abs(range.X - X);
        float yDist = Mathf.Abs(range.Y - Y);

        float r = R;
        float w = range.W;
        float h = range.H;

        float edges = Mathf.Pow(xDist - w, 2) + Mathf.Pow(yDist - h, 2);

        if (xDist > (r + w) || yDist > (r + h))
            return false;

        if (xDist <= w || yDist <= h)
            return true;

        return edges <= RSquared;
    }
}