using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] private bool spawnerActivated;
    [SerializeField] private GameObject dormantMassBall;
    [SerializeField] private GameObject bacteriaDivider;
    [SerializeField] private List<GameObject> massBalls = new();
    [SerializeField] private List<GameObject> bacteria = new();
    public List<EnemyBehaviour> enemies = new();

    void Start()
    {
        spawnerActivated = true;
        StartCoroutine(Spawner(.1f));
        StartCoroutine(BacteriaSpawner(1));

    }

    void Update()
    {

    }
    public void ReactivateSpawner()
    {
        spawnerActivated = true;
        StartCoroutine(Spawner(.1f));
        StartCoroutine(BacteriaSpawner(1));
    }
    private IEnumerator Spawner(float delay)
    {
        while (spawnerActivated)
        {
            massBalls.RemoveAll(obj => obj == null);
            if (massBalls.Count < 500)
            {
                massBalls.Add(Instantiate(dormantMassBall, new Vector3(Random.Range(-48f, 49f), Random.Range(-48f, 49f), 0), transform.rotation));
                
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
