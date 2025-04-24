using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class AbsorbableObject : NetworkBehaviour
{
    public float mass;
    public bool isBacteria;
    [SerializeField] private bool canAbsorb;
    public float superficialDensity = 100;
    public void Start()
    {
        float newDiameter = Mathf.Sqrt(mass / (Mathf.PI * superficialDensity)) * 2;
        transform.localScale = new Vector3(newDiameter, newDiameter, newDiameter);
    }
    void Update()
    {
    }


    public void UpdateDiameter(Transform transform, float mass){
        float newDiameter = Mathf.Sqrt(mass / (Mathf.PI * superficialDensity)) * 2;
        transform.localScale = new Vector3(newDiameter, newDiameter, newDiameter);
    }

}
