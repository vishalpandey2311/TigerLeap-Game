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
    
    [Header("Debug")]
    [Tooltip("Show debug information")]
    public bool showDebug = false;
    
    void Start()
    {
        SetupButtons();
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