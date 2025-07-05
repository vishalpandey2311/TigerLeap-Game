using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PTSpawnManager : MonoBehaviour
{
    [Header("Cube Spawning")]
    [Tooltip("The cube prefab to spawn")]
    public GameObject cubePrefab;
    
    [Header("Spawn Settings")]
    [Tooltip("Time interval between spawns (in seconds)")]
    public float spawnInterval = 2f;
    
    [Tooltip("Whether to start spawning automatically")]
    public bool autoStart = false; // Changed to false for menu control
    
    [Header("Spawn Positions")]
    [Tooltip("Predefined spawn positions for the cubes")]
    public Vector3[] spawnPositions = new Vector3[]
    {
        new Vector3(-5f, 0.5f, 200f),
        new Vector3(-2f, 0.5f, 200f),
        new Vector3(1f, 0.5f, 200f),
        new Vector3(4f, 0.5f, 200f)
    };
    
    [Header("Spawning Control")]
    [Tooltip("Enable/disable spawning at runtime")]
    public bool isSpawning = false;
    
    [Header("Debug")]
    [Tooltip("Show debug information in console")]
    public bool showDebug = false;
    
    private Coroutine spawnCoroutine;
    
    void Start()
    {
        // Remove auto-start - let PTMenuManager control this
        // if (autoStart)
        // {
        //     StartSpawning();
        // }
    }
    
    /// <summary>
    /// Starts the spawning process
    /// </summary>
    public void StartSpawning()
    {
        if (!isSpawning && cubePrefab != null)
        {
            isSpawning = true;
            spawnCoroutine = StartCoroutine(SpawnCubes());
            
            if (showDebug)
                Debug.Log("PTSpawnManager: Started spawning cubes");
        }
    }
    
    /// <summary>
    /// Stops the spawning process
    /// </summary>
    public void StopSpawning()
    {
        if (isSpawning)
        {
            isSpawning = false;
            
            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
                spawnCoroutine = null;
            }
            
            if (showDebug)
                Debug.Log("PTSpawnManager: Stopped spawning cubes");
        }
    }
    
    /// <summary>
    /// Coroutine that handles the spawning logic
    /// </summary>
    private IEnumerator SpawnCubes()
    {
        while (isSpawning)
        {
            SpawnRandomCube();
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    
    /// <summary>
    /// Spawns a cube at a random position from the predefined positions
    /// </summary>
    private void SpawnRandomCube()
    {
        if (cubePrefab == null || spawnPositions.Length == 0)
        {
            if (showDebug)
                Debug.LogWarning("PTSpawnManager: Cannot spawn cube - missing prefab or spawn positions");
            return;
        }
        
        // Pick a random spawn position
        int randomIndex = Random.Range(0, spawnPositions.Length);
        Vector3 spawnPosition = spawnPositions[randomIndex];
        
        // Instantiate the cube
        GameObject spawnedCube = Instantiate(cubePrefab, spawnPosition, Quaternion.identity);
        
        if (showDebug)
        {
            Debug.Log($"PTSpawnManager: Spawned cube at position {spawnPosition} (index: {randomIndex})");
        }
    }
    
    /// <summary>
    /// Spawns a single cube manually (useful for testing)
    /// </summary>
    [ContextMenu("Spawn Single Cube")]
    public void SpawnSingleCube()
    {
        SpawnRandomCube();
    }
    
    /// <summary>
    /// Toggle spawning on/off
    /// </summary>
    [ContextMenu("Toggle Spawning")]
    public void ToggleSpawning()
    {
        if (isSpawning)
        {
            StopSpawning();
        }
        else
        {
            StartSpawning();
        }
    }
    
    void OnValidate()
    {
        // Ensure spawn interval is not negative
        if (spawnInterval < 0f)
        {
            spawnInterval = 0f;
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw spawn positions in scene view
        if (spawnPositions != null)
        {
            Gizmos.color = Color.yellow;
            foreach (Vector3 position in spawnPositions)
            {
                Gizmos.DrawWireCube(position, Vector3.one);
                Gizmos.DrawWireSphere(position, 0.5f);
            }
        }
    }
}