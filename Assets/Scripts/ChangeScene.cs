using UnityEngine;
using UnityEngine.SceneManagement;
public class ChangeScene : MonoBehaviour
{
    public void MoveToNeonLandInGame()
  {
    UnityEngine.SceneManagement.SceneManager.LoadScene("NeonLand_InGame");
    }

    public void MoveFromNeoLand_InGameToFabric_InGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Fabric_InGame");
    }
    public void MoveFromFabric_InGameToInGameEnd()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("InGame_End");
    }
    public void MoveFromInGameEndToMenu_Start()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu_Start");
    }
    public void MoveToMenu_InGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu_InGame");
    }
    public void MoveToHistoria()
    {
        
        UnityEngine.SceneManagement.SceneManager.LoadScene("Historia");
    }

}
