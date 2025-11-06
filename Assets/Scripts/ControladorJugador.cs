using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class ControladorJugador : MonoBehaviour
{
    public float velocidad = 5f;
    public float fuerzaSalto = 5f;
    public float fuerzaDobleSalto = 5f;

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
        direccion = transform.TransformDirection(direccion);

        Vector3 movimiento = direccion * velocidad * Time.deltaTime;
        rb.MovePosition(rb.position + movimiento);

        // Saltos
        if (Input.GetKeyDown(KeyCode.Space) && saltosRestantes > 0)
        {
            // Resetear velocidad vertical para evitar saltos débiles y asegurar salto robusto
            Vector3 v = rb.linearVelocity;
            v.y = 0f;
            rb.linearVelocity = v;

            float fuerza = (saltosRestantes == 2) ? fuerzaSalto : fuerzaDobleSalto;
            rb.AddForce(Vector3.up * fuerza, ForceMode.Impulse);

            saltosRestantes--;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Suelo"))
        {
            saltosRestantes = 2; // Siempre recuperar doble salto al tocar suelo
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
