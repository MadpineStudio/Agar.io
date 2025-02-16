using UnityEngine;

public class EnemyBehaviour : PlayerBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // absorbables.RemoveAll(obj => obj == null);
        // absorbables.UpdatePositions();
        // Transform nearest = absorbables.FindClosest(transform.position);
        // Vector2 direction = (nearest.position - transform.position) / (nearest.position - transform.position).magnitude;
        // transform.position = Vector2.MoveTowards(transform.position, direction, 2 * Time.fixedDeltaTime);

    }
}
