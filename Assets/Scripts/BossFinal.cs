using UnityEngine;

public class BossFinal : MonoBehaviour
{
    [Header("Configuración del Jefe")]
    [SerializeField] private GameObject objetoJaula;
    
    private void Start()
    {
        if (objetoJaula == null)
        {
            objetoJaula = GameObject.FindGameObjectWithTag("Jaula");
            
            if (objetoJaula == null)
            {
                Debug.LogError("Jefe: No se encontró la jaula con tag 'Jaula'");
            }
            else
            {
                Debug.Log($"Jefe: Jaula encontrada - {objetoJaula.name}");
            }
        }
    }
    
    // CAMBIA A OnTriggerEnter (igual que ColaDañina)
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger con: {other.gameObject.name} - Tag: {other.gameObject.tag}");
        
        if (other.CompareTag("Cola"))
        {
            Debug.Log("¡JEFE DERROTADO! Liberando prisioneros...");
            
            // Destruir la jaula
            if (objetoJaula != null)
            {
                Destroy(objetoJaula);
                Debug.Log("Jaula destruida - Prisioneros liberados!");
            }
            
            // Destruir al jefe
            Destroy(gameObject);
        }
    }
}