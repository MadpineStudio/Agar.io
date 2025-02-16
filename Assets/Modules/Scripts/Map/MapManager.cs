using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] private GameObject playerPref;
    [SerializeField] private GameObject playerManager;
    [SerializeField] private PlayerDataScriptable playerData;
    [SerializeField] private TextMeshProUGUI nickArea;
    public List<EnemyBehaviour> enemies = new();

    [Header("Map Settings")]
    
    [SerializeField] private MapDataSettings mapDataSettings;

    void Start()
    {
        nickArea.SetText(playerData.playerName);
    }
    public void StartGame(){
        playerData.playerName = nickArea.text;
        QTreeEntryPoint.instance.SetupQTree(mapDataSettings.maxCapacityByChunk, Mathf.CeilToInt(mapDataSettings.boardScale * .5f));
        spawnerActivated = true;
        SpawnStartInertMassCells();
        StartCoroutine(SpawnInertMass(.1f));
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
