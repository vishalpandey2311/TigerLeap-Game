using UnityEngine;
using System.Collections;

public class CubeButton : MonoBehaviour
{
    [Header("Button Settings")]
    [Tooltip("The key that activates this button")]
    public KeyCode buttonKey = KeyCode.A;
    
    [Header("Catch Settings")]
    [Tooltip("Range within which cubes can be caught")]
    public float catchRange = 2f;
    
    [Tooltip("Tag of objects that can be caught")]
    public string catchableTag = "MovingCube";
    
    [Tooltip("How often to check for cubes while key is held (per second)")]
    public float catchFrequency = 10f;
    
    [Header("Visual Feedback")]
    [Tooltip("Material when button is pressed")]
    public Material pressedMaterial;
    
    [Tooltip("Original material")]
    public Material originalMaterial;
    
    [Tooltip("Duration of press animation")]
    public float pressDuration = 0.1f;
    
    [Header("Debug")]
    [Tooltip("Show debug information")]
    public bool showDebug = false;
    
    private Renderer buttonRenderer;
    private bool isPressed = false;
    private bool isKeyHeld = false;
    private Coroutine continuousCatchCoroutine;
    private float nextCatchTime = 0f;
    
    void Start()
    {
        buttonRenderer = GetComponent<Renderer>();
        if (originalMaterial == null && buttonRenderer != null)
        {
            originalMaterial = buttonRenderer.material;
        }
    }
    
    void Update()
    {
        HandleInput();
    }
    
    /// <summary>
    /// Handles input for this button
    /// </summary>
    private void HandleInput()
    {
        // Check for key press (start holding)
        if (Input.GetKeyDown(buttonKey))
        {
            StartHoldingKey();
        }
        
        // Check for key release (stop holding)
        if (Input.GetKeyUp(buttonKey))
        {
            StopHoldingKey();
        }
        
        // Continuous catching while key is held
        if (isKeyHeld && Time.time >= nextCatchTime)
        {
            CatchNearbyMovingCubes();
            nextCatchTime = Time.time + (1f / catchFrequency);
        }
    }
    
    /// <summary>
    /// Called when key starts being held
    /// </summary>
    private void StartHoldingKey()
    {
        if (isKeyHeld) return;
        
        isKeyHeld = true;
        nextCatchTime = Time.time; // Allow immediate first catch
        
        // Start visual feedback
        if (!isPressed)
        {
            StartCoroutine(ButtonPressAnimation());
        }
        
        if (showDebug)
        {
            Debug.Log($"CubeButton: {buttonKey} started being held!");
        }
    }
    
    /// <summary>
    /// Called when key stops being held
    /// </summary>
    private void StopHoldingKey()
    {
        if (!isKeyHeld) return;
        
        isKeyHeld = false;
        
        if (showDebug)
        {
            Debug.Log($"CubeButton: {buttonKey} stopped being held!");
        }
    }
    
    /// <summary>
    /// Called when button is pressed (for external calls)
    /// </summary>
    public void PressButton()
    {
        CatchNearbyMovingCubes();
        
        if (!isPressed)
        {
            StartCoroutine(ButtonPressAnimation());
        }
        
        if (showDebug)
        {
            Debug.Log($"CubeButton: {buttonKey} pressed manually!");
        }
    }
    
    /// <summary>
    /// Finds and catches nearby moving cubes
    /// </summary>
    private void CatchNearbyMovingCubes()
    {
        // Find all objects with the catchable tag
        GameObject[] movingCubes = GameObject.FindGameObjectsWithTag(catchableTag);
        
        foreach (GameObject cube in movingCubes)
        {
            float distance = Vector3.Distance(transform.position, cube.transform.position);
            
            if (distance <= catchRange)
            {
                CatchCube(cube);
                // Don't break here - catch all cubes in range while key is held
            }
        }
    }
    
    /// <summary>
    /// Catches and destroys a cube
    /// </summary>
    private void CatchCube(GameObject cube)
    {
        if (showDebug)
        {
            Debug.Log($"CubeButton: Caught cube at distance {Vector3.Distance(transform.position, cube.transform.position)}");
        }
        
        // Notify score manager BEFORE destroying the cube
        if (PTScoreManager.Instance != null)
        {
            PTScoreManager.Instance.OnCubeCaught();
        }
        
        // Add any catch effects here (particles, sound, etc.)
        
        // Destroy the caught cube
        Destroy(cube);
    }
    
    /// <summary>
    /// Visual animation for button press
    /// </summary>
    private IEnumerator ButtonPressAnimation()
    {
        isPressed = true;
        
        // Change material if available
        if (pressedMaterial != null && buttonRenderer != null)
        {
            buttonRenderer.material = pressedMaterial;
        }
        
        // Scale animation - stay pressed while key is held
        Vector3 originalScale = transform.localScale;
        Vector3 pressedScale = originalScale * 0.9f;
        
        // Press down
        float timer = 0f;
        while (timer < pressDuration && isKeyHeld)
        {
            timer += Time.deltaTime;
            float progress = timer / pressDuration;
            transform.localScale = Vector3.Lerp(originalScale, pressedScale, progress);
            yield return null;
        }
        
        // Stay pressed while key is held
        while (isKeyHeld)
        {
            transform.localScale = pressedScale;
            yield return null;
        }
        
        // Press up when key is released
        timer = 0f;
        while (timer < pressDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / pressDuration;
            transform.localScale = Vector3.Lerp(pressedScale, originalScale, progress);
            yield return null;
        }
        
        // Reset
        transform.localScale = originalScale;
        
        if (originalMaterial != null && buttonRenderer != null)
        {
            buttonRenderer.material = originalMaterial;
        }
        
        isPressed = false;
    }
    
    /// <summary>
    /// Draws the catch range in scene view
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = isKeyHeld ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, catchRange);
        
        // Draw additional visual when key is held
        if (isKeyHeld)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 0.2f);
        }
    }
}