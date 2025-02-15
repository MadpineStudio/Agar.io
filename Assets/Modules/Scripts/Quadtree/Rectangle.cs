public class Rectangle
{
    public float X, Y, W, H;

    public Rectangle(float x, float y, float w, float h)
    {
        X = x;
        Y = y;
        W = w;
        H = h;
    }

    public bool Contains(Point point)
    {
        return (point.X >= X - W &&
                point.X <= X + W &&
                point.Y >= Y - H &&
                point.Y <= Y + H);
    }

    public bool Intersects(Rectangle range)
    {
        return !(range.X - range.W > X + W ||
                 range.X + range.W < X - W ||
                 range.Y - range.H > Y + H ||
                 range.Y + range.H < Y - H);
    }
}