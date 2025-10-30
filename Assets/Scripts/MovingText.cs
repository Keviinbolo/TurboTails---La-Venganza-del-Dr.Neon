using UnityEngine;

public class MovingText : MonoBehaviour
{
   [SerializeField] private float _Scrollspeed = 40f;


    
    void Update()
    {
        transform.Translate(Vector3.up * _Scrollspeed * Time.deltaTime);
       if (transform.position.z >= 1225)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu_Start");
        }


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu_Start");
        }
    }
}
