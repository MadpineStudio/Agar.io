using System.Collections;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] private bool spawnerActivated;
    [SerializeField] private GameObject dormantMassBall;
    void Start()
    {
        spawnerActivated = true;
        StartCoroutine(Spawner(.25f));
    }

    void Update()
    {
        
    }
    public void ReactivateSpawner(){
        spawnerActivated = true;
        StartCoroutine(Spawner(.33f));
    }
    private IEnumerator Spawner(float delay){
        while(spawnerActivated){
            Instantiate(dormantMassBall, new Vector3(Random.Range(-48f, 49f), Random.Range(-48f, 49f), 0), transform.rotation);
            // Instantiate(dormantMassBall, new Vector3(Random.Range(-5f, 6f), Random.Range(-5f, 6f), 0), transform.rotation);
        yield return new WaitForSeconds(delay);
        }
    }

}
