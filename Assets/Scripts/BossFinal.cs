using UnityEngine;
using System.Collections;

public class BossFinal : MonoBehaviour
{
    [Header("Configuración del Jefe")]
    [SerializeField] private GameObject objetoJaula;
    
    [Header("Sistema de Vida - 6 VIDAS")]
    [SerializeField] private int vidaMaxima = 6;
    [SerializeField] private int vidaActual = 6;
    
    [Header("Efectos Visuales")]
    [SerializeField] private Color colorGolpe = Color.red;
    [SerializeField] private float tiempoColorGolpe = 0.1f;
    
    private Renderer rendererBoss;
    private Color colorOriginal;
    private LifeBarContinua barraVida; // NUEVA REFERENCIA
    
    private void Start()
    {
        // Inicializar vida
        vidaActual = vidaMaxima;
        
        Debug.Log($"BOSS: Vida inicializada - {vidaActual}/{vidaMaxima} (6 vidas total)");
        
        // Obtener renderer para efectos visuales
        rendererBoss = GetComponent<Renderer>();
        if (rendererBoss != null)
        {
            colorOriginal = rendererBoss.material.color;
        }
        else
        {
            Debug.LogWarning("BOSS: No se encontró Renderer para efectos visuales");
        }
        
        // Buscar LifeBarContinua en hijos (NOMBRE CORREGIDO)
        barraVida = GetComponentInChildren<LifeBarContinua>();
        if (barraVida != null)
        {
            Debug.Log("Barra de vida continua encontrada en hijos");
            // Inicializar con 6 vidas llenas
            barraVida.ActualizarVida(6);
        }
        else
        {
            Debug.LogWarning("No se encontró LifeBarContinua en los hijos del boss");
        }
        
        // Buscar jaula por tag si no está asignada
        if (objetoJaula == null)
        {
            objetoJaula = GameObject.FindGameObjectWithTag("Jaula");
        }
        
        if (objetoJaula != null)
        {
            Debug.Log($"BOSS: Jaula asignada - {objetoJaula.name}");
        }
        else
        {
            Debug.LogWarning("BOSS: No se encontró jaula con tag 'Jaula'");
        }
        
        // Asegurar que tiene tag "Enemigo"
        if (!gameObject.CompareTag("Enemigo"))
        {
            gameObject.tag = "Enemigo";
            Debug.Log($"BOSS: Tag 'Enemigo' asignado a {gameObject.name}");
        }
    }
    
    // Método para recibir daño (llamado por la cola)
    public void RecibirDaño(int cantidad = 1)
    {
        if (vidaActual <= 0)
        {
            Debug.Log("BOSS: Ya está muerto, ignorando daño");
            return;
        }
        
        vidaActual -= cantidad;
        Debug.Log($"¡BOSS golpeado! Vida: {vidaActual}/{vidaMaxima} (queda {vidaActual} golpes)");
        
        // Actualizar LifeBar (NOMBRE CORREGIDO)
        if (barraVida != null)
        {
            barraVida.ActualizarVida(vidaActual);
        }
        
        // Efecto visual de golpe
        StartCoroutine(EfectoGolpeVisual());
        
        // Verificar si murió
        if (vidaActual <= 0)
        {
            Morir();
        }
    }
    
    // Efecto visual al recibir daño
    private IEnumerator EfectoGolpeVisual()
    {
        if (rendererBoss != null)
        {
            // Cambiar a color rojo
            rendererBoss.material.color = colorGolpe;
            
            // Esperar
            yield return new WaitForSeconds(tiempoColorGolpe);
            
            // Volver al color original
            rendererBoss.material.color = colorOriginal;
        }
    }
    
    // Muerte del boss
    private void Morir()
    {
        Debug.Log("¡JEFE DERROTADO después de 6 golpes! Liberando prisioneros...");
        
        // Ocultar barra de vida (NOMBRE CORREGIDO)
        if (barraVida != null)
        {
            barraVida.OcultarBarra();
        }
        
        // Destruir la jaula
        if (objetoJaula != null)
        {
            Destroy(objetoJaula);
            Debug.Log("Jaula destruida - Prisioneros liberados!");
        }
        else
        {
            Debug.LogWarning("BOSS: No hay jaula para destruir");
        }
        
        // Esperar un poco para que se vea la animación de la barra
        StartCoroutine(EsperarYDestruir());
    }
    
    private IEnumerator EsperarYDestruir()
    {
        yield return new WaitForSeconds(0.5f);
        // Destruir al boss
        Destroy(gameObject);
    }
    
    // También puede recibir daño por trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cola"))
        {
            Debug.Log($"BOSS: Golpeado por cola - {other.gameObject.name}");
            RecibirDaño();
        }
    }
}