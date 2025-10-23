using UnityEngine;
using UnityEngine.SceneManagement;
public class ChangeScene : MonoBehaviour
{
  public void MoveToNeonLandInGame()
  {
    UnityEngine.SceneManagement.SceneManager.LoadScene("NeonLand_InGame");
    }
}
