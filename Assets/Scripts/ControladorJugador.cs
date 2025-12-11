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

    [Header("Referencias")]
    [SerializeField] private Transform modeloJugador;

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

    private Camera cam;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        saltosRestantes = saltosMaximos;

        // Obtener la cámara principal
        cam = Camera.main;

        // Si no se asignó modeloJugador, usar este transform
        if (modeloJugador == null)
        {
            modeloJugador = transform;
            Debug.Log("Usando transform principal como modelo visual");
        }
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
        }
    }

    private void MoverJugador()
    {
        Vector3 direccion = new Vector3(inputHorizontal, 0f, inputVertical);

        if (direccion.magnitude > 0.1f)
        {
            // Convertir input a dirección relativa a la cámara
            Vector3 camForward = cam.transform.forward;
            Vector3 camRight = cam.transform.right;

            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            Vector3 direccionRelativa = (camForward * inputVertical + camRight * inputHorizontal).normalized;

            Vector3 velocidadMovimiento = direccionRelativa * velocidad;
            rb.linearVelocity = new Vector3(velocidadMovimiento.x, rb.linearVelocity.y, velocidadMovimiento.z);

            // Rotar SOLO el modelo hacia la dirección de movimiento
            // ¡CORRECCIÓN AQUÍ! Se añade el signo negativo para invertir la dirección
            RotarModeloHaciaMovimiento(-direccionRelativa);
        }
        else
        {
            // Frenar en X y Z cuando no hay input
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }
    }

    private void RotarModeloHaciaMovimiento(Vector3 direccionMovimiento)
    {
        if (direccionMovimiento.sqrMagnitude > 0.01f)
        {
            Quaternion rotObjetivo = Quaternion.LookRotation(direccionMovimiento, Vector3.up);
            modeloJugador.rotation = Quaternion.Slerp(
                modeloJugador.rotation,
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

        // Usar la rotación actual del MODELO, no del transform principal
        Quaternion rotacionInicial = modeloJugador.rotation;
        Quaternion rotacionGirado = rotacionInicial * Quaternion.Euler(0f, anguloGiroAtaque, 0f);

        // Giro de ida
        float elapsed = 0f;
        while (elapsed < duracionGiro)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duracionGiro);
            modeloJugador.rotation = Quaternion.Slerp(rotacionInicial, rotacionGirado, t);
            yield return null;
        }
        modeloJugador.rotation = rotacionGirado;

        // Ventana de ataque
        yield return new WaitForSeconds(duracionAtaque);

        // Giro de vuelta
        elapsed = 0f;
        while (elapsed < duracionGiro)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duracionGiro);
            modeloJugador.rotation = Quaternion.Slerp(rotacionGirado, rotacionInicial, t);
            yield return null;
        }
        modeloJugador.rotation = rotacionInicial;

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