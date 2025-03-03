using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(QTreeEntryPoint))]
public class MapManager : MonoBehaviour
{
    [SerializeField] private bool spawnerActivated;
    [SerializeField] private GameObject dormantMassBall;
    [SerializeField] private GameObject bacteriaDivider;
    [SerializeField] private List<GameObject> massBalls = new();
    private List<GameObject> bacterias = new();
    [SerializeField] private GameObject playerPref;
    [SerializeField] private GameObject playerManager;
    [SerializeField] private PlayerDataScriptable playerData;
    [SerializeField] private TMP_InputField nickArea;
    public List<EnemyBehaviour> enemies = new();

    [Header("Map Settings")]

    [SerializeField] private MapDataSettings mapDataSettings;

    void Start()
    {
        nickArea.text = playerData.playerName;
         Debug.Log("Listando NetworkPrefabs registrados:");
        foreach (var prefab in NetworkManager.Singleton.NetworkConfig.Prefabs.Prefabs)
        {
            Debug.Log($"Prefab: {prefab.Prefab.name}, Hash: {prefab.GetHashCode()}");
        }
    }
    public void StartGame(bool isClient)
    {
        playerData.playerName = nickArea.text;
        QTreeEntryPoint.instance.SetupQTree(mapDataSettings.maxCapacityByChunk, Mathf.CeilToInt(mapDataSettings.boardScale * .5f));
        spawnerActivated = true;
        SpawnStartInertMassCells(isClient);
        StartCoroutine(SpawnInertMass(.1f));
        StartCoroutine(BacteriaSpawner(1));

    }
    public void SpawnStartInertMassCells(bool isClient)
    {
        for (int a = 0; a < mapDataSettings.startAbsorbablesCount; a++)
        {
            Vector2 pos = new Vector2(Random.Range(-mapDataSettings.boardScale * .5f, mapDataSettings.boardScale * .5f), Random.Range(
                -mapDataSettings.boardScale * .5f, mapDataSettings.boardScale * .5f));

            GameObject newDormantBall = Instantiate(dormantMassBall, pos, quaternion.identity, transform);
            QTreeEntryPoint.instance.Insert(pos.x, pos.y, newDormantBall);
        }
        // playerManager.GetComponent<PlayerManager>().isClient = isClient;
        playerManager.SetActive(true);
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
                GameObject newDormantMassBall = Instantiate(dormantMassBall, pos, transform.rotation, transform);
                QTreeEntryPoint.instance.Insert(pos.x, pos.y, newDormantMassBall);
            }
            yield return new WaitForSeconds(delay);
        }
    }
    private IEnumerator BacteriaSpawner(float delay)
    {
        int maxSpawnCapacity = 5;
        while (spawnerActivated)
        {
            bacterias.RemoveAll(obj => obj == null);
            if (maxSpawnCapacity > bacterias.Count)
            {
                Vector2 pos = new Vector2(Random.Range(-mapDataSettings.boardScale * .5f, mapDataSettings.boardScale * .5f), Random.Range(
           -mapDataSettings.boardScale * .5f, mapDataSettings.boardScale * .5f));
                GameObject newBacteria = Instantiate(bacteriaDivider, pos, transform.rotation, transform);
                QTreeEntryPoint.instance.Insert(pos.x, pos.y, newBacteria);
                bacterias.Add(newBacteria);
            }
            yield return new WaitForSeconds(delay);
        }
    }
}
