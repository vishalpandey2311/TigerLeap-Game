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
    
    [Tooltip("Cross/X resume button for paused game")]
    public Button crossResumeButton;
    
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
    
    [Header("Gate System")]
    [Tooltip("Reference to PTGateSpawnManager")]
    public PTGateSpawnManager gateSpawnManager;
    
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
        
        if (crossResumeButton != null)
        {
            crossResumeButton.onClick.AddListener(OnCrossResumeButtonClicked);
            // Hide cross resume button initially (game hasn't started yet)
            crossResumeButton.gameObject.SetActive(false);
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
            
            // Show cross resume button only if game has started (paused state)
            if (crossResumeButton != null && gameStarted)
            {
                crossResumeButton.gameObject.SetActive(true);
            }
            
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
            
            // Hide cross resume button when menu is hidden
            if (crossResumeButton != null)
            {
                crossResumeButton.gameObject.SetActive(false);
            }
            
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
            Debug.Log("PTMenuManager: Quit button clicked - Loading MainMenu");
        }
        
        LoadMainMenu();
    }
    
    /// <summary>
    /// Called when Cross Resume button is clicked
    /// </summary>
    public void OnCrossResumeButtonClicked()
    {
        PlayButtonSound();
        
        if (showDebug)
        {
            Debug.Log("PTMenuManager: Cross Resume button clicked - Resuming game");
        }
        
        ResumeGame();
    }
    
    /// <summary>
    /// Starts the game
    /// </summary>
    private void StartGame()
    {
        gameStarted = true;
    // Ensure GM2 is selected for Firebase writes
    if (FirebaseManager.Instance != null) FirebaseManager.Instance.SelectTaichiGame();
    // Reset game manager state
    if (PTGameManager.Instance != null) PTGameManager.Instance.ResetRun();
        
        // Reset score for new game
        if (scoreManager != null)
        {
            scoreManager.ResetScore();
        }
        else if (PTScoreManager.Instance != null)
        {
            PTScoreManager.Instance.ResetScore();
        }
        
        // Hide the cross resume button when starting the game
        if (crossResumeButton != null)
        {
            crossResumeButton.gameObject.SetActive(false);
        }
        
        // Hide the menu
    HideMenu();
    Time.timeScale = 1f;
        
        // Start spawning cubes
        if (spawnManager != null)
        {
            spawnManager.StartSpawning();
        }
        
        // Start spawning gates
        if (gateSpawnManager != null)
        {
            gateSpawnManager.StartSpawning();
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
    /// Loads the MainMenu scene
    /// </summary>
    private void LoadMainMenu()
    {
        if (showDebug)
        {
            Debug.Log("PTMenuManager: Loading MainMenu scene...");
        }
        
        // Stop any ongoing game processes
        if (spawnManager != null)
        {
            spawnManager.StopSpawning();
        }
        
        if (gateSpawnManager != null)
        {
            gateSpawnManager.StopSpawning();
        }
        
        // Destroy remaining objects
        DestroyRemainingCubes();
        DestroyRemainingGates();
        
        // Reset time scale
        Time.timeScale = 1f;
        
        // Load MainMenu scene
        try
        {
            SceneManager.LoadScene("MainMenu");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"PTMenuManager: Failed to load MainMenu scene: {e.Message}");
            // Fallback: try loading by index
            SceneManager.LoadScene(0);
        }
    }
    
    /// <summary>
    /// Resumes the game (hides menu and continues gameplay)
    /// </summary>
    private void ResumeGame()
    {
        if (showDebug)
        {
            Debug.Log("PTMenuManager: Resuming game...");
        }
        
        // Hide the menu to resume game
        HideMenu();
    }
    
    /// <summary>
    /// Quits the game (deprecated - now redirects to main menu)
    /// </summary>
    private void QuitGame()
    {
        if (showDebug)
        {
            Debug.Log("PTMenuManager: QuitGame called - redirecting to LoadMainMenu...");
        }
        
        LoadMainMenu();
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
        
        // Stop gate spawning
        if (gateSpawnManager != null)
        {
            gateSpawnManager.StopSpawning();
        }
        
        // Destroy any remaining cubes and gates
        DestroyRemainingCubes();
        DestroyRemainingGates();
        
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
    /// Destroys any remaining decorative gates
    /// </summary>
    private void DestroyRemainingGates()
    {
        if (gateSpawnManager != null)
        {
            gateSpawnManager.DestroyAllGates();
        }
        
        // Also clean up any gates that might not be tracked
        GameObject[] decorativeGates = GameObject.FindGameObjectsWithTag("DecorativeGate");
        foreach (GameObject gate in decorativeGates)
        {
            Destroy(gate);
        }
        
        if (showDebug && decorativeGates.Length > 0)
        {
            Debug.Log($"PTMenuManager: Destroyed {decorativeGates.Length} remaining gates");
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