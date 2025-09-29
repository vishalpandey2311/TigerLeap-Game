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
    public bool autoStart = false;
    
    [Header("Spawn Positions")]
    [Tooltip("Predefined spawn positions for the cubes")]
    public Vector3[] spawnPositions = new Vector3[]
    {
        new Vector3(-5f, 0.5f, 200f),
        new Vector3(-2f, 0.5f, 200f),
        new Vector3(1f, 0.5f, 200f),
        new Vector3(4f, 0.5f, 200f)
    };

    [Tooltip("Predefined spawn rotations for the cubes (in Euler angles)")]
    public Vector3[] spawnRotations = new Vector3[]
    {
        new Vector3(0f, 0f, 0f),
        new Vector3(0f, 0f, 0f),
        new Vector3(0f, 0f, 0f),
        new Vector3(0f, 0f, 0f)
    };
    
    [Header("Material Assignment")]
    [Tooltip("Apply button materials to spawned cubes")]
    public bool applyButtonMaterials = true;
    
    [Header("Spawning Control")]
    [Tooltip("Enable/disable spawning at runtime")]
    public bool isSpawning = false;
    
    [Header("Debug")]
    [Tooltip("Show debug information in console")]
    public bool showDebug = false;
    
    private Coroutine spawnCoroutine;
    
    // Singleton for easy access
    public static PTSpawnManager Instance { get; private set; }
    
    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Menu manager controls spawning
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
        
        // Pick a random spawn position and its corresponding rotation
        int randomIndex = Random.Range(0, spawnPositions.Length);
        Vector3 spawnPosition = spawnPositions[randomIndex];
        Vector3 spawnRotation = randomIndex < spawnRotations.Length ? spawnRotations[randomIndex] : Vector3.zero;
        
        // Instantiate the cube with the specified rotation
        GameObject spawnedCube = Instantiate(cubePrefab, spawnPosition, Quaternion.Euler(spawnRotation));
        
        // Apply button material if enabled
        if (applyButtonMaterials && ButtonManager.Instance != null)
        {
            ButtonManager.Instance.ApplyMaterialToMovingCube(spawnedCube, spawnPosition);
        }
        
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

        // Ensure spawnRotations array matches spawnPositions length
        if (spawnPositions != null && spawnRotations != null && spawnRotations.Length != spawnPositions.Length)
        {
            System.Array.Resize(ref spawnRotations, spawnPositions.Length);
            Debug.LogWarning("PTSpawnManager: Resized spawnRotations array to match spawnPositions length");
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw spawn positions in scene view
        if (spawnPositions != null)
        {
            // Use different colors for each spawn position
            Color[] colors = { Color.red, Color.green, Color.blue, Color.yellow };
            
            for (int i = 0; i < spawnPositions.Length; i++)
            {
                Gizmos.color = colors[i % colors.Length];
                Gizmos.DrawWireCube(spawnPositions[i], Vector3.one);
                Gizmos.DrawWireSphere(spawnPositions[i], 0.5f);
                
                // Draw line to show movement path
                Gizmos.DrawLine(spawnPositions[i], new Vector3(spawnPositions[i].x, spawnPositions[i].y, -7f));
            }
        }
    }
}