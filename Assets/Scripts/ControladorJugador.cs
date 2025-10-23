using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ControladorJugador : MonoBehaviour
{
    public float velocidad = 5f;      // Velocidad de movimiento
    public float fuerzaSalto = 5f;    // Fuerza del salto
    private Rigidbody rb;

    private bool enSuelo = true;       // Para saber si puede saltar

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Movimiento horizontal y vertical
        float movimientoHorizontal = Input.GetAxis("Horizontal"); // A/D o flechas
        float movimientoVertical = Input.GetAxis("Vertical");     // W/S o flechas

        Vector3 direccion = new Vector3(movimientoHorizontal, 0f, movimientoVertical);
        direccion = transform.TransformDirection(direccion); // Para moverse según la orientación del jugador

        Vector3 movimiento = direccion * velocidad * Time.deltaTime;
        rb.MovePosition(rb.position + movimiento);

        // Salto
        if (Input.GetKeyDown(KeyCode.Space) && enSuelo)
        {
            rb.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);
            enSuelo = false; // Evita saltar en el aire
        }
    }

    // Detectar si está en el suelo
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Suelo"))
        {
            enSuelo = true;
        }
    }
}
