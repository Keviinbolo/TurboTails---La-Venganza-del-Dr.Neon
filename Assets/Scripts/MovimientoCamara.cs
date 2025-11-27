using UnityEngine;

public class MovimientoCamara : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
     public float inclinacionMax = 10f;
    public float suavizado = 5f;

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Quaternion rotObjetivo = Quaternion.Euler(
            vertical * inclinacionMax,  // adelante/atrás
            0,
            -horizontal * inclinacionMax // izquierda/derecha
        );

        transform.localRotation = Quaternion.Lerp(
            transform.localRotation,
            rotObjetivo,
            Time.deltaTime * suavizado
        );
    }
}
