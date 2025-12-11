using UnityEngine;

public class Enemigo : MonoBehaviour
{
    [SerializeField] private GameObject jaula;
    
  private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Cola"))
        {
            Debug.Log($"Enemigo normal {gameObject.name} derrotado");
            
            // Solo se destruye a sí mismo
            Destroy(gameObject);
        }
    }
}