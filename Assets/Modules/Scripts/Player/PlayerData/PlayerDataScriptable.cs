using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "NewPlayerData", menuName = "Player/PlayerData", order = 0)]
public class PlayerDataScriptable : ScriptableObject
{
    public string playerName;
    public Sprite playerImage;
    public Color playerBaseColor;


}
