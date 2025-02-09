using UnityEngine;

public static class SoftBodyCollisionHandler
{
    public static void ResolveCollision(SoftBodyNode node, Vector2 bodyPosition, float nodeRadius, LayerMask collisionLayers)
    {
        Vector2 worldPos = bodyPosition + node.Position;
        Collider2D col = Physics2D.OverlapCircle(worldPos, nodeRadius, collisionLayers);
        if (col != null)
        {
            Vector2 closest = col.ClosestPoint(worldPos);
            Vector2 direction = (worldPos - closest).normalized;
            float penetration = nodeRadius - Vector2.Distance(worldPos, closest);
            if (penetration > 0)
                node.Position += direction * penetration;
        }
    }
}