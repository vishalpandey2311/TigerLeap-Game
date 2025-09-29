using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PTGameManager : MonoBehaviour
{
    [Header("Rules")]
    [Tooltip("Allowed misses before losing (lose when misses > allowed)")]
    public int maxAllowedMisses = 5;

    [Header("References")]
    public PTSpawnManager spawnManager;
    public PTGateSpawnManager gateSpawnManager;
    public PTScoreManager scoreManager;
    public PTMenuManager menuManager;
    public PTGameOverUI gameOverUI;

    [Header("Debug")] 
    public bool showDebug = false;

    // State
    private int currentMisses = 0;
    private bool isGameOver = false;
    private bool uploadedThisRun = false;

    // Trigger timing
    private float lastTriggerTime = -1f;
    private float maxDelayBetweenTriggers = 0f;

    public static PTGameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this; else { Destroy(gameObject); return; }
    }

    void Start()
    {
        // Auto-wire common refs if missing
        if (spawnManager == null) spawnManager = PTSpawnManager.Instance ?? FindFirstObjectByType<PTSpawnManager>();
        if (scoreManager == null) scoreManager = PTScoreManager.Instance ?? FindFirstObjectByType<PTScoreManager>();
        if (menuManager == null) menuManager = FindFirstObjectByType<PTMenuManager>();
        if (gateSpawnManager == null) gateSpawnManager = FindFirstObjectByType<PTGateSpawnManager>();
        if (gameOverUI == null) gameOverUI = FindFirstObjectByType<PTGameOverUI>();

        if (gameOverUI != null)
        {
            gameOverUI.Hide();
            gameOverUI.onPlayAgain = PlayAgain;
            gameOverUI.onQuit = QuitToMenu;
        }
    }

    public void ResetRun()
    {
        currentMisses = 0;
        isGameOver = false;
        uploadedThisRun = false;
        lastTriggerTime = -1f;
        maxDelayBetweenTriggers = 0f;
    }

    // Call from CubeButton when a user triggers an input
    public void NotifyUserTrigger()
    {
        if (isGameOver) return;
        float now = Time.time;
        if (lastTriggerTime >= 0f)
        {
            float delta = now - lastTriggerTime;
            if (delta > maxDelayBetweenTriggers) maxDelayBetweenTriggers = delta;
        }
        lastTriggerTime = now;
    }

    // Call when a cube is missed
    public void NotifyMiss()
    {
        if (isGameOver) return;
        currentMisses++;
        if (showDebug) Debug.Log($"PTGameManager: Missed {currentMisses}/{maxAllowedMisses}");
        if (currentMisses > maxAllowedMisses)
        {
            TriggerGameOver();
        }
    }

    public void TriggerGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        // Stop spawning
        if (spawnManager != null) spawnManager.StopSpawning();
        if (gateSpawnManager != null) gateSpawnManager.StopSpawning();

        // Destroy remaining cubes and gates
        DestroyRemainingObjects();

        // Pause world
        Time.timeScale = 0f;

        // Show UI
        if (gameOverUI != null)
        {
            int score = scoreManager != null ? scoreManager.GetCurrentScore() : 0;
            gameOverUI.Show(score, currentMisses, maxDelayBetweenTriggers);
        }

        // Update Firebase
        StartCoroutine(UploadResultCoroutine(false));
    }

    private void DestroyRemainingObjects()
    {
        var cubes = GameObject.FindGameObjectsWithTag("MovingCube");
        foreach (var c in cubes) Destroy(c);
        if (gateSpawnManager != null) gateSpawnManager.DestroyAllGates();
    }

    private IEnumerator UploadResultCoroutine(bool quitting)
    {
        if (uploadedThisRun) yield break;
        uploadedThisRun = true;

        int score = scoreManager != null ? scoreManager.GetCurrentScore() : 0;

        if (FirebaseManager.Instance != null && FirebaseManager.Instance.isFirebaseInitialized)
        {
            FirebaseManager.Instance.SelectTaichiGame(); // ensure GM2
            var task = FirebaseManager.Instance.UpdateGM2Score(score, maxDelayBetweenTriggers);
            yield return new WaitUntil(() => task.IsCompleted);
        }

        if (quitting)
        {
            // Unpause before changing scenes
            Time.timeScale = 1f;
            // Prefer PTMenuManager if present, else load scene
            if (menuManager != null)
            {
                menuManager.ShowMainMenu();
            }
            else
            {
                try { SceneManager.LoadScene("MainMenu"); }
                catch { SceneManager.LoadScene(0); }
            }
        }
    }

    // UI hooks
    public void PlayAgain()
    {
        // Unpause
        Time.timeScale = 1f;

        // Reset state and score
        ResetRun();
        if (scoreManager != null) scoreManager.ResetScore();
        if (gameOverUI != null) gameOverUI.Hide();

        // Restart spawning
        if (spawnManager != null) spawnManager.StartSpawning();
        if (gateSpawnManager != null) gateSpawnManager.StartSpawning();
    }

    public void QuitToMenu()
    {
        StartCoroutine(UploadResultCoroutine(true));
    }
}
