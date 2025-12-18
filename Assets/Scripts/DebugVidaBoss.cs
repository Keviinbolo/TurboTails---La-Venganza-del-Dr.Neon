using UnityEngine;

public class DebugVidaBoss : MonoBehaviour
{
    private void Start()
    {
        // Buscar todos los Boss en escena
        BossFinal[] bosses = FindObjectsOfType<BossFinal>();
        
        Debug.Log($"=== DEBUG VIDA BOSS ===");
        Debug.Log($"Número de bosses encontrados: {bosses.Length}");
        
        foreach (BossFinal boss in bosses)
        {
            Debug.Log($"Boss: {boss.gameObject.name}");
            Debug.Log($"- Tag: {boss.gameObject.tag}");
            Debug.Log($"- Tiene componente BossFinal: {boss != null}");
            
            // Verificar vida mediante reflexión (si no hay método público)
            System.Type type = boss.GetType();
            var vidaField = type.GetField("vidaActual", 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance);
            
            if (vidaField != null)
            {
                int vida = (int)vidaField.GetValue(boss);
                Debug.Log($"- Vida actual (por reflexión): {vida}");
            }
            
            var vidaMaxField = type.GetField("vidaMaxima", 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance);
            
            if (vidaMaxField != null)
            {
                int vidaMax = (int)vidaMaxField.GetValue(boss);
                Debug.Log($"- Vida máxima (por reflexión): {vidaMax}");
            }
        }
    }
}