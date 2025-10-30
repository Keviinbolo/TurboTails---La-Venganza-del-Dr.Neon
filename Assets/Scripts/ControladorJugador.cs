using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class ControladorJugador : MonoBehaviour
{
    public float velocidad = 5f;
    public float fuerzaSalto = 5f;
    public float fuerzaDobleSalto = 3f; // fuerza menor para el doble salto

    private Rigidbody rb;
    private int saltosRestantes = 1; // 1 salto adicional (doble salto)

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Movimiento
        float movimientoHorizontal = Input.GetAxis("Horizontal");
        float movimientoVertical = Input.GetAxis("Vertical");

        Vector3 direccion = new Vector3(movimientoHorizontal, 0f, movimientoVertical);
        direccion = transform.TransformDirection(direccion);

        Vector3 movimiento = direccion * velocidad * Time.deltaTime;
        rb.MovePosition(rb.position + movimiento);

        // Salto y doble salto
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (saltosRestantes > 0)
            {
                float fuerza = (saltosRestantes == 1) ? fuerzaSalto : fuerzaDobleSalto;
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // resetear velocidad vertical
                rb.AddForce(Vector3.up * fuerza, ForceMode.Impulse);
                saltosRestantes--;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Suelo"))
        {
            saltosRestantes = 2; // Permite salto + doble salto
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Prisma"))
        {
            SceneManager.LoadScene("Fabric_InGame");
        }
        if (other.CompareTag("PrismaEnd"))
        {
            SceneManager.LoadScene("InGame_End");
        }
    }
}
