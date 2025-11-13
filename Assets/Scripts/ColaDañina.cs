using UnityEngine;

public class ColaDañina : MonoBehaviour
{
    /*private void OnTriggerEnter(Collision collision)
{
    if (collision.gameObject.CompareTag("Enemigo"))
        Destroy(collision.gameObject);
}*/
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemigo"))
        {
            Destroy(other.gameObject);
        }
    }
}
