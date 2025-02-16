using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private MapDataSettings mapDataSettings;
    [SerializeField] private bool debugQTree;
    [SerializeField] private GameObject playerPref;
    
    private Quadtree _quadtree;
    
    private void Start()
    {
        Setup();
    }
    public void Setup()
    {
        Rectangle bounds = new Rectangle(0, 0, mapDataSettings.boardScale, mapDataSettings.boardScale);
        _quadtree = new Quadtree(bounds, mapDataSettings.maxCapacityByChunk);
        for (int a = 0; a < 20; a++)
        {
            float x = Random.Range(-mapDataSettings.boardScale * .5f, mapDataSettings.boardScale * .5f);
            float y = Random.Range(-mapDataSettings.boardScale * .5f, mapDataSettings.boardScale * .5f);
     
            Point point = new Point(x, y, null);
            InsertPlayer(point, null);
        }
    }

    public void InsertPlayer(Point point, GameObject data)
    {
        _quadtree.Insert(point);
        GameObject newPlayer = Instantiate(playerPref, new Vector2(point.X, point.Y), quaternion.identity);
    }
    
    void OnDrawGizmos()
    {
        if (_quadtree == null) return;
        if (!debugQTree) return;

        DrawQuadtree(_quadtree);
    }

    void DrawQuadtree(Quadtree tree)
    {
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
