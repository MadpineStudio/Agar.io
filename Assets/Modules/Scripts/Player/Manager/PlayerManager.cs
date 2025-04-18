using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerManager : NetworkBehaviour
{
    [SerializeField] private MapDataSettings mapDataSettings;
    [SerializeField] private bool debugQTree;
    [SerializeField] private GameObject playerPref;
    [SerializeField] private GameObject CameraPref;
    [SerializeField] PlayerDataScriptable playerData;
    [SerializeField] private GameObject Nick;
    private Queue<IEnumerator> _insertMassCorroutines = new();    
    private Coroutine _InsertPlayerMassCorroutine;
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
        float scale = Mathf.CeilToInt(mapDataSettings.boardScale * .5f);
        Rectangle bounds = new Rectangle(0, 0, scale, scale);
        quadtree = new Quadtree(bounds, mapDataSettings.maxCapacityByChunk);
        SpawnPlayer();
    }

    public Point InsertPlayer(float x, float y, GameObject pointObject)
    {
        Point point = new Point(x, y, pointObject);
        PlayerBehaviour playerBH = pointObject.GetComponent<PlayerBehaviour>();
        if(playerBH != null) playerBH.AddInitialPoint(point);
        quadtree.Insert(point);

    
        return point;
    }
    private IEnumerator InsertPlayerMass(Point point){
        quadtree.Insert(point);
        yield return new WaitForEndOfFrame();
        if(_insertMassCorroutines.Count > 0) _InsertPlayerMassCorroutine = StartCoroutine(_insertMassCorroutines.Dequeue());
        else _InsertPlayerMassCorroutine = null;
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
        playerSpawned.GetComponent<SpriteRenderer>().material.SetTexture("_MainTex0", playerData.playerImage.texture);

        GameObject newNickArea = Instantiate(Nick, playerSpawned.transform.position , playerSpawned.transform.rotation, playerSpawned.transform);
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
