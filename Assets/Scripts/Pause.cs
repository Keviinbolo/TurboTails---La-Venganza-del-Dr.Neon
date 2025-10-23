using UnityEngine;

public class Pausa : MonoBehaviour
{
    bool gamePaused = false;
    public GameObject menuPausaUI;
    void Start()
    {
        menuPausaUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

    }
    public void Resume()
    {
        menuPausaUI.SetActive(false);
        Time.timeScale = 1f;
        gamePaused = false;
    }
    public void Pause()
    {
        gamePaused = true;
        menuPausaUI.SetActive(true);
        Time.timeScale = 0f;

    }
    public void BacktoGame()
    {
        Resume();
    }
}
