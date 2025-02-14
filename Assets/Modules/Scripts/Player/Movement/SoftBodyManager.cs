using UnityEngine;
using System.Collections.Generic;

public class SoftBodyManager : MonoBehaviour
{
    public static SoftBodyManager Instance { get; private set; }
    public float collisionRadius = 0.3f;
    public float repulsionForce = 150f;
    private List<SoftBodyInstance> softBodies = new List<SoftBodyInstance>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterSoftBody(SoftBodyInstance softBody)
    {
        softBodies.Add(softBody);
    }

    void Update()
    {
        // Aplicar colisões entre todos os SoftBodies registrados
        for (int i = 0; i < softBodies.Count; i++)
        {
            for (int j = i + 1; j < softBodies.Count; j++)
            {
                SoftBody bodyA = softBodies[i].softBody;
                SoftBody bodyB = softBodies[j].softBody;

                if (bodyA.IsCollidingWith(bodyB))
                {
                    bodyA.CheckCollision(bodyB, collisionRadius, repulsionForce);
                }
            }
        }
    }
}