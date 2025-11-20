using UnityEngine;
using UnityEngine.AI;

public class MovimientoEnemigo : MonoBehaviour
{
    public Transform target;           // El jugador
    public float attackDistance = 2f;  // Distancia de ataque
    public float attackCooldown = 1.5f; // Tiempo entre ataques
    public int damage = 10;            // Daño del ataque

    private NavMeshAgent agent;
    private float lastAttackTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false; // Mantener rotación fija
        lastAttackTime = -attackCooldown; // Permitir atacar inmediatamente
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > attackDistance)
        {
            // Perseguir al jugador siempre
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
        else
        {
            // Detenerse y atacar cuando esté cerca
            agent.isStopped = true;
            Attack();
        }
    }

    void Attack()
    {
        // Ataque con cooldown
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            Debug.Log("¡Ataque al jugador! Daño: " + damage);

            // Aquí puedes llamar al script de salud del jugador, por ejemplo:
            // target.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }
}
