using UnityEngine;

public class SoftBodyInstance : MonoBehaviour
{
    public float width = 2f;
    public float height = 2f;
    public float mass = 1f;
    public float stiffness = 10f;
    public float gravity = 9.81f;
    public Vector2 initialVelocity = Vector2.zero;

    public SoftBody softBody;

    void Start()
    {
        // Criar um softbody (caixa)
        softBody = new SoftBox(transform.position, width, height, mass, stiffness);

        // Aplicar velocidade inicial
        foreach (var point in softBody.Points)
        {
            point.Velocity = initialVelocity;
        }

        // Registrar este softbody no gerenciador global
        SoftBodyManager.Instance.RegisterSoftBody(this);
    }

    void Update()
    {
        // Aplicar gravidade
        softBody.ApplyGravity(gravity * Time.deltaTime);

        // Atualizar o softbody
        softBody.Update(Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        if (softBody == null) return;

        // Desenhar os pontos
        Gizmos.color = Color.red;
        foreach (var point in softBody.Points)
        {
            Gizmos.DrawSphere(point.Position, 0.1f);
        }

        // Desenhar as molas
        Gizmos.color = Color.green;
        foreach (var spring in softBody.Springs)
        {
            Gizmos.DrawLine(spring.PointA.Position, spring.PointB.Position);
        }

        // Desenhar o centro de massa
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(CalculateCenterOfMass(), 0.15f);
    }

    public Vector2 CalculateCenterOfMass()
    {
        Vector2 sum = Vector2.zero;
        foreach (var point in softBody.Points) sum += point.Position;
        return sum / softBody.Points.Count;
    }
}