using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public PlayerDataScriptable playerData;
    public Image image;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image.sprite = playerData.playerImage;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeProfilePicture(Image sprite){
        playerData.playerImage = sprite.sprite;
        image.sprite = sprite.sprite;
    }
    public void OnQuit(){
        Destroy(NetworkManager.Singleton.gameObject);
        SceneManager.LoadScene(0);
    }
}
