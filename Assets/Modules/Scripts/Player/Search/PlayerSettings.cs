using UnityEngine;

[CreateAssetMenu(menuName = "IO/PlayerSettings")]
public class PlayerSettings : ScriptableObject
{
    [Header("Player Settings Debug")]
    public bool debug;
    public int searchAreaWidth, searchAreaHeight;
    public Color foundNearColor;
    public float foundNearGizmoSize;
}
