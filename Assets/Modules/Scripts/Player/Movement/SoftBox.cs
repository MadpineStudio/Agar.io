using UnityEngine;

public class SoftBox : SoftBody
{
    public SoftBox(Vector2 position, float width, float height, float mass, float stiffness)
    {
        // Criar pontos nos cantos da caixa
        SoftBodyPoint topLeft = new SoftBodyPoint(position + new Vector2(-width / 2, height / 2), mass);
        SoftBodyPoint topRight = new SoftBodyPoint(position + new Vector2(width / 2, height / 2), mass);
        SoftBodyPoint bottomLeft = new SoftBodyPoint(position + new Vector2(-width / 2, -height / 2), mass);
        SoftBodyPoint bottomRight = new SoftBodyPoint(position + new Vector2(width / 2, -height / 2), mass);

        AddPoint(topLeft);
        AddPoint(topRight);
        AddPoint(bottomLeft);
        AddPoint(bottomRight);

        // Conectar os pontos com molas
        AddSpring(new Spring(topLeft, topRight, stiffness));
        AddSpring(new Spring(topRight, bottomRight, stiffness));
        AddSpring(new Spring(bottomRight, bottomLeft, stiffness));
        AddSpring(new Spring(bottomLeft, topLeft, stiffness));

        // Molas diagonais para mais rigidez
        AddSpring(new Spring(topLeft, bottomRight, stiffness));
        AddSpring(new Spring(topRight, bottomLeft, stiffness));
    }
}