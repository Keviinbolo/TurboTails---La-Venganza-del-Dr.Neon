using UnityEngine;

public class MovimientoTurboTails : MonoBehaviour
{
    public float velocidad = 10f;

    private Vector3 limiteInferior;
    private Vector3 limiteSuperior;
    private Camera camara;

    void Start()
    {
        camara = Camera.main;

        // Calculamos los límites de la pantalla según la cámara
        float distanciaZ = Mathf.Abs(transform.position.z - camara.transform.position.z);
        limiteInferior = camara.ViewportToWorldPoint(new Vector3(0, 0, distanciaZ));
        limiteSuperior = camara.ViewportToWorldPoint(new Vector3(1, 0, distanciaZ)); // solo X-Z
    }

    void Update()
    {
        Mover();
        ControlLimitesPantalla();
    }

    void Mover()
    {
        // Movimiento solo horizontal (X) y profundidad (Z)
        float dirHorizontal = Input.GetAxisRaw("Horizontal");
        float dirProfundidad = Input.GetAxisRaw("Vertical"); // adelante/atrás

        Vector3 direccion = new Vector3(dirHorizontal, 0, dirProfundidad).normalized;

        Vector3 desplazamiento = direccion * velocidad * Time.deltaTime;

        transform.position += desplazamiento;
    }

    void ControlLimitesPantalla()
    {
        Vector3 nuevaPos = transform.position;

        // Limitamos solo X y Z
        nuevaPos.x = Mathf.Clamp(nuevaPos.x, limiteInferior.x, limiteSuperior.x);
        nuevaPos.z = Mathf.Clamp(nuevaPos.z, limiteInferior.z, limiteSuperior.z);

        transform.position = nuevaPos;
    }
}
