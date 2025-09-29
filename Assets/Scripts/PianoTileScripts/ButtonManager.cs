using UnityEngine;
using System.Collections.Generic;

public class ButtonManager : MonoBehaviour
{
    [Header("Button References")]
    [Tooltip("The four button cubes in order (A, S, D, F)")]
    public CubeButton[] buttons = new CubeButton[4];
    
    [Header("Settings")]
    [Tooltip("Keys for the buttons")]
    public KeyCode[] buttonKeys = { KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F };
    
    [Header("Material Assignment")]
    [Tooltip("Materials for each button position (will be auto-extracted from buttons if empty)")]
    public Material[] buttonMaterials = new Material[4];
    
    [Tooltip("Automatically extract materials from button renderers")]
    public bool autoExtractMaterials = true;
    
    [Header("Debug")]
    [Tooltip("Show debug information")]
    public bool showDebug = false;
    
    // Singleton for easy access from spawn manager
    public static ButtonManager Instance { get; private set; }
    
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
            return;
        }
    }
    
    void Start()
    {
        SetupButtons();
        ExtractButtonMaterials();
    }
    
    /// <summary>
    /// Sets up the button keys
    /// </summary>
    private void SetupButtons()
    {
        for (int i = 0; i < buttons.Length && i < buttonKeys.Length; i++)
        {
            if (buttons[i] != null)
            {
                buttons[i].buttonKey = buttonKeys[i];
                
                if (showDebug)
                {
                    Debug.Log($"ButtonManager: Button {i} assigned key {buttonKeys[i]}");
                }
            }
        }
    }
    
    /// <summary>
    /// Extracts materials from button renderers
    /// </summary>
    private void ExtractButtonMaterials()
    {
        if (!autoExtractMaterials) return;
        
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] != null)
            {
                Renderer buttonRenderer = buttons[i].GetComponent<Renderer>();
                if (buttonRenderer != null)
                {
                    // Use original material if available, otherwise current material
                    Material materialToUse = buttons[i].originalMaterial != null 
                        ? buttons[i].originalMaterial 
                        : buttonRenderer.material;
                    
                    buttonMaterials[i] = materialToUse;
                    
                    if (showDebug)
                    {
                        Debug.Log($"ButtonManager: Extracted material '{materialToUse.name}' from button {i}");
                    }
                }
                else
                {
                    if (showDebug)
                        Debug.LogWarning($"ButtonManager: Button {i} has no Renderer component");
                }
            }
        }
    }
    
    /// <summary>
    /// Gets the material for a specific button position (0-3)
    /// </summary>
    public Material GetButtonMaterial(int buttonIndex)
    {
        if (buttonIndex >= 0 && buttonIndex < buttonMaterials.Length)
        {
            return buttonMaterials[buttonIndex];
        }
        
        if (showDebug)
        {
            Debug.LogWarning($"ButtonManager: Invalid button index {buttonIndex}");
        }
        
        return null;
    }
    
    /// <summary>
    /// Gets the material for a specific spawn position based on X coordinate
    /// </summary>
    public Material GetMaterialForSpawnPosition(Vector3 spawnPosition)
    {
        // Map spawn positions to button indices
        // Assuming spawn positions are: [-5, -2, 1, 4] mapping to buttons [0, 1, 2, 3]
        int buttonIndex = GetButtonIndexFromPosition(spawnPosition);
        return GetButtonMaterial(buttonIndex);
    }
    
    /// <summary>
    /// Gets button index based on spawn position
    /// </summary>
    private int GetButtonIndexFromPosition(Vector3 spawnPosition)
    {
        // Get spawn positions from PTSpawnManager
        PTSpawnManager spawnManager = PTSpawnManager.Instance ?? Object.FindFirstObjectByType<PTSpawnManager>();
        
        if (spawnManager != null && spawnManager.spawnPositions != null)
        {
            // Find the closest spawn position
            float minDistance = float.MaxValue;
            int closestIndex = 0;
            
            for (int i = 0; i < spawnManager.spawnPositions.Length; i++)
            {
                float distance = Mathf.Abs(spawnPosition.x - spawnManager.spawnPositions[i].x);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestIndex = i;
                }
            }
            
            return closestIndex;
        }
        
        // Fallback: estimate based on X position
        if (spawnPosition.x < -3.5f) return 0;      // Button A
        else if (spawnPosition.x < -0.5f) return 1; // Button S
        else if (spawnPosition.x < 2.5f) return 2;  // Button D
        else return 3;                              // Button F
    }
    
    /// <summary>
    /// Applies material to a moving cube based on its position
    /// </summary>
    public void ApplyMaterialToMovingCube(GameObject cube, Vector3 spawnPosition)
    {
        if (cube == null) return;
        
        Material materialToApply = GetMaterialForSpawnPosition(spawnPosition);
        
        if (materialToApply != null)
        {
            Renderer cubeRenderer = cube.GetComponent<Renderer>();
            if (cubeRenderer != null)
            {
                // Get existing materials
                Material[] materials = cubeRenderer.materials;
                
                // Find the index of the "Top" material
                int topMaterialIndex = -1;
                for (int i = 0; i < materials.Length; i++)
                {
                    if (materials[i].name.Contains("Top"))
                    {
                        topMaterialIndex = i;
                        break;
                    }
                }
                
                // If we found the "Top" material, only change that one
                if (topMaterialIndex != -1)
                {
                    materials[topMaterialIndex] = materialToApply;
                    cubeRenderer.materials = materials;
                }
                else
                {
                    // Fallback: if we can't find "Top" material, change all materials
                    cubeRenderer.material = materialToApply;
                }
                
                if (showDebug)
                {
                    int buttonIndex = GetButtonIndexFromPosition(spawnPosition);
                    Debug.Log($"ButtonManager: Applied material '{materialToApply.name}' to cube for button {buttonIndex} (Top material only)");
                }
            }
        }
    }
    
    /// <summary>
    /// Refresh materials from buttons (useful for runtime changes)
    /// </summary>
    [ContextMenu("Refresh Button Materials")]
    public void RefreshButtonMaterials()
    {
        ExtractButtonMaterials();
    }
    
    /// <summary>
    /// Manual button press for testing
    /// </summary>
    [ContextMenu("Test Button A")]
    public void TestButtonA() { if (buttons[0] != null) buttons[0].PressButton(); }
    
    [ContextMenu("Test Button S")]
    public void TestButtonS() { if (buttons[1] != null) buttons[1].PressButton(); }
    
    [ContextMenu("Test Button D")]
    public void TestButtonD() { if (buttons[2] != null) buttons[2].PressButton(); }
    
    [ContextMenu("Test Button F")]
    public void TestButtonF() { if (buttons[3] != null) buttons[3].PressButton(); }
}