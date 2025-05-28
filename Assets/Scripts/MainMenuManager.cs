using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI titleText;
    
    void Start()
    {
        // Set the title text (optional if you already set it in the inspector)
        if (titleText != null)
            titleText.text = "TigerLeap";
    }
    
    public void StartGame()
    {
        // Load your game scene - replace "GameScene" with your actual game scene name
        // If your game scene is the scene at build index 1, you can use:
        SceneManager.LoadScene(1);
        // Or if you prefer to use the scene name:
        // SceneManager.LoadScene("GameScene");
    }
    
    public void QuitGame()
    {
        // This will close the application
        Debug.Log("Quitting game...");
        Application.Quit();
        
        // Note: Application.Quit() only works in a built game
        // It won't do anything in the Unity Editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}