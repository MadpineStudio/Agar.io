using System.Diagnostics;
using TMPro;
using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private MapDataSettings mapDataSettings;
    [SerializeField] private bool debugQTree;
    [SerializeField] private GameObject playerPref;
    [SerializeField] private GameObject CameraPref;
    [SerializeField] PlayerDataScriptable playerData;
    [SerializeField] private GameObject Nick;
    


    public Quadtree quadtree;
    public static PlayerManager instance {get; private set;}
    void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        Setup();
    }
    public void Setup()
    {
        Rectangle bounds = new Rectangle(0, 0, mapDataSettings.boardScale, mapDataSettings.boardScale);
        quadtree = new Quadtree(bounds, mapDataSettings.maxCapacityByChunk);
        SpawnPlayer();
    }

    public void InsertPlayer(float x, float y, GameObject pointObject)
    {
        Point point = new Point(x, y, pointObject);
        quadtree.Insert(point);
    }

    void OnDrawGizmos()
    {
        if (quadtree == null) return;
        if (!debugQTree) return;

        DrawQuadtree(quadtree);
    }

    void DrawQuadtree(Quadtree tree)
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(new Vector3(tree.boundary.X, tree.boundary.Y, 0),
            new Vector3(tree.boundary.W * 2, tree.boundary.H * 2, 0));

        Gizmos.color = Color.green;
        foreach (Point p in tree.points)
        {
            Gizmos.DrawSphere(new Vector3(p.X, p.Y, 0), 2f);
        }
        if (tree.divided)
        {
            if (tree.northeast != null) DrawQuadtree(tree.northeast);
            if (tree.northwest != null) DrawQuadtree(tree.northwest);
            if (tree.southeast != null) DrawQuadtree(tree.southeast);
            if (tree.southwest != null) DrawQuadtree(tree.southwest);
        }
    }
    public void SpawnPlayer()
    {
        Vector2 pos = new Vector2(Random.Range(-mapDataSettings.boardScale * .5f, mapDataSettings.boardScale * .5f), Random.Range(-mapDataSettings.boardScale * .5f, mapDataSettings.boardScale * .5f));
        
        GameObject playerSpawned = Instantiate(playerPref, pos, quaternion.identity);
        playerSpawned.GetComponent<PlayerBehaviour>().playerRef = playerSpawned;
        
        InsertPlayer(pos.x, pos.y, playerSpawned);

        GameObject newNickArea = Instantiate(Nick, playerSpawned.transform.position + new Vector3(0, 1.5f), playerSpawned.transform.rotation, playerSpawned.transform);
        newNickArea.GetComponent<TMP_Text>().text = playerData.playerName;
        GameObject upperPoint = new GameObject("UpperPivot");
        
        upperPoint.transform.SetParent(playerSpawned.transform);
        upperPoint.transform.position = playerSpawned.transform.position + new Vector3(0, 1.25f);

        GameObject lowerPoint = new GameObject("LowerPivot");
        
        lowerPoint.transform.SetParent(playerSpawned.transform);
        lowerPoint.transform.position = playerSpawned.transform.position + new Vector3(0, -1.25f);


        CinemachineTargetGroup targetGroup = playerSpawned.transform.GetChild(0).transform.GetComponent<CinemachineTargetGroup>();
        
        targetGroup.AddMember(playerSpawned.transform, 1, 7);
        targetGroup.AddMember(upperPoint.transform, 1, 7);
        targetGroup.AddMember(lowerPoint.transform, 1, 7);

        CinemachineVirtualCameraBase camera = Instantiate(CameraPref, Vector3.zero, quaternion.identity).GetComponent<CinemachineCamera>();

        camera.Follow = targetGroup.transform;
    }

    
}
