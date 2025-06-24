using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI titleText;
    
    [Header("Music")]
    public AudioClip backgroundMusicClip;
    
    [Header("Button Sounds")]
    public AudioClip buttonClickSound;
    
    [Header("Authentication Panels")]
    public GameObject loginPanel;
    public GameObject signupPanel;
    public GameObject gameChoosePanel;
    public GameObject loadingPanel;
    public GameObject errorPanel;
    
    [Header("Login Panel Elements")]
    public TMP_InputField loginEmailInput;
    public TMP_InputField loginPasswordInput;
    public Button loginSubmitButton;
    public Button loginToSignupButton;
    
    [Header("Signup Panel Elements")]
    public TMP_InputField signupEmailInput;
    public TMP_InputField signupPasswordInput;
    public Button signupSubmitButton;
    public Button signupToLoginButton;
    
    [Header("Game Choose Panel Elements")]
    public Button mahjongGameButton;
    public Button taichiGameButton;
    
    [Header("Loading Panel Elements")]
    public TextMeshProUGUI loadingText;
    
    [Header("Error Panel Elements")]
    public TextMeshProUGUI errorText;
    public Button errorRetryButton;
    
    [Header("Info Panel")]
    public Button infoButton;
    public GameObject instructionPanel;
    public Button gotItButton;
    
    [Header("Sound Toggle")]
    public Button soundToggleButton;
    public Sprite soundOnSprite;
    public Sprite soundOffSprite;
    
    // Private variables
    private AudioSource buttonAudioSource;
    private GameObject previousPanel;
    private bool isProcessing = false;
    
    void Start()
    {
        SetupAuthentication();
        SetupInfoButton();
        SetupButtonAudio();
        SetupSoundToggle();
        InitializeBackgroundMusic();
        
        // Show login panel first
        ShowLoginPanel();
    }
    
    void OnEnable()
    {
        // Subscribe to Firebase events
        FirebaseManager.OnAuthenticationResult += OnAuthenticationComplete;
        FirebaseManager.OnAuthenticationError += OnAuthenticationError;
        FirebaseManager.OnRegistrationResult += OnRegistrationComplete;
        FirebaseManager.OnRegistrationError += OnRegistrationError;
    }
    
    void OnDisable()
    {
        // Unsubscribe from Firebase events
        FirebaseManager.OnAuthenticationResult -= OnAuthenticationComplete;
        FirebaseManager.OnAuthenticationError -= OnAuthenticationError;
        FirebaseManager.OnRegistrationResult -= OnRegistrationComplete;
        FirebaseManager.OnRegistrationError -= OnRegistrationError;
    }
    
    #region Authentication Setup
    
    void SetupAuthentication()
    {
        // Setup login panel
        if (loginSubmitButton != null)
            loginSubmitButton.onClick.AddListener(OnLoginSubmit);
        if (loginToSignupButton != null)
            loginToSignupButton.onClick.AddListener(ShowSignupPanel);
        
        // Setup signup panel
        if (signupSubmitButton != null)
            signupSubmitButton.onClick.AddListener(OnSignupSubmit);
        if (signupToLoginButton != null)
            signupToLoginButton.onClick.AddListener(ShowLoginPanel);
        
        // Setup game choose panel
        if (mahjongGameButton != null)
            mahjongGameButton.onClick.AddListener(StartMahjongGame);
        if (taichiGameButton != null)
            taichiGameButton.onClick.AddListener(StartTaichiGame);
        
        // Setup error panel
        if (errorRetryButton != null)
            errorRetryButton.onClick.AddListener(OnErrorRetry);
        
        // Hide all panels initially
        HideAllPanels();
    }
    
    #endregion
    
    #region Panel Management
    
    void HideAllPanels()
    {
        if (loginPanel != null) loginPanel.SetActive(false);
        if (signupPanel != null) signupPanel.SetActive(false);
        if (gameChoosePanel != null) gameChoosePanel.SetActive(false);
        if (loadingPanel != null) loadingPanel.SetActive(false);
        if (errorPanel != null) errorPanel.SetActive(false);
        if (instructionPanel != null) instructionPanel.SetActive(false);
    }
    
    public void ShowLoginPanel()
    {
        PlayButtonSound();
        HideAllPanels();
        
        if (loginPanel != null)
        {
            loginPanel.SetActive(true);
            previousPanel = loginPanel;
            
            // Clear input fields
            if (loginEmailInput != null) loginEmailInput.text = "";
            if (loginPasswordInput != null) loginPasswordInput.text = "";
        }
    }
    
    public void ShowSignupPanel()
    {
        PlayButtonSound();
        HideAllPanels();
        
        if (signupPanel != null)
        {
            signupPanel.SetActive(true);
            previousPanel = signupPanel;
            
            // Clear input fields
            if (signupEmailInput != null) signupEmailInput.text = "";
            if (signupPasswordInput != null) signupPasswordInput.text = "";
        }
    }
    
    public void ShowGameChoosePanel()
    {
        HideAllPanels();
        
        if (gameChoosePanel != null)
        {
            gameChoosePanel.SetActive(true);
            previousPanel = gameChoosePanel;
        }
    }
    
    public void ShowLoadingPanel(string message = "Please wait...")
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
            if (loadingText != null)
                loadingText.text = message;
        }
    }
    
    public void HideLoadingPanel()
    {
        if (loadingPanel != null)
            loadingPanel.SetActive(false);
    }
    
    public void ShowErrorPanel(string errorMessage)
    {
        HideLoadingPanel();
        
        if (errorPanel != null)
        {
            errorPanel.SetActive(true);
            if (errorText != null)
                errorText.text = errorMessage;
        }
    }
    
    #endregion
    
    #region Authentication Methods
    
    public async void OnLoginSubmit()
    {
        if (isProcessing) return;
        
        PlayButtonSound();
        
        // Validate inputs
        string email = loginEmailInput?.text?.Trim();
        string password = loginPasswordInput?.text;
        
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowErrorPanel("Please enter both email and password");
            return;
        }
        
        if (!IsValidEmail(email))
        {
            ShowErrorPanel("Please enter a valid email address");
            return;
        }
        
        isProcessing = true;
        ShowLoadingPanel("Logging in...");
        
        // Attempt login through FirebaseManager
        if (FirebaseManager.Instance != null)
        {
            await FirebaseManager.Instance.LoginUser(email, password);
        }
        else
        {
            isProcessing = false;
            ShowErrorPanel("Firebase not initialized. Please restart the game.");
        }
    }
    
    public async void OnSignupSubmit()
    {
        if (isProcessing) return;
        
        PlayButtonSound();
        
        // Validate inputs
        string email = signupEmailInput?.text?.Trim();
        string password = signupPasswordInput?.text;
        
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowErrorPanel("Please enter both email and password");
            return;
        }
        
        if (!IsValidEmail(email))
        {
            ShowErrorPanel("Please enter a valid email address");
            return;
        }
        
        if (password.Length < 6)
        {
            ShowErrorPanel("Password must be at least 6 characters");
            return;
        }
        
        isProcessing = true;
        ShowLoadingPanel("Creating account...");
        
        // Attempt registration through FirebaseManager
        if (FirebaseManager.Instance != null)
        {
            await FirebaseManager.Instance.RegisterUser(email, password);
        }
        else
        {
            isProcessing = false;
            ShowErrorPanel("Firebase not initialized. Please restart the game.");
        }
    }
    
    #endregion
    
    #region Firebase Event Handlers
    
    void OnAuthenticationComplete(bool success)
    {
        isProcessing = false;
        HideLoadingPanel();
        
        if (success)
        {
            Debug.Log("✅ Authentication successful");
            ShowGameChoosePanel();
        }
    }
    
    void OnAuthenticationError(string error)
    {
        isProcessing = false;
        ShowErrorPanel(error);
    }
    
    void OnRegistrationComplete(bool success)
    {
        isProcessing = false;
        HideLoadingPanel();
        
        if (success)
        {
            Debug.Log("✅ Registration successful");
            ShowErrorPanel("Account created successfully!\nPlease login with your new account.");
            StartCoroutine(DelayedLoginPanelShow());
        }
    }
    
    void OnRegistrationError(string error)
    {
        isProcessing = false;
        ShowErrorPanel(error);
    }
    
    IEnumerator DelayedLoginPanelShow()
    {
        yield return new WaitForSeconds(2f);
        ShowLoginPanel();
    }
    
    #endregion
    
    #region Game Navigation
    
    public void StartMahjongGame()
    {
        PlayButtonSound();
        
        // Load the existing game scene (index 1)
        SceneManager.LoadScene(1);
    }
    
    public void StartTaichiGame()
    {
        PlayButtonSound();
        
        try
        {
            // Load the new Taichi game scene
            SceneManager.LoadScene("TaichiGame");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load TaichiGame scene: {e.Message}");
            ShowErrorPanel("TaichiGame scene not found. Please add it to Build Settings.");
        }
    }
    
    #endregion
    
    #region Error Handling
    
    public void OnErrorRetry()
    {
        PlayButtonSound();
        
        if (errorPanel != null)
            errorPanel.SetActive(false);
        
        // Return to previous panel
        if (previousPanel != null)
            previousPanel.SetActive(true);
    }
    
    #endregion
    
    #region Utility Methods
    
    bool IsValidEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            return false;
        
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
    
    #endregion
    
    #region Existing Methods (Keep as they were)
    
    void SetupInfoButton()
    {
        if (infoButton != null)
        {
            infoButton.onClick.AddListener(ShowInstructions);
        }
        
        if (gotItButton != null)
        {
            gotItButton.onClick.AddListener(HideInstructions);
        }
        
        if (instructionPanel != null)
        {
            instructionPanel.SetActive(false);
        }
    }
    
    public void ShowInstructions()
    {
        PlayButtonSound();
        
        if (instructionPanel != null)
        {
            instructionPanel.SetActive(true);
        }
    }
    
    public void HideInstructions()
    {
        PlayButtonSound();
        
        if (instructionPanel != null)
        {
            instructionPanel.SetActive(false);
        }
    }
    
    void SetupButtonAudio()
    {
        if (buttonClickSound != null)
        {
            buttonAudioSource = gameObject.AddComponent<AudioSource>();
            buttonAudioSource.clip = buttonClickSound;
            buttonAudioSource.volume = 0.7f;
            buttonAudioSource.playOnAwake = false;
        }
    }
    
    private void SetupSoundToggle()
    {
        if (soundToggleButton != null)
        {
            GlobalSoundToggle soundToggle = soundToggleButton.GetComponent<GlobalSoundToggle>();
            if (soundToggle == null)
            {
                soundToggle = soundToggleButton.gameObject.AddComponent<GlobalSoundToggle>();
            }
            
            soundToggle.soundButton = soundToggleButton;
            soundToggle.soundOnSprite = soundOnSprite;
            soundToggle.soundOffSprite = soundOffSprite;
            
            Debug.Log("✅ Sound toggle setup complete");
        }
    }
    
    void InitializeBackgroundMusic()
    {
        if (MusicManager.Instance == null && backgroundMusicClip != null)
        {
            GameObject musicManagerObj = new GameObject("MusicManager");
            MusicManager musicManager = musicManagerObj.AddComponent<MusicManager>();
            musicManager.backgroundMusic = backgroundMusicClip;
        }
    }
    
    public void QuitGame()
    {
        PlayButtonSound();
        
        Debug.Log("Quitting game...");
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    
    private void PlayButtonSound()
    {
        if (buttonAudioSource != null && buttonClickSound != null)
        {
            buttonAudioSource.PlayOneShot(buttonClickSound);
        }
    }
    
    #endregion
}