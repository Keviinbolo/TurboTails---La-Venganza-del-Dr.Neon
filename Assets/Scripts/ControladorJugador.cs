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

    [Header("Giro Estético")]
    public float suavizadoRot = 6f; // velocidad de giro hacia dirección de movimiento

    [Header("Ataque (Giro)")]
    public float duracionGiro = 0.25f;
    public float duracionAtaque = 0.4f;
    private bool atacando = false;

    private Rigidbody rb;
    private int saltosRestantes = 2;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    void Update()
    {
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

    void FixedUpdate()
    {
        if (!atacando)
        {
            float movimientoHorizontal = Input.GetAxis("Horizontal");
            float movimientoVertical = Input.GetAxis("Vertical");

            Vector3 direccion = new Vector3(movimientoHorizontal, 0f, movimientoVertical);

            // Movimiento con Rigidbody respetando colisiones
            Vector3 velocidadMovimiento = transform.TransformDirection(direccion) * velocidad;
            rb.linearVelocity = new Vector3(velocidadMovimiento.x, rb.linearVelocity.y, velocidadMovimiento.z);

            // Giro hacia la dirección de movimiento
            if (direccion.sqrMagnitude > 0.01f)
            {
                Quaternion rotObjetivo = Quaternion.LookRotation(direccion.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotObjetivo, Time.fixedDeltaTime * suavizadoRot);
            }
        }
    }

    private IEnumerator GiroDeAtaque()
    {
        atacando = true;

        Quaternion rotacionInicial = transform.rotation;
        Quaternion rotacionGirado = rotacionInicial * Quaternion.Euler(0f, 180f, 0f);

        float t = 0f;
        while (t < duracionGiro)
        {
            t += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(rotacionInicial, rotacionGirado, t / duracionGiro);
            yield return null;
        }

        transform.rotation = rotacionGirado;
        yield return new WaitForSeconds(duracionAtaque);

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

        if (collision.gameObject.CompareTag("Enemigo") || collision.gameObject.CompareTag("Subsuelo"))
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
