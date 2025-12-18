using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LifeBarContinua : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private Image barraFrontal;
    [SerializeField] private RectTransform rectBarra;
    
    [Header("Colores Dinámicos")]
    [SerializeField] private Color colorLleno = Color.green;
    [SerializeField] private Color colorMedio = Color.yellow;
    [SerializeField] private Color colorBajo = Color.red;
    
    [Header("Configuración")]
    [SerializeField] private bool seguirCamara = true;
    [SerializeField] private float suavizado = 8f;
    
    private Camera cam;
    private float anchoMaximo;
    private float anchoObjetivo;
    private Color colorObjetivo;
    
    private void Start()
    {
        cam = Camera.main;
        
        if (rectBarra != null)
        {
            anchoMaximo = rectBarra.sizeDelta.x;
            anchoObjetivo = anchoMaximo;
        }
        
        if (barraFrontal != null)
        {
            colorObjetivo = barraFrontal.color = colorLleno;
        }
        
        Debug.Log("✅ Barra continua inicializada");
    }
    
    private void Update()
    {
        // Seguir cámara
        if (seguirCamara && cam != null)
        {
            transform.LookAt(transform.position + cam.transform.forward);
        }
        
        // Suavizar cambios de tamaño
        if (rectBarra != null)
        {
            Vector2 size = rectBarra.sizeDelta;
            size.x = Mathf.Lerp(size.x, anchoObjetivo, Time.deltaTime * suavizado);
            rectBarra.sizeDelta = size;
        }
        
        // Suavizar cambios de color
        if (barraFrontal != null)
        {
            barraFrontal.color = Color.Lerp(barraFrontal.color, colorObjetivo, Time.deltaTime * suavizado);
        }
    }
    
    // Llamado por el Boss cuando recibe daño
    public void ActualizarVida(int vidaActual)
    {
        // Vida total = 6
        float porcentaje = vidaActual / 6f;
        
        // Calcular nuevo ancho
        anchoObjetivo = anchoMaximo * porcentaje;
        
        // Determinar color
        if (porcentaje > 0.66f) // 4-6 vidas
        {
            colorObjetivo = colorLleno;
        }
        else if (porcentaje > 0.33f) // 2-3 vidas
        {
            colorObjetivo = colorMedio;
        }
        else // 0-1 vida
        {
            colorObjetivo = colorBajo;
        }
        
        Debug.Log($"🔄 Barra: {vidaActual}/6 = {porcentaje*100}%");
    }
    
    // Cuando el boss muere
    public void OcultarBarra()
    {
        StartCoroutine(AnimacionMuerte());
    }
    
    private IEnumerator AnimacionMuerte()
    {
        // Reducir a 0
        anchoObjetivo = 0;
        
        // Esperar a que se reduzca
        yield return new WaitForSeconds(0.3f);
        
        // Desvanecer
        CanvasGroup grupo = GetComponent<CanvasGroup>();
        if (grupo == null) grupo = gameObject.AddComponent<CanvasGroup>();
        
        float tiempo = 0;
        float duracion = 0.3f;
        
        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            grupo.alpha = 1 - (tiempo / duracion);
            yield return null;
        }
        
        Destroy(gameObject);
    }
}