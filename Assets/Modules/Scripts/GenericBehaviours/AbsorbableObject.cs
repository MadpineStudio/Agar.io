using Unity.Mathematics;
using UnityEngine;

public class AbsorbableObject : MonoBehaviour
{
    public float mass;
    public bool isBacteria;
    [SerializeField] private bool canAbsorb;
    [SerializeField] private GameObject playerSecondaryBody;
    public float superficialDensity = 100;
    public void Start()
    {
        float newDiameter = Mathf.Sqrt(this.mass / (Mathf.PI * superficialDensity)) * 2;
        transform.localScale = new Vector3(newDiameter, newDiameter, newDiameter);
    }

    void Update()
    {

    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (canAbsorb && other.CompareTag("Absorbable"))
        {
            AbsorbableObject otherObject = other.transform.gameObject.GetComponent<AbsorbableObject>();
            if (otherObject.mass < (mass * .75f))
            {
                Absorb(other.transform.gameObject, otherObject.mass, otherObject.isBacteria);
            }
        }
    }
    public void Absorb(GameObject Absorbed, float mass, bool isBacteria)
    {
        Destroy(Absorbed);
        this.mass += mass;
        if (isBacteria)
        {
            this.mass /= 5;
            for (int i = 0; i < 4; i++)
            {
                PlayerSecondaryBody playeSecBInstance = playerSecondaryBody.GetComponent<PlayerSecondaryBody>();
                playeSecBInstance.mass = this.mass;
                playeSecBInstance.playerRef = gameObject;
                GameObject secondBody = Instantiate(playerSecondaryBody, transform.position, transform.rotation);
                secondBody.GetComponent<Rigidbody2D>().AddForce(new Vector2(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1)) * 1000 * Time.deltaTime, ForceMode2D.Impulse);

            }
        }
        float newDiameter = Mathf.Sqrt(this.mass / (Mathf.PI * superficialDensity)) * 2;
        transform.localScale = new Vector3(newDiameter, newDiameter, newDiameter);
    }
}
