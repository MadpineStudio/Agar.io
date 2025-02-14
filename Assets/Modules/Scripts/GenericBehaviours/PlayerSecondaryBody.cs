using UnityEngine;

public class PlayerSecondaryBody : PlayerBehaviour
{
    public GameObject playerRef;
    private float delay;
    void Start()
    {
        delay = Time.time + 2;
        float newDiameter = Mathf.Sqrt(this.mass / (Mathf.PI * superficialDensity)) * 2;
        transform.localScale = new Vector3(newDiameter, newDiameter, newDiameter);
    }
    void Update()
    {
        if (Time.time > delay)
        {
            Vector2 direction = (playerRef.transform.position - transform.position) / (playerRef.transform.position - transform.position).magnitude * 600 * Time.deltaTime;
            playerRb.AddForce(direction, ForceMode2D.Force);

        }

    }
}
