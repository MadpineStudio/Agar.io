using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "IO/Map/MapSettings")]
public class MapDataSettings : ScriptableObject
{
    [Header("Default Map Properties")]
    public int boardScale;
    [FormerlySerializedAs("startCollectablesCount")] public int startAbsorbablesCount;
    
    [Header("QTree Structure")]
    public int maxCapacityByChunk;
}
