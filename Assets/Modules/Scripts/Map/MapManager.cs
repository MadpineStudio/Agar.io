using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(QTreeEntryPoint))]
public class MapManager : MonoBehaviour
{
    [SerializeField] private bool spawnerActivated;
    [SerializeField] private GameObject dormantMassBall;
    [SerializeField] private GameObject bacteriaDivider;
    [SerializeField] private List<GameObject> massBalls = new();
    [SerializeField] private List<GameObject> bacteria = new();
    public List<EnemyBehaviour> enemies = new();

    [Header("Map Settings")]
    [SerializeField] private CinemachineVirtualCameraBase playerCamera;
    [SerializeField] private GameObject playerPref;
    [SerializeField] private GameObject CameraPref;
    [SerializeField] private MapDataSettings mapDataSettings;

    void Start()
    {
        QTreeEntryPoint.instance.SetupQTree(mapDataSettings.maxCapacityByChunk, Mathf.CeilToInt(mapDataSettings.boardScale * .5f));


        spawnerActivated = true;
        SpawnStartInertMassCells();
        SpawnPlayer();
        StartCoroutine(SpawnInertMass(.1f));
        // StartCoroutine(BacteriaSpawner(1));
    }

    public void SpawnStartInertMassCells()
    {
        for (int a = 0; a < mapDataSettings.startAbsorbablesCount; a++)
        {
            Vector2 pos = new Vector2(Random.Range(-mapDataSettings.boardScale * .5f, mapDataSettings.boardScale * .5f), Random.Range(
                -mapDataSettings.boardScale * .5f, mapDataSettings.boardScale * .5f));

            GameObject newDormantBall = Instantiate(dormantMassBall, pos, quaternion.identity, transform);
            QTreeEntryPoint.instance.Insert(pos.x, pos.y, newDormantBall);
        }
    }

    public void SpawnPlayer()
    {
        Vector2 pos = new Vector2(Random.Range(-mapDataSettings.boardScale * .5f, mapDataSettings.boardScale * .5f), Random.Range(
            -mapDataSettings.boardScale * .5f, mapDataSettings.boardScale * .5f));

        GameObject playerSpawned = Instantiate(playerPref, pos, quaternion.identity);


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
        // QTreeEntryPoint.instance.Insert(pos.x, pos.y, playerSpawned);

        CinemachineVirtualCameraBase camera = Instantiate(CameraPref, Vector3.zero, quaternion.identity)
            .GetComponent<CinemachineCamera>();

        camera.Follow = targetGroup.transform;
    }
    public void ReactivateSpawner()
    {
        // spawnerActivated = true;
        // StartCoroutine(SpawnInertMass(.1f));
        // StartCoroutine(BacteriaSpawner(1));
    }
    private IEnumerator SpawnInertMass(float delay)
    {
        int maxSpawnCapacity = 500;

        while (spawnerActivated)
        {
            massBalls.RemoveAll(obj => obj == null);
            if (maxSpawnCapacity > QTreeEntryPoint.instance.CountPoints())
            {
                Vector2 pos = new Vector2(Random.Range(-mapDataSettings.boardScale * .5f, mapDataSettings.boardScale * .5f), Random.Range(
           -mapDataSettings.boardScale * .5f, mapDataSettings.boardScale * .5f));
                // massBalls.Add(Instantiate(dormantMassBall, new Vector3(Random.Range(-48f, 49f), Random.Range(-48f, 49f), 0), transform.rotation));
                GameObject newDormantMassBall = Instantiate(dormantMassBall, pos, transform.rotation, transform);
                QTreeEntryPoint.instance.Insert(pos.x, pos.y, newDormantMassBall);
                // enemies[0].absorbables.AddAll(massBalls.Select(obj => obj.transform).ToList());
            }
            yield return new WaitForSeconds(delay);
        }
    }
    private IEnumerator BacteriaSpawner(float delay)
    {
        while (spawnerActivated)
        {
            bacteria.RemoveAll(obj => obj == null);
            if (bacteria.Count < 15)
            {
                bacteria.Add(Instantiate(bacteriaDivider, new Vector3(Random.Range(-48f, 49f), Random.Range(-48f, 49f), 0), transform.rotation));
            }
            yield return new WaitForSeconds(delay);
        }
    }
}
