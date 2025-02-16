using UnityEngine;

public class QTreeEntryPoint : MonoBehaviour
{
    public Quadtree QuadTree;
    
    public static QTreeEntryPoint instance { get; private set; }
    public bool debugQTree;
    private void Awake()
    {
        instance = this;
    }

    public void SetupQTree(int capacity, int boardScale)
    {
        var position = transform.position;
        Rectangle boundary = new Rectangle(position.x, position.y, boardScale, boardScale);
        QuadTree = new Quadtree(boundary, capacity);
    }
    public void Insert(float x, float y, GameObject poindData)
    {
        Point point = new Point(x, y, poindData);
        QuadTree.Insert(point);
    }
    public int CountPoints(){
        return QuadTree.CountPoints()
;    }
    void OnDrawGizmos()
    {
        if (QuadTree == null) return;
        if (!debugQTree) return;

        DrawQuadtree(QuadTree);
    }

    void DrawQuadtree(Quadtree tree)
    {
        if (tree == null) return; // Verifica se a árvore é nula

        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(new Vector3(tree.boundary.X, tree.boundary.Y, 0),
            new Vector3(tree.boundary.W * 2, tree.boundary.H * 2, 0));

        Gizmos.color = Color.green;
        foreach (Point p in tree.points)
        {
            Gizmos.DrawSphere(new Vector3(p.X, p.Y, 0), 0.02f);
        }

        if (tree.divided)
        {
            if (tree.northeast != null) DrawQuadtree(tree.northeast);
            if (tree.northwest != null) DrawQuadtree(tree.northwest);
            if (tree.southeast != null) DrawQuadtree(tree.southeast);
            if (tree.southwest != null) DrawQuadtree(tree.southwest);
        }
    }
}