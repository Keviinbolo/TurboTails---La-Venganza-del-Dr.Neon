using UnityEngine;

public class Enemigo : MonoBehaviour
{
private void OnCollisionEnter(Collision collision)
{
    if (collision.gameObject.CompareTag("Cola"))
        Destroy(gameObject);
}
}
