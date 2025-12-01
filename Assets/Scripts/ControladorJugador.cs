using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class ControladorJugador : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 5f;
    [SerializeField] private float fuerzaSalto = 5f;
    [SerializeField] private float fuerzaDobleSalto = 5f;
    [SerializeField] private int saltosMaximos = 2;

    [Header("Giro Estético")]
    [SerializeField] private float suavizadoRot = 6f;

    [Header("Ataque (Giro)")]
    [SerializeField] private float duracionGiro = 0.25f;
    [SerializeField] private float duracionAtaque = 0.4f;
    [SerializeField] private float anguloGiroAtaque = 180f;

    // Constantes de etiquetas
    private const string TAG_SUELO = "Suelo";
    private const string TAG_ENEMIGO = "Enemigo";
    private const string TAG_SUBSUELO = "Subsuelo";
    private const string TAG_PRISMA = "Prisma";
    private const string TAG_PRISMA_END = "PrismaEnd";

    // Constantes de escenas
    private const string SCENE_DEAD = "InGame_Dead";
    private const string SCENE_FABRIC = "Fabric_InGame";
    private const string SCENE_END = "InGame_End";

    // Estado
    private Rigidbody rb;
    private bool atacando = false;
    private int saltosRestantes;

    // Input cacheado
    private float inputHorizontal;
    private float inputVertical;
    private bool jumpPressed;
    private bool attackPressed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        saltosRestantes = saltosMaximos;
    }

    private void Update()
    {
        // Leer input SOLO aquí
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        inputVertical = Input.GetAxisRaw("Vertical");
        jumpPressed = Input.GetKeyDown(KeyCode.Space);
        attackPressed = Input.GetKeyDown(KeyCode.E);

        // Saltos
        if (jumpPressed && saltosRestantes > 0)
        {
            RealizarSalto();
        }

        // Ataque
        if (attackPressed && !atacando)
        {
            StartCoroutine(GiroDeAtaque());
        }
    }

    private void FixedUpdate()
    {
        if (!atacando)
        {
            MoverJugador();
            RotarHaciaMovimiento();
        }
    }

    private void MoverJugador()
    {
        Vector3 direccion = new Vector3(inputHorizontal, 0f, inputVertical);
        Vector3 velocidadMovimiento = transform.TransformDirection(direccion.normalized) * velocidad;

        // Mantener componente Y de la velocidad para no romper gravedad/saltos
        rb.linearVelocity = new Vector3(velocidadMovimiento.x, rb.linearVelocity.y, velocidadMovimiento.z);
    }

    private void RotarHaciaMovimiento()
    {
        Vector3 direccion = new Vector3(inputHorizontal, 0f, inputVertical);
        if (direccion.sqrMagnitude > 0.01f)
        {
            Quaternion rotObjetivo = Quaternion.LookRotation(direccion.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rotObjetivo,
                Time.fixedDeltaTime * suavizadoRot
            );
        }
    }

    private void RealizarSalto()
    {
        // Resetear velocidad vertical para tener salto consistente
        Vector3 v = rb.linearVelocity;
        v.y = 0f;
        rb.linearVelocity = v;

        float fuerza = (saltosRestantes == saltosMaximos) ? fuerzaSalto : fuerzaDobleSalto;
        rb.AddForce(Vector3.up * fuerza, ForceMode.Impulse);
        saltosRestantes--;
    }

    private IEnumerator GiroDeAtaque()
    {
        atacando = true;

        Quaternion rotacionInicial = transform.rotation;
        Quaternion rotacionGirado =
            rotacionInicial * Quaternion.Euler(0f, anguloGiroAtaque, 0f);

        // Giro de ida
        float elapsed = 0f;
        while (elapsed < duracionGiro)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duracionGiro);
            transform.rotation = Quaternion.Slerp(rotacionInicial, rotacionGirado, t);
            yield return null;
        }
        transform.rotation = rotacionGirado;

        // Ventana de ataque
        yield return new WaitForSeconds(duracionAtaque);

        // Giro de vuelta
        elapsed = 0f;
        while (elapsed < duracionGiro)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duracionGiro);
            transform.rotation = Quaternion.Slerp(rotacionGirado, rotacionInicial, t);
            yield return null;
        }
        transform.rotation = rotacionInicial;

        atacando = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(TAG_SUELO))
        {
            saltosRestantes = saltosMaximos;
        }

        if (collision.gameObject.CompareTag(TAG_ENEMIGO) ||
            collision.gameObject.CompareTag(TAG_SUBSUELO))
        {
            SceneManager.LoadScene(SCENE_DEAD);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TAG_PRISMA))
        {
            SceneManager.LoadScene(SCENE_FABRIC);
        }

        if (other.CompareTag(TAG_PRISMA_END))
        {
            SceneManager.LoadScene(SCENE_END);
        }
    }
}
