using UnityEngine;

public class BossFinal : MonoBehaviour
{
    [Header("Configuración del Jefe")]
    [SerializeField] private GameObject objetoJaula;
    
    [Header("Sistema de Vida - 3 VIDAS")]
    [SerializeField] private int vidaMaxima = 6;
    [SerializeField] private int vidaActual = 6; // ← Siempre 3 al empezar
    
    [Header("Efectos Visuales")]
    [SerializeField] private Color colorGolpe = Color.red;
    [SerializeField] private float tiempoColorGolpe = 0.1f;
    
    private Renderer rendererBoss;
    private Color colorOriginal;
    
    // Se ejecuta ANTES de Start
    private void Awake()
    {
        // GARANTIZAR 3 VIDAS - Esto se ejecuta primero
        vidaMaxima = 6;
        vidaActual = 6;
        
        Debug.Log($"<color=green>BOSS CONFIGURADO:</color> {vidaActual}/{vidaMaxima} vidas garantizadas");
    }
    
    private void Start()
    {
        // Verificación adicional
        if (vidaActual != 6 || vidaMaxima != 6)
        {
            Debug.LogWarning($"<color=yellow>CORRECCIÓN AUTOMÁTICA:</color> Vida {vidaActual}/{vidaMaxima} -> 3/3");
            vidaMaxima = 6;
            vidaActual = 6;
        }
        
        Debug.Log($"<color=cyan>BOSS INICIALIZADO:</color> {gameObject.name} con {vidaActual} vidas");
        
        // Configurar renderer
        rendererBoss = GetComponent<Renderer>();
        if (rendererBoss != null)
        {
            colorOriginal = rendererBoss.material.color;
        }
        
        // Buscar jaula
        if (objetoJaula == null)
        {
            objetoJaula = GameObject.FindGameObjectWithTag("Jaula");
            if (objetoJaula != null)
            {
                Debug.Log($"Jaula encontrada: {objetoJaula.name}");
            }
        }
        
        // Asegurar tag
        if (!gameObject.CompareTag("Enemigo"))
        {
            gameObject.tag = "Enemigo";
        }
    }
    
    // Método para recibir daño
    public void RecibirDaño(int cantidad = 1)
    {
        // Verificar que tiene vida
        if (vidaActual <= 0)
        {
            Debug.Log("BOSS ya está muerto");
            return;
        }
        
        // Aplicar daño
        int vidaAnterior = vidaActual;
        vidaActual -= cantidad;
        
        Debug.Log($"<color=orange>BOSS DAÑADO:</color> {vidaAnterior} -> {vidaActual}/{vidaMaxima}");
        
        // Efecto visual
        if (rendererBoss != null)
        {
            StartCoroutine(EfectoGolpeVisual());
        }
        
        // Verificar muerte
        if (vidaActual <= 0)
        {
            Morir();
        }
    }
    
    // Efecto visual de golpe
    private System.Collections.IEnumerator EfectoGolpeVisual()
    {
        if (rendererBoss != null)
        {
            rendererBoss.material.color = colorGolpe;
            yield return new WaitForSeconds(tiempoColorGolpe);
            rendererBoss.material.color = colorOriginal;
        }
    }
    
    // Muerte del boss
    private void Morir()
    {
        Debug.Log($"<color=red>¡BOSS DERROTADO!</color> después de {vidaMaxima} golpes");
        
        // Destruir jaula
        if (objetoJaula != null)
        {
            Destroy(objetoJaula);
            Debug.Log("Jaula destruida - Prisioneros liberados!");
        }
        
        // Destruir boss
        Destroy(gameObject);
    }
    
    // Detectar colisión con cola
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cola"))
        {
            RecibirDaño();
        }
    }
    
    // Métodos para verificar vida
    public int GetVidaActual() { return vidaActual; }
    public int GetVidaMaxima() { return vidaMaxima; }
}