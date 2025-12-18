using UnityEngine;

public class ColaDañina : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private int dañoPorGolpe = 1;
    
    private void Start()
    {
        // Configurar como trigger
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.isTrigger = true;
            Debug.Log($"COLA: Configurada como trigger");
        }
        else
        {
            Debug.LogError("COLA: No tiene Collider!");
        }
        
        // Asegurar tag
        if (string.IsNullOrEmpty(gameObject.tag))
        {
            gameObject.tag = "Cola";
            Debug.Log($"COLA: Tag 'Cola' asignado");
        }
        else
        {
            Debug.Log($"COLA: Tag actual - {gameObject.tag}");
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"COLA: Trigger con {other.gameObject.name}, Tag: {other.gameObject.tag}");
        
        // PRIMERO: Verificar si es BOSS (por componente BossFinal)
        BossFinal boss = other.GetComponent<BossFinal>();
        if (boss != null)
        {
            Debug.Log($"¡BOSS detectado por componente: {other.gameObject.name}!");
            boss.RecibirDaño(dañoPorGolpe);
            return; // Salir, ya procesamos el boss
        }
        
        // SEGUNDO: Enemigos normales (tag "Enemigo")
        if (other.CompareTag("Enemigo"))
        {
            Debug.Log($"¡Enemigo normal eliminado: {other.gameObject.name}!");
            Destroy(other.gameObject);
        }
        else
        {
            Debug.Log($"Objeto no procesado: {other.gameObject.name}, Tag: {other.gameObject.tag}");
        }
    }
}