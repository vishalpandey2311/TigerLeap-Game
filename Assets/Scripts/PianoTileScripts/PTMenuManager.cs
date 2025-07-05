using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PTMenuManager : MonoBehaviour
{
    [Header("Menu References")]
    [Tooltip("The main menu panel")]
    public GameObject ptMenuPanel;
    
    [Header("Button References")]
    [Tooltip("Start game button")]
    public Button startButton;
    
    [Tooltip("Quit game button")]
    public Button quitButton;
    
    [Header("Game Components")]
    [Tooltip("Reference to PTSpawnManager")]
    public PTSpawnManager spawnManager;
    
    [Tooltip("Reference to ButtonManager")]
    public ButtonManager buttonManager;
    
    [Tooltip("Reference to PTScoreManager")]
    public PTScoreManager scoreManager;
    
    [Header("Audio")]
    [Tooltip("Button click sound")]
    public AudioClip buttonClickSound;
    
    [Header("Debug")]
    [Tooltip("Show debug information")]
    public bool showDebug = false;
    
    private AudioSource audioSource;
    private bool gameStarted = false;
    
    void Start()
    {
        SetupMenu();
        SetupAudio();
        ShowMenu();
    }
    
    /// <summary>
    /// Sets up the menu system
    /// </summary>
    private void SetupMenu()
    {
        // Setup button listeners
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
        }
        
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitButtonClicked);
        }
        
        // Ensure game components are stopped initially
        if (spawnManager != null)
        {
            spawnManager.StopSpawning();
        }
        
        if (showDebug)
        {
            Debug.Log("PTMenuManager: Menu setup complete");
        }
    }
    
    /// <summary>
    /// Sets up audio source for button sounds
    /// </summary>
    private void SetupAudio()
    {
        if (buttonClickSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = buttonClickSound;
            audioSource.playOnAwake = false;
            audioSource.volume = 0.7f;
        }
    }
    
    /// <summary>
    /// Shows the menu panel
    /// </summary>
    public void ShowMenu()
    {
        if (ptMenuPanel != null)
        {
            ptMenuPanel.SetActive(true);
            
            // Pause the game time while in menu
            Time.timeScale = 0f;
            
            if (showDebug)
            {
                Debug.Log("PTMenuManager: Menu shown");
            }
        }
    }
    
    /// <summary>
    /// Hides the menu panel
    /// </summary>
    public void HideMenu()
    {
        if (ptMenuPanel != null)
        {
            ptMenuPanel.SetActive(false);
            
            // Resume game time
            Time.timeScale = 1f;
            
            if (showDebug)
            {
                Debug.Log("PTMenuManager: Menu hidden");
            }
        }
    }
    
    /// <summary>
    /// Called when Start button is clicked
    /// </summary>
    public void OnStartButtonClicked()
    {
        PlayButtonSound();
        
        if (showDebug)
        {
            Debug.Log("PTMenuManager: Start button clicked - Starting game");
        }
        
        StartGame();
    }
    
    /// <summary>
    /// Called when Quit button is clicked
    /// </summary>
    public void OnQuitButtonClicked()
    {
        PlayButtonSound();
        
        if (showDebug)
        {
            Debug.Log("PTMenuManager: Quit button clicked - Quitting game");
        }
        
        QuitGame();
    }
    
    /// <summary>
    /// Starts the game
    /// </summary>
    private void StartGame()
    {
        gameStarted = true;
        
        // Reset score for new game
        if (scoreManager != null)
        {
            scoreManager.ResetScore();
        }
        else if (PTScoreManager.Instance != null)
        {
            PTScoreManager.Instance.ResetScore();
        }
        
        // Hide the menu
        HideMenu();
        
        // Start spawning cubes
        if (spawnManager != null)
        {
            spawnManager.StartSpawning();
        }
        
        // Enable button controls (they should already be active)
        if (buttonManager != null && showDebug)
        {
            Debug.Log("PTMenuManager: Button controls ready");
        }
        
        if (showDebug)
        {
            Debug.Log("PTMenuManager: Game started successfully");
        }
    }
    
    /// <summary>
    /// Quits the game
    /// </summary>
    private void QuitGame()
    {
        if (showDebug)
        {
            Debug.Log("PTMenuManager: Quitting game...");
        }
        
        // Stop any ongoing game processes
        if (spawnManager != null)
        {
            spawnManager.StopSpawning();
        }
        
        // Add any cleanup here
        
        // Quit the application
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    
    /// <summary>
    /// Plays button click sound
    /// </summary>
    private void PlayButtonSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }
    
    /// <summary>
    /// Public method to show menu (can be called from other scripts)
    /// </summary>
    public void ShowMainMenu()
    {
        gameStarted = false;
        
        // Stop spawning
        if (spawnManager != null)
        {
            spawnManager.StopSpawning();
        }
        
        // Destroy any remaining cubes
        DestroyRemainingCubes();
        
        // Show menu
        ShowMenu();
    }
    
    /// <summary>
    /// Destroys any remaining moving cubes
    /// </summary>
    private void DestroyRemainingCubes()
    {
        GameObject[] movingCubes = GameObject.FindGameObjectsWithTag("MovingCube");
        foreach (GameObject cube in movingCubes)
        {
            Destroy(cube);
        }
        
        if (showDebug && movingCubes.Length > 0)
        {
            Debug.Log($"PTMenuManager: Destroyed {movingCubes.Length} remaining cubes");
        }
    }
    
    /// <summary>
    /// Check if game is currently started
    /// </summary>
    public bool IsGameStarted()
    {
        return gameStarted;
    }
    
    /// <summary>
    /// Handle pause functionality (optional)
    /// </summary>
    public void TogglePause()
    {
        if (gameStarted)
        {
            if (Time.timeScale == 0f)
            {
                // Resume
                HideMenu();
            }
            else
            {
                // Pause
                ShowMenu();
            }
        }
    }
    
    void Update()
    {
        // Optional: Handle ESC key to show/hide menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameStarted)
            {
                TogglePause();
            }
        }
    }
}