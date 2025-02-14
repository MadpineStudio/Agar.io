using UnityEngine;

public class PlayerSecondaryBody : PlayerBehaviour
{
    public GameObject playerRef;
    private float delay;
    void Start()
    {
        delay = Time.time + 0.5F;
        float newDiameter = Mathf.Sqrt(this.mass / (Mathf.PI * superficialDensity)) * 2;
        transform.localScale = new Vector3(newDiameter, newDiameter, newDiameter);
    }
    void Update()
    {
    }
    new void FixedUpdate()
    {
        base.FixedUpdate();
        if (Time.time > delay)
        {
            // Vector2 direction = (playerRef.transform.position - transform.position) / (playerRef.transform.position - transform.position).magnitude * speed * Time.fixedDeltaTime;
            Vector2 direction = (playerRef.transform.position - transform.position).normalized * speed * Time.fixedDeltaTime;
            playerRb.MovePosition(playerRb.position + direction);

        }

    }
}
