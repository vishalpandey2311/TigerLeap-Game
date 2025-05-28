using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Singleton pattern for easy access
    public static GameManager Instance;
    
    // Reference to UI elements
    [Header("UI References")]
    public GameObject winPanel;
    public TextMeshProUGUI winText;
    public Button restartButton;
    public TextMeshProUGUI attemptsText;
    public GameObject attemptsPanel;
    public GameObject gameOverPanel;        // NEW: Game Over panel
    public TextMeshProUGUI gameOverText;    // NEW: Game Over text
    public Button gameOverRestartButton;    // NEW: Restart button in Game Over panel
    
    [Header("Audio")]
    public AudioClip correctMatchSound;   // Sound to play when correct match is found
    public AudioClip wrongMatchSound;     // Sound to play when incorrect match is selected
    public AudioClip gameWinSound;        // Sound to play when the game is won
    public AudioClip countdownMusic;      // Music to play during the countdown
    public AudioClip columnCompletedSound; // Sound to play when a column is completed
    public AudioClip loseGameSound;       // NEW: Sound to play when game is lost
    private AudioSource audioSource;      // Audio source component

    // Game state tracking
    private int totalMatchesNeeded = 0;  // Player hand cards * 4 = 12 matches needed
    private int currentMatches = 0;
    private int attempts = 0;  // Track number of attempts

    [Header("Collection Grid")]
    private bool[,] collectionGridFilled = new bool[4, 3]; // Track filled positions
    private CardSpawner cardSpawner; // Reference to the CardSpawner
    
    [Header("Countdown")]
    public TextMeshProUGUI countdownText;  // Reference to countdown text
    public float initialViewTime = 3f;     // Time to view cards at start

    [Header("Timer")]
    public TextMeshProUGUI timerText;
    public GameObject timerPanel;
    private float gameTimer = 60f;              // NEW: Start from 60 seconds
    private bool isTimerRunning = false;
    public TextMeshProUGUI finalTimeText;
    private float totalGameTime = 60f;          // NEW: Total time limit (1 minute)

    [Header("Completion Checkers")]
    public GameObject[] completionCheckers;  // Array of 3 checker icons
    private int[] matchesPerCard;            // Track matches found for each card type

    [Header("Pause Menu")]
    public Button pauseButton;           // Reference to the pause button
    public GameObject pausePanel;        // Reference to the panel that appears when game is paused
    public Button infoButton;           // NEW: Reference to info button in pause panel
    public Sprite pauseSprite;           // Pause icon sprite
    public Sprite resumeSprite;          // Resume/play icon sprite
    private bool isPaused = false;       // Track whether the game is currently paused
    private bool isInInstructionMode = false;  // NEW: Track if we're viewing instructions from pause

    // Replace UI checkers with 3D checkers
    [Header("3D Completion Checkers")]
    public GameObject checkerPrefab;           // Prefab for 3D checker signs
    private GameObject[] threeDCheckers;       // Array to store 3D checker instance

    [Header("Checker Position Offsets")]
    public float threeDCheckersOffsetX = 0;
    public float threeDCheckersOffsetY = 0;
    public float threeDCheckersOffsetZ = 0;

    [Header("Instructions")]
    public GameObject instructionPanel;      // Reference to instruction panel
    public Button gotItButton;              // Reference to "Got It" button
    public bool gameStarted = false;        // NEW: Track if game has actually started

    [Header("Difficulty Selection")]
    public GameObject difficultyPanel;      // Reference to difficulty selection panel
    public Button easyButton;              // Easy difficulty button (5 minutes)
    public Button intermediateButton;      // Intermediate difficulty button (3 minutes)
    public Button hardButton;              // Hard difficulty button (1 minute)
    private bool difficultySelected = false;  // Track if difficulty has been selected
    private float selectedGameTime = 60f;     // Will be set based on difficulty choice


    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        
        // Hide panels at start
        if (winPanel != null)
            winPanel.SetActive(false);
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        
        // Get or add audio source component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Setup game over button
        SetupGameOverButton();
        
        // NEW: Setup difficulty buttons
        SetupDifficultyButtons();
    }

    void Start()
    {
        // Get reference to CardSpawner
        cardSpawner = Object.FindFirstObjectByType<CardSpawner>();
        
        // Initialize collection grid
        collectionGridFilled = new bool[4, 3];
        
        // Initialize attempts counter
        attempts = 0;
        UpdateAttemptsUI();

        // Setup pause button
        SetupPauseButton();
        
        // Setup instruction panel
        SetupInstructionPanel();
        
        // NEW: Show difficulty selection first
        ShowDifficultySelection();
    }
    
    void Update()
    {
        // Only update timer if it's currently running
        if (isTimerRunning)
        {
            // Subtract time (countdown)
            gameTimer -= Time.deltaTime;
            
            // Check if time is up
            if (gameTimer <= 0f)
            {
                gameTimer = 0f;
                isTimerRunning = false;
                // Trigger Game Over
                StartCoroutine(ShowGameOver());
            }
            
            // Update the timer display
            UpdateTimerUI();
        }
    }

    // Method to update the timer UI with MM:SS format
    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            // Convert seconds to minutes and seconds
            int minutes = Mathf.FloorToInt(gameTimer / 60f);
            int seconds = Mathf.FloorToInt(gameTimer % 60f);
            
            // Format as MM:SS
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
    
    public void SetupGame(int playerHandCards)
    {
        // Reset game state
        gameStarted = false;
        
        // Reset attempts
        attempts = 0;
        UpdateAttemptsUI();
        
        // Set total matches needed
        totalMatchesNeeded = playerHandCards * 4;
        currentMatches = 0;
        
        // Use selected difficulty time (or default if not selected yet)
        if (difficultySelected)
        {
            gameTimer = selectedGameTime;
            totalGameTime = selectedGameTime;
        }
        else
        {
            gameTimer = totalGameTime; // Use default
        }
        
        isTimerRunning = false;
        UpdateTimerUI();
        
        if (finalTimeText != null)
        {
            finalTimeText.text = "";
        }
        
        // Hide win and game over panels at start
        if (winPanel != null)
            winPanel.SetActive(false);
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        
        // Initialize match counters for each card type
        matchesPerCard = new int[playerHandCards];
        for (int i = 0; i < matchesPerCard.Length; i++)
        {
            matchesPerCard[i] = 0;
        }
        
        // Make sure all checkers start inactive
        if (completionCheckers != null)
        {
            foreach (GameObject checker in completionCheckers)
            {
                if (checker != null)
                    checker.SetActive(false);
            }
        }

        // Initialize or clean up 3D checkers
        if (threeDCheckers != null)
        {
            foreach (var checker in threeDCheckers)
            {
                if (checker != null)
                    Destroy(checker);
            }
        }
        
        // Create new 3D checkers
        threeDCheckers = new GameObject[playerHandCards];
        
        // Find player hand cards to position checkers
        GameObject playerHandParent = GameObject.FindGameObjectWithTag("PlayerHand");
        if (playerHandParent != null && checkerPrefab != null)
        {
            int index = 0;
            foreach (Transform cardTransform in playerHandParent.transform)
            {
                if (index < playerHandCards)
                {
                    // Position checker above the card
                    Vector3 checkerPosition = cardTransform.position + new Vector3(threeDCheckersOffsetX, threeDCheckersOffsetY, threeDCheckersOffsetZ);
                    threeDCheckers[index] = Instantiate(checkerPrefab, checkerPosition, Quaternion.Euler(0, 0, 0));
                    threeDCheckers[index].SetActive(false); // Hide initially
                    index++;
                }
            }
        }
        
        // Hide UI checkers permanently since we're using 3D ones now
        if (completionCheckers != null)
        {
            foreach (GameObject checker in completionCheckers)
            {
                if (checker != null)
                    checker.SetActive(false);
            }
        }
    }
    
    // Call this when a card is flipped (attempts incremented)
    public void IncrementAttempts()
    {
        attempts++;
        UpdateAttemptsUI();
    }
    
    // Update the attempts UI element
    private void UpdateAttemptsUI()
    {
        if (attemptsText != null)
        {
            attemptsText.text = "Attempts: " + attempts;
        }
    }
    
    // Play sound for
    public void PlayCorrectMatchSound()
    {
        if (audioSource != null && correctMatchSound != null)
        {
            audioSource.PlayOneShot(correctMatchSound);
        }
    }
    
    // Play sound for wrong match
    public void PlayWrongMatchSound()
    {
        if (audioSource != null && wrongMatchSound != null)
        {
            audioSource.PlayOneShot(wrongMatchSound);
        }
    }
    
    // Play win sound
    public void PlayWinSound()
    {
        if (audioSource != null && gameWinSound != null)
        {
            audioSource.PlayOneShot(gameWinSound);
        }
    }
    
    // Play column completed sound
    public void PlayColumnCompletedSound()
    {
        if (audioSource != null && columnCompletedSound != null)
        {
            audioSource.PlayOneShot(columnCompletedSound);
        }
    }
    
    // NEW: Play lose game sound
    public void PlayLoseSound()
    {
        if (audioSource != null && loseGameSound != null)
        {
            audioSource.PlayOneShot(loseGameSound);
        }
    }
    
    public void CardMatched(int cardTypeId, CardController matchedCard)
    {
        currentMatches++;
        
        // Play correct match sound
        PlayCorrectMatchSound();
        
        // Find next available position in collection grid
        Vector3 targetPosition = FindCollectionPosition(cardTypeId);
        
        // Move card to collection grid
        matchedCard.MoveToCollectionGrid(targetPosition);
        
        // Find which player hand card matched with this card type
        int handIndex = -1;
        for (int i = 0; i < cardSpawner.GetPlayerHandIndices().Count; i++)
        {
            if (cardSpawner.GetPlayerHandIndices()[i] == cardTypeId)
            {
                handIndex = i;
                break;
            }
        }
        
        // Update the match counter for this card type
        if (handIndex >= 0 && handIndex < matchesPerCard.Length)
        {
            matchesPerCard[handIndex]++;
            
            // Check if all 4 matches have been found for this card type
            if (matchesPerCard[handIndex] >= 4 && completionCheckers != null 
                && handIndex < completionCheckers.Length)
            {
                ActivateCheckerWithEffect(handIndex);
            }
        }
        
        // Check if all cards are matched
        if (currentMatches >= totalMatchesNeeded)
        {
            StartCoroutine(ShowWinCelebration());
        }
    }

    // Find an available position in the collection grid
    Vector3 FindCollectionPosition(int cardTypeId)
    {
        // Access the positions from the CardSpawner
        if (cardSpawner != null)
        {
            // Find which player hand card this matched
            int handIndex = -1;
            for (int i = 0; i < cardSpawner.GetPlayerHandIndices().Count; i++)
            {
                if (cardSpawner.GetPlayerHandIndices()[i] == cardTypeId)
                {
                    handIndex = i;
                    break;
                }
            }
            
            if (handIndex >= 0 && handIndex < 3)
            {
                // Find first open row in this column
                for (int row = 0; row < 4; row++)
                {
                    if (!collectionGridFilled[row, handIndex])
                    {
                        // Mark as filled and return position
                        collectionGridFilled[row, handIndex] = true;
                        return cardSpawner.GetCollectionGridPosition(row, handIndex);
                    }
                }
            }
        }
        
        // Fallback position if no space found
        return new Vector3(0, 3, 0);
    }
    
    IEnumerator ShowWinCelebration()
    {
        // Stop the timer
        isTimerRunning = false;
        
        // Calculate time taken (60 - remaining time)
        float timeTaken = totalGameTime - gameTimer;
        
        // Hide pause button when game ends
        if (pauseButton != null)
        {
            pauseButton.gameObject.SetActive(false);
        }
        
        
        // Wait a moment before showing win screen
        yield return new WaitForSeconds(1f);
        
        // Play win sound
        PlayWinSound();
        
        // Hide the attempts panel since we'll show attempts in the win panel
        if (attemptsPanel != null)
        {
            attemptsPanel.SetActive(false);
        }
        
        // Hide the timer panel
        if (timerPanel != null)
        {
            timerPanel.SetActive(false);
        }
        
        // Hide all checker icons
        if (completionCheckers != null)
        {
            foreach (GameObject checker in completionCheckers)
            {
                if (checker != null)
                    checker.SetActive(false);
            }
        }
        
        // Hide 3D checker icons
        if (threeDCheckers != null)
        {
            foreach (GameObject checker in threeDCheckers)
            {
                if (checker != null)
                    checker.SetActive(false);
            }
        }
        
        // Show win panel
        if (winPanel != null)
        {
            winPanel.SetActive(true);
            
            // Set win text to include time taken
            if (winText != null)
            {
                int minutes = Mathf.FloorToInt(timeTaken / 60f);
                int seconds = Mathf.FloorToInt(timeTaken % 60f);
                winText.text = string.Format("YOU WIN\n");
            }
            
            // Optional animations
            // RectTransform panelRect = winPanel.GetComponent<RectTransform>();
            // if (panelRect != null)
            // {
            //     panelRect.localScale = Vector3.zero;
                
            //     // Animate panel scaling up
            //     float duration = 0.5f;
            //     float elapsed = 0f;
                
            //     while (elapsed < duration)
            //     {
            //         elapsed += Time.deltaTime;
            //         float t = Mathf.SmoothStep(0, 1, elapsed / duration);
            //         panelRect.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            //         yield return null;
            //     }
                
            //     panelRect.localScale = Vector3.one;
            // }
            
            // Optional: play sound effect
            AudioSource audio = GetComponent<AudioSource>();
            if (audio != null)
                audio.Play();
        }
    }
    
    public void RestartGame()
    {
        // Reload the current scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
    
    // New method to start and handle the countdown
    public void StartInitialCountdown()
    {
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
            
            // Play countdown music using AudioManager
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.Play("countdown"); // Make sure you have a sound named "countdown" in AudioManager
            }
            // Fallback to direct AudioSource if AudioManager isn't available
            else if (audioSource != null && countdownMusic != null)
            {
                audioSource.clip = countdownMusic;
                audioSource.loop = true;
                audioSource.Play();
            }
            
            StartCoroutine(CountdownRoutine());
        }
    }
    
    // Coroutine to handle the countdown
    private IEnumerator CountdownRoutine()
    {
        // Hide pause button during countdown
        if (pauseButton != null)
        {
            pauseButton.gameObject.SetActive(false);
        }

        int countdownValue = Mathf.CeilToInt(initialViewTime);
        
        while (countdownValue > 0)
        {
            countdownText.text = "Memorize the cards: " + countdownValue;
            yield return new WaitForSeconds(1f);
            countdownValue--;
        }
        
        countdownText.text = "Go!";
        
        // Fade out the "Go!" message
        float fadeDuration = 0.5f;
        Color originalColor = countdownText.color;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            countdownText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            
            // Fade out audio
            if (AudioManager.Instance != null)
            {
                float currentVolume = AudioManager.Instance.masterVolume;
                AudioManager.Instance.SetVolume(currentVolume * (1f - (elapsed / fadeDuration)));
            }
            else if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.volume = 1f - (elapsed / fadeDuration);
            }
            
            yield return null;
        }

    // Stop the music
    if (AudioManager.Instance != null)
    {
        AudioManager.Instance.Stop("countdown");
        AudioManager.Instance.SetVolume(1f);
    }
    else if (audioSource != null && audioSource.clip == countdownMusic)
    {
        audioSource.Stop();
        audioSource.volume = 1f;
    }

    // NOW start the countdown timer using selected difficulty time
    gameTimer = totalGameTime; // This will be the selected difficulty time
    isTimerRunning = true;
    UpdateTimerUI();

    // Show pause button now that gameplay is starting
    if (pauseButton != null)
    {
        pauseButton.gameObject.SetActive(true);
    }

    // Hide countdown text after it's faded
    countdownText.gameObject.SetActive(false);
}
    // Add this method to create a visual effect when activating a checker
    private void ActivateCheckerWithEffect(int index)
    {
        // Don't use UI checkers anymore
        // if (completionCheckers == null || index < 0 || index >= completionCheckers.Length)
        //    return;
        
        // Instead use 3D checkers
        if (threeDCheckers == null || index < 0 || index >= threeDCheckers.Length)
            return;
            
        GameObject checker = threeDCheckers[index];
        if (checker == null)
            return;
            
        // Activate the checker
        checker.SetActive(true);
        
        // Play the column completed sound
        PlayColumnCompletedSound();
        
        // Animate the 3D checker
        StartCoroutine(Animate3DCheckerActivation(checker.transform));
    }

    // New method for 3D checker animation
    private IEnumerator Animate3DCheckerActivation(Transform checkerTransform)
    {
        // Store original scale and position
        Vector3 originalScale = checkerTransform.localScale;
        Vector3 originalPosition = checkerTransform.position;
        
        // Pop effect - scale up
        float duration = 0.3f;
        float elapsed = 0f;
        
        // Scale up
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);
            checkerTransform.localScale = Vector3.Lerp(originalScale, originalScale * 1.5f, t);
            // Optional: Add a small upward movement
            checkerTransform.position = Vector3.Lerp(originalPosition, 
                                                    originalPosition + new Vector3(0, 0.1f, 0), t);
            yield return null;
        }
        
        // Scale back to original
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);
            checkerTransform.localScale = Vector3.Lerp(originalScale * 1.5f, originalScale, t);
            checkerTransform.position = Vector3.Lerp(originalPosition + new Vector3(0, 0.1f, 0), 
                                                   originalPosition, t);
            yield return null;
        }
        
        // Ensure final state is correct
        checkerTransform.localScale = originalScale;
        checkerTransform.position = originalPosition;
    }

    // Initialize the pause button in your Start or Awake method
    private void SetupPauseButton()
    {
        if (pauseButton != null)
        {
            // Initially set to pause icon
            pauseButton.GetComponent<Image>().sprite = pauseSprite;
            
            // Add click listener
            pauseButton.onClick.AddListener(TogglePause);
            
            // Initialize pause panel state
            if (pausePanel != null)
            {
                pausePanel.SetActive(false);
            }
        }
        
        // NEW: Setup info button
        if (infoButton != null)
        {
            infoButton.onClick.AddListener(ShowInstructionsFromPause);
        }
    }

    // Toggle between paused and unpaused states
    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    // Pauses the game
    public void PauseGame()
    {
        // Only pause if game is actually running
        if (isTimerRunning)
        {
            // Set pause state
            isPaused = true;
            
            // Stop the timer
            isTimerRunning = false;
            
            // Change button icon to resume/play
            if (pauseButton != null)
            {
                pauseButton.GetComponent<Image>().sprite = resumeSprite;
            }
            
            // Show pause panel
            if (pausePanel != null)
            {
                pausePanel.SetActive(true);
            }
            
            // Freeze the game time
            Time.timeScale = 0f;
        }
    }

    // Resumes the game
    public void ResumeGame()
    {
        // Set unpaused state
        isPaused = false;
        
        // Restart the timer
        if (!isTimerRunning)
        {
            isTimerRunning = true;
        }
        
        // Change button icon back to pause
        if (pauseButton != null)
        {
            pauseButton.GetComponent<Image>().sprite = pauseSprite;
        }
        
        // Hide pause panel
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        
        // Unfreeze game time
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Quits the game when called from the quit button in the pause menu
    /// </summary>
    public void QuitGame()
    {
        // Add any cleanup or save operations here before quitting
        
        // Log message (visible in the editor console)
        Debug.Log("Quitting game...");
        
        // In the editor, this will stop play mode
        // In a build, this will quit the application
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Setup instruction panel
    private void SetupInstructionPanel()
    {
        if (gotItButton != null)
        {
            gotItButton.onClick.AddListener(OnGotItButtonClicked);
        }
        
        if (instructionPanel != null)
        {
            instructionPanel.SetActive(false);
        }
    }

    // NEW: Show instructions from pause menu
    public void ShowInstructionsFromPause()
    {
        if (instructionPanel != null)
        {
            // Hide pause panel
            pausePanel.SetActive(false);
            
            // Show instruction panel
            instructionPanel.SetActive(true);
            
            // Set flag that we came from pause
            isInInstructionMode = true;
        }
    }

    // Modified: Called when "Got It" button is clicked
    public void OnGotItButtonClicked()
    {
        if (instructionPanel != null)
        {
            instructionPanel.SetActive(false);
        }
        
        // Check if we came from pause menu
        if (isInInstructionMode)
        {
            // Return to pause panel instead of resuming game
            if (pausePanel != null)
            {
                pausePanel.SetActive(true);
            }
            
            // Reset the flag
            isInInstructionMode = false;
            
            // FIX: Ensure the game remains paused
            isPaused = true;
            isTimerRunning = false;
            Time.timeScale = 0f;
            
            // Make sure pause button shows resume icon
            if (pauseButton != null)
            {
                pauseButton.GetComponent<Image>().sprite = resumeSprite;
            }
        }
        else
        {
            // Show UI elements
            if (timerPanel != null)
                timerPanel.SetActive(true);
            if (attemptsPanel != null)
                attemptsPanel.SetActive(true);
            
            // Start the game after instructions are dismissed
            StartGameAfterInstructions();
        }
    }

    // Keep this method but it won't be used in normal flow anymore
    private void StartGameAfterInstructions()
    {
        // Signal that the game can now start
        gameStarted = true;
        
        // Reset timer but DON'T start it yet
        gameTimer = 0f;
        isTimerRunning = false;  // Keep timer stopped during countdown
        UpdateTimerUI();
        
        // The cards will now start their initialization automatically
        // No need to call StartInitialCountdown() here as CardController will handle it
    }

    // NEW: Game Over when time runs out
    IEnumerator ShowGameOver()
    {
        // Stop the timer
        isTimerRunning = false;
        
        // Hide pause button when game ends
        if (pauseButton != null)
        {
            pauseButton.gameObject.SetActive(false);
        }
        
        // Wait a moment before showing game over screen
        yield return new WaitForSeconds(1f);
        
        // NEW: Play lose game sound
        PlayLoseSound();
        
        // Hide the attempts and timer panels
        if (attemptsPanel != null)
        {
            attemptsPanel.SetActive(false);
        }
        
        if (timerPanel != null)
        {
            timerPanel.SetActive(false);
        }
        
        // Hide all checker icons
        if (completionCheckers != null)
        {
            foreach (GameObject checker in completionCheckers)
            {
                if (checker != null)
                    checker.SetActive(false);
            }
        }
        
        if (threeDCheckers != null)
        {
            foreach (GameObject checker in threeDCheckers)
            {
                if (checker != null)
                    checker.SetActive(false);
            }
        }
        
        // Show game over panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
            // Set game over text
            if (gameOverText != null)
            {
                gameOverText.text = "GAME OVER\nAttempts: " + attempts + 
                                   "\nMatches: " + currentMatches + "/" + totalMatchesNeeded;
            }
        }
    }

    // Setup game over button
    private void SetupGameOverButton()
    {
        if (gameOverRestartButton != null)
        {
            gameOverRestartButton.onClick.AddListener(RestartGame);
        }
    }

    // NEW: Resume game from instructions panel
    public void ResumeFromInstructions()
    {
        if (instructionPanel != null)
        {
            instructionPanel.SetActive(false);
        }
        
        isInInstructionMode = false;
        
        // Show UI elements
        if (timerPanel != null)
            timerPanel.SetActive(true);
        if (attemptsPanel != null)
            attemptsPanel.SetActive(true);
        
        // Restart the game timer
        gameTimer = 0f;
        isTimerRunning = false;  // Keep timer stopped during countdown
        UpdateTimerUI();
    }

    // Setup difficulty selection buttons
    private void SetupDifficultyButtons()
    {
        if (easyButton != null)
        {
            easyButton.onClick.AddListener(() => SelectDifficulty(300f)); // 5 minutes = 300 seconds
        }
        
        if (intermediateButton != null)
        {
            intermediateButton.onClick.AddListener(() => SelectDifficulty(180f)); // 3 minutes = 180 seconds
        }
        
        if (hardButton != null)
        {
            hardButton.onClick.AddListener(() => SelectDifficulty(60f)); // 1 minute = 60 seconds
        }
    }

    // Show difficulty selection panel
    private void ShowDifficultySelection()
    {
        if (difficultyPanel != null)
        {
            difficultyPanel.SetActive(true);
        }
        
        // Hide other UI elements until difficulty is selected
        if (timerPanel != null)
            timerPanel.SetActive(false);
        if (attemptsPanel != null)
            attemptsPanel.SetActive(false);
        if (pauseButton != null)
            pauseButton.gameObject.SetActive(false);
    }

    // Called when player selects a difficulty
    public void SelectDifficulty(float timeInSeconds)
    {
        // Store the selected time
        selectedGameTime = timeInSeconds;
        totalGameTime = timeInSeconds;
        
        // Mark difficulty as selected
        difficultySelected = true;
        
        // Hide difficulty panel
        if (difficultyPanel != null)
        {
            difficultyPanel.SetActive(false);
        }
        
        // Now start the game
        StartGameAfterDifficultySelection();
    }

    // Start game after difficulty selection
    private void StartGameAfterDifficultySelection()
    {
        // Signal that the game can start
        gameStarted = true;
        
        // Show UI elements
        if (timerPanel != null)
            timerPanel.SetActive(true);
        if (attemptsPanel != null)
            attemptsPanel.SetActive(true);
        
        // Set timer to selected difficulty time but don't start it yet (countdown will handle this)
        gameTimer = totalGameTime;
        isTimerRunning = false;
        UpdateTimerUI();
        
        // The CardSpawner should already be initialized, so cards will start their countdown
    }
}