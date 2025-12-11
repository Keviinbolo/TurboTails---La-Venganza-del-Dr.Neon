using UnityEngine;

public class ColaDañina : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private bool siempreActiva = true;
    
    private void Start()
    {
        // Asegurar que es trigger
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
        
        // Asignar tag
        gameObject.tag = "Cola";
        
        Debug.Log("Cola configurada - SIEMPRE ACTIVA");
    }
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Cola golpeó: {other.gameObject.name}, Tag: {other.gameObject.tag}");
        
        if (other.CompareTag("Enemigo"))
        {
            Debug.Log($"¡Enemigo {other.gameObject.name} destruido!");
            Destroy(other.gameObject);
        }
        
        if (other.CompareTag("Boss"))
        {
            Debug.Log("¡Golpe al Jefe!");
            // El boss maneja su propia lógica
        }
    }
}