using UnityEngine;
using UnityEngine.AI;

public class MovimientoEnemigo : MonoBehaviour
{
    [Header("Referencias")]
    public Transform target;           // El jugador
    
    [Header("Detección")]
    public float detectionRange = 30f; // Rango para detectar al jugador
    public float attackDistance = 0f;  // Distancia de ataque
    
    [Header("Configuración de Movimiento")]
    public float rotationSpeed = 5f;   // Velocidad de rotación suave
    
    [Header("Configuración de Ataque")]
    public float attackCooldown = 0.5f; // Tiempo entre ataques
              

    [Header("Estados")]
    public bool playerDetected = false;
    public bool isChasing = false;

    private NavMeshAgent agent;
    private float lastAttackTime;
    private Vector3 startPosition;
    private bool hasReturned = true;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false; // Nosotros controlamos la rotación
        
        // Si no se ha asignado startPosition, usar la posición actual
        if (startPosition == Vector3.zero)
        {
            startPosition = transform.position;
        }
        
        lastAttackTime = -attackCooldown;
    }

    // Método público para que el spawner asigne la posición inicial
    public void SetStartPosition(Vector3 position)
    {
        startPosition = position;
        hasReturned = true; // Comienza en su posición inicial
    }

    void Update()
    {
        if (target == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, target.position);

        // DETECCIÓN DEL JUGADOR
        if (!playerDetected && distanceToPlayer <= detectionRange)
        {
            PlayerDetected();
        }
        else if (playerDetected && distanceToPlayer > detectionRange * 1.2f) // Margen para evitar flickering
        {
            PlayerLost();
        }

        // COMPORTAMIENTO BASADO EN EL ESTADO
        if (playerDetected)
        {
            if (distanceToPlayer > attackDistance)
            {
                // Perseguir al jugador
                ChasePlayer();
            }
            else
            {
                // Atacar cuando está cerca
               // AttackPlayer();
            }
        }
        else if (!hasReturned)
        {
            // Regresar a la posición inicial
            ReturnToStart();
        }
    }

    void PlayerDetected()
    {
        playerDetected = true;
        isChasing = true;
        hasReturned = false;
        Debug.Log("¡Jugador detectado!");
    }

    void PlayerLost()
    {
        playerDetected = false;
        isChasing = false;
        Debug.Log("Jugador perdido, regresando...");
    }

    void ChasePlayer()
    {
        agent.isStopped = false;
        agent.SetDestination(target.position);
        
        // Rotar hacia el jugador
        LookAtPlayerSmooth();
    }

    void AttackPlayer()
    {
        // Detener movimiento
        agent.isStopped = true;
        
        // Mirar al jugador
        LookAtPlayerSmooth();
        
        // Atacar
        Attack();
    }

    void ReturnToStart()
    {
        float distanceToStart = Vector3.Distance(transform.position, startPosition);
        
        if (distanceToStart > 0.5f) // Margen pequeño
        {
            // Regresar a la posición inicial
            agent.isStopped = false;
            agent.SetDestination(startPosition);
            
            // Rotar hacia la dirección del movimiento
            if (agent.velocity.magnitude > 0.1f)
            {
                RotateTowardsMovement();
            }
        }
        else
        {
            // Llegó a la posición inicial
            agent.isStopped = true;
            hasReturned = true;
            transform.position = startPosition; // Asegurar posición exacta
        }
    }

    void LookAtPlayerSmooth()
    {
        Vector3 direction = target.position - transform.position;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void RotateTowardsMovement()
    {
        Vector3 direction = agent.velocity.normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void Attack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            Debug.Log("¡Ataque al jugador!");

            // Aquí puedes llamar al script de salud del jugador
           
        }
    }

    // Para debugging
    void OnDrawGizmosSelected()
    {
        // Rango de detección (verde)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Rango de ataque (rojo)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
        
        // Línea al jugador si está detectado
        if (playerDetected && target != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, target.position);
        }
        
        // Posición inicial (azul) - solo en Play Mode
        if (Application.isPlaying)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(startPosition, Vector3.one * 0.5f);
            if (!hasReturned)
            {
                Gizmos.DrawLine(transform.position, startPosition);
            }
        }
    }
}