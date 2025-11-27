using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class ControladorJugador : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 5f;
    public float fuerzaSalto = 5f;
    public float fuerzaDobleSalto = 5f;

    [Header("Tilt (Inclinación Estética)")]
    public float inclinacionMax = 15f;     // cuánto se inclina
    public float suavizadoRot = 6f;        // velocidad de suavizado

    [Header("Ataque (Giro)")]
    public float duracionGiro = 0.25f;
    public float duracionAtaque = 0.4f;
    private bool atacando = false;

    private Rigidbody rb;
    private int saltosRestantes = 2;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

 void Update()
{
    float movimientoHorizontal = Input.GetAxis("Horizontal");
    float movimientoVertical = Input.GetAxis("Vertical");

    Vector3 direccion = new Vector3(movimientoHorizontal, 0f, movimientoVertical);

    if (!atacando)
    {
        // Movimiento
        Vector3 movimiento = transform.TransformDirection(direccion) * velocidad * Time.deltaTime;
        rb.MovePosition(rb.position + movimiento);

        // Giro hacia la dirección de movimiento si se está moviendo
        if(direccion.sqrMagnitude > 0.01f) // solo si hay movimiento
        {
            Quaternion rotObjetivo = Quaternion.LookRotation(direccion.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotObjetivo, Time.deltaTime * 6f);
        }
    }

    // Saltos
    if (Input.GetKeyDown(KeyCode.Space) && saltosRestantes > 0)
    {
        Vector3 v = rb.linearVelocity;
        v.y = 0f;
        rb.linearVelocity = v;

        float fuerza = (saltosRestantes == 2) ? fuerzaSalto : fuerzaDobleSalto;
        rb.AddForce(Vector3.up * fuerza, ForceMode.Impulse);
        saltosRestantes--;
    }

    // Ataque
    if (Input.GetKeyDown(KeyCode.E) && !atacando)
    {
        StartCoroutine(GiroDeAtaque());
    }
}


    void RotacionPersonaje(float horizontal, float vertical)
    {
        // Rotación estética solo si se está moviendo
        Quaternion rotObjetivo = Quaternion.Euler(
            vertical * inclinacionMax,
            0f,
            -horizontal * inclinacionMax
        );

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            rotObjetivo,
            Time.deltaTime * suavizadoRot
        );
    }

    private IEnumerator GiroDeAtaque()
    {
        atacando = true;

        Quaternion rotacionInicial = transform.rotation;
        Quaternion rotacionGirado = rotacionInicial * Quaternion.Euler(0f, 180f, 0f);

        // --- Giro hacia atrás (ataque) ---
        float t = 0f;
        while (t < duracionGiro)
        {
            t += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(rotacionInicial, rotacionGirado, t / duracionGiro);
            yield return null;
        }

        transform.rotation = rotacionGirado;
        Debug.Log("Zorro giró y atacó con la cola");

        yield return new WaitForSeconds(duracionAtaque);

        // --- Volver ---
        t = 0f;
        while (t < duracionGiro)
        {
            t += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(rotacionGirado, rotacionInicial, t / duracionGiro);
            yield return null;
        }

        transform.rotation = rotacionInicial;
        atacando = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Suelo"))
            saltosRestantes = 2;

        if (collision.gameObject.CompareTag("Enemigo"))
            SceneManager.LoadScene("InGame_Dead");

        if (collision.gameObject.CompareTag("Subsuelo"))
            SceneManager.LoadScene("InGame_Dead");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Prisma"))
            SceneManager.LoadScene("Fabric_InGame");

        if (other.CompareTag("PrismaEnd"))
            SceneManager.LoadScene("InGame_End");
    }
}
