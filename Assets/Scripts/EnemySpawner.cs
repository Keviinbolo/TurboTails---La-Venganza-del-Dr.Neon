using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configuración del Spawner")]
    public GameObject enemyPrefab;
    public int enemiesToSpawn = 3;
    public Transform[] spawnPoints;
    public Transform playerTarget;

    [Header("Corrección de Rotación")]
    public bool forceCorrectRotation = true;
    public Vector3 rotationCorrection = Vector3.zero;

    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private bool hasSpawned = false;
    private List<Transform> availableSpawnPoints = new List<Transform>();

    void Start()
    {
        if (!hasSpawned)
        {
            Invoke("SpawnEnemies", 0.1f);
        }
    }

    void SpawnEnemies()
    {
        if (hasSpawned) return;

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("No hay puntos de spawn asignados!", this);
            return;
        }

        if (playerTarget == null)
        {
            playerTarget = FindPlayer();
            if (playerTarget == null) 
            {
                Debug.LogError("No se encontró al jugador!");
                return;
            }
        }

        // Inicializar lista de spawn points disponibles
        availableSpawnPoints.Clear();
        foreach (Transform point in spawnPoints)
        {
            if (point != null) availableSpawnPoints.Add(point);
        }

        // Asegurar que no spawneamos más enemigos que puntos disponibles
        int actualEnemiesToSpawn = Mathf.Min(enemiesToSpawn, availableSpawnPoints.Count);

        for (int i = 0; i < actualEnemiesToSpawn; i++)
        {
            SpawnEnemyAtRandomPosition();
        }

        hasSpawned = true;
        Debug.Log($"Spawneados {actualEnemiesToSpawn} enemigos en puntos únicos");
    }

    void SpawnEnemyAtRandomPosition()
    {
        if (availableSpawnPoints.Count == 0)
        {
            Debug.LogWarning("No hay más spawn points disponibles");
            return;
        }

        // Elegir un spawn point aleatorio de los disponibles
        int randomIndex = Random.Range(0, availableSpawnPoints.Count);
        Transform spawnPoint = availableSpawnPoints[randomIndex];
        
        // Remover este punto de los disponibles para que no se use otra vez
        availableSpawnPoints.RemoveAt(randomIndex);

        // CALCULAR ROTACIÓN HACIA EL JUGADOR
        Vector3 directionToPlayer = playerTarget.position - spawnPoint.position;
        directionToPlayer.y = 0;
        Quaternion targetRotation = directionToPlayer != Vector3.zero ? 
            Quaternion.LookRotation(directionToPlayer) : Quaternion.identity;

        // APLICAR CORRECCIÓN SI ES NECESARIO
        if (forceCorrectRotation)
        {
            targetRotation *= Quaternion.Euler(rotationCorrection);
        }

        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, targetRotation);
        
        // FORZAR ROTACIÓN INMEDIATA después del spawn
        if (forceCorrectRotation)
        {
            enemy.transform.rotation = targetRotation;
        }

        spawnedEnemies.Add(enemy);
        SetupEnemy(enemy, spawnPoint.position);

        Debug.Log($"Enemigo spawneado en: {spawnPoint.name}");
    }

    Transform FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) return player.transform;

        player = GameObject.Find("Player");
        if (player != null) return player.transform;

        return null;
    }

    void SetupEnemy(GameObject enemy, Vector3 spawnPosition)
    {
        MovimientoEnemigo enemyMovement = enemy.GetComponent<MovimientoEnemigo>();
        if (enemyMovement != null)
        {
            enemyMovement.target = playerTarget;
            // Guardar la posición de spawn como posición inicial
            enemyMovement.SetStartPosition(spawnPosition);
        }
        else
        {
            Debug.LogWarning("El enemigo no tiene el script MovimientoEnemigo", enemy);
        }
    }

    [ContextMenu("Respawn Enemies")]
    public void RespawnEnemies()
    {
        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy != null) Destroy(enemy);
        }
        spawnedEnemies.Clear();
        hasSpawned = false;
        Invoke("SpawnEnemies", 0.1f);
    }

    // Método para spawnear un enemigo en un punto específico (útil para testing)
    public void SpawnEnemyAtPoint(Transform spawnPoint)
    {
        if (spawnPoint == null) return;

        Vector3 directionToPlayer = playerTarget.position - spawnPoint.position;
        directionToPlayer.y = 0;
        Quaternion targetRotation = directionToPlayer != Vector3.zero ? 
            Quaternion.LookRotation(directionToPlayer) : Quaternion.identity;

        if (forceCorrectRotation)
        {
            targetRotation *= Quaternion.Euler(rotationCorrection);
        }

        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, targetRotation);
        
        if (forceCorrectRotation)
        {
            enemy.transform.rotation = targetRotation;
        }

        spawnedEnemies.Add(enemy);
        SetupEnemy(enemy, spawnPoint.position);
    }

    public int GetAliveEnemiesCount()
    {
        int count = 0;
        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy != null) count++;
        }
        return count;
    }

    public int GetAvailableSpawnPointsCount()
    {
        return availableSpawnPoints.Count;
    }

    // Para debugging en el Editor
    #if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (spawnPoints != null)
        {
            foreach (Transform point in spawnPoints)
            {
                if (point != null)
                {
                    // Color diferente para puntos usados vs disponibles
                    bool isAvailable = availableSpawnPoints.Contains(point);
                    Gizmos.color = isAvailable ? Color.green : Color.red;
                    
                    Gizmos.DrawWireSphere(point.position, 0.5f);
                    
                    // Etiqueta con nombre del punto
                    UnityEditor.Handles.color = isAvailable ? Color.green : Color.red;
                    UnityEditor.Handles.Label(point.position, point.name);
                    
                    // Dibujar línea al jugador
                    if (playerTarget != null)
                    {
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawLine(point.position, playerTarget.position);
                    }
                }
            }
        }
    }
    #endif
}