using Unity.Mathematics;
using UnityEngine;

public class AbsorbableObject : MonoBehaviour
{
    public float mass;
    [SerializeField] private bool canAbsorb;
    private float superficialDensity = 100;
    void Start()
    {
        mass = 10;

    }

    void Update()
    {

    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (canAbsorb && other.CompareTag("Absorbable"))
        {
            float otherMass = other.transform.gameObject.GetComponent<AbsorbableObject>().mass;
            if (otherMass < (mass * .75f))
            {
                Absorb(other.transform.gameObject, otherMass);
            }
        }
    }
    public void Absorb(GameObject Absorbed, float mass)
    {
        Destroy(Absorbed);
        this.mass += mass;
        float newDiameter = Mathf.Sqrt(this.mass / (Mathf.PI * superficialDensity)) * 2;
        transform.localScale = new Vector3(newDiameter, newDiameter, newDiameter);
    }
}
