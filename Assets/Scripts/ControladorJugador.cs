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

    [Header("Ataque (Giro)")]
    public float duracionGiro = 0.25f;           // tiempo que tarda en girar
    public float duracionAtaque = 0.4f;          // cuánto tiempo mantiene la pose
    private bool atacando = false;

    private Rigidbody rb;
    private int saltosRestantes = 2;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // === Movimiento ===
        if (!atacando) // evita moverse mientras ataca, opcional
        {
            float movimientoHorizontal = Input.GetAxis("Horizontal");
            float movimientoVertical = Input.GetAxis("Vertical");

            Vector3 direccion = new Vector3(movimientoHorizontal, 0f, movimientoVertical);
            direccion = transform.TransformDirection(direccion);

            Vector3 movimiento = direccion * velocidad * Time.deltaTime;
            rb.MovePosition(rb.position + movimiento);
        }

        // === Saltos ===
        if (Input.GetKeyDown(KeyCode.Space) && saltosRestantes > 0)
        {
            Vector3 v = rb.linearVelocity;
            v.y = 0f;
            rb.linearVelocity = v;

            float fuerza = (saltosRestantes == 2) ? fuerzaSalto : fuerzaDobleSalto;
            rb.AddForce(Vector3.up * fuerza, ForceMode.Impulse);
            saltosRestantes--;
        }

        // === Ataque (giro ida y vuelta) ===
        if (Input.GetKeyDown(KeyCode.E) && !atacando)
        {
            StartCoroutine(GiroDeAtaque());
        }
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

        // --- Mantener posición atacando ---
        yield return new WaitForSeconds(duracionAtaque);

        // --- Volver a posición original ---
        t = 0f;
        while (t < duracionGiro)
        {
            t += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(rotacionGirado, rotacionInicial, t / duracionGiro);
            yield return null;
        }

        transform.rotation = rotacionInicial;

        Debug.Log("Zorro volvió a su posición inicial");

        atacando = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Recuperar saltos al tocar el suelo
        if (collision.gameObject.CompareTag("Suelo"))
        {
            saltosRestantes = 2;
        }

        // Destruir jugador si toca enemigo
        if (collision.gameObject.CompareTag("Enemigo"))
        {
            SceneManager.LoadScene("InGame_End");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Prisma"))
            SceneManager.LoadScene("Fabric_InGame");

        if (other.CompareTag("PrismaEnd"))
            SceneManager.LoadScene("InGame_End");
    }
}
