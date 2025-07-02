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
    public Button forgotPasswordButton; // NEW: Forgot Password button

    [Header("Signup Panel Elements")]
    public TMP_InputField signupEmailInput;
    public TMP_InputField signupPasswordInput;
    public Button signupSubmitButton;
    public Button signupToLoginButton;

    [Header("Email Check Panel Elements")] // NEW: Email Check Panel
    public GameObject emailCheckPanel;
    public TMP_InputField emailCheckInput;
    public Button emailVerifyButton;
    public TextMeshProUGUI verifyText;
    public Button backFromEmailCheckButton;

    [Header("Reset Password Panel Elements")] // NEW: Reset Password Panel
    public GameObject resetPasswordPanel;
    public TMP_InputField newPasswordInput;
    public TMP_InputField confirmPasswordInput;
    public Button confirmPasswordButton;
    public Button backFromResetPasswordButton;

    [Header("Game Choose Panel Elements")]
    public Button mahjongGameButton;
    public Button taichiGameButton;

    [Header("Game Start Panel Elements")]
    public GameObject gameStartPanel;
    public Button backFromGameStartButton;

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

    [Header("Logout")]
    public Button logoutButton;

    [Header("Session Info (Debug)")]
    public TextMeshProUGUI sessionInfoText;

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

        // Check session status and show appropriate panel
        CheckSessionAndShowPanel();
    }

    void OnEnable()
    {
        // Subscribe to Firebase events
        FirebaseManager.OnAuthenticationResult += OnAuthenticationComplete;
        FirebaseManager.OnAuthenticationError += OnAuthenticationError;
        FirebaseManager.OnRegistrationResult += OnRegistrationComplete;
        FirebaseManager.OnRegistrationError += OnRegistrationError;
        FirebaseManager.OnSessionExpired += OnSessionExpired;
        FirebaseManager.OnPasswordResetEmailSent += OnPasswordResetEmailSent; // NEW
        FirebaseManager.OnPasswordResetError += OnPasswordResetError; // NEW
        FirebaseManager.OnPasswordUpdateResult += OnPasswordUpdateResult; // NEW
    }

    void OnDisable()
    {
        // Unsubscribe from Firebase events
        FirebaseManager.OnAuthenticationResult -= OnAuthenticationComplete;
        FirebaseManager.OnAuthenticationError -= OnAuthenticationError;
        FirebaseManager.OnRegistrationResult -= OnRegistrationComplete;
        FirebaseManager.OnRegistrationError -= OnRegistrationError;
        FirebaseManager.OnSessionExpired -= OnSessionExpired;
        FirebaseManager.OnPasswordResetEmailSent -= OnPasswordResetEmailSent; // NEW
        FirebaseManager.OnPasswordResetError -= OnPasswordResetError; // NEW
        FirebaseManager.OnPasswordUpdateResult -= OnPasswordUpdateResult; // NEW
    }

    // NEW: Check session and show appropriate panel
    void CheckSessionAndShowPanel()
    {
        if (FirebaseManager.Instance != null && FirebaseManager.Instance.IsUserLoggedIn())
        {
            Debug.Log("✅ User has valid session, showing game choose panel");
            ShowGameChoosePanel();
            UpdateSessionInfo();
        }
        else
        {
            Debug.Log("ℹ️ No valid session, showing login panel");
            ShowLoginPanel();
        }
    }

    // NEW: Update session info display (optional)
    void UpdateSessionInfo()
    {
        if (sessionInfoText != null && FirebaseManager.Instance != null)
        {
            sessionInfoText.text = FirebaseManager.Instance.GetSessionInfo();
        }
    }

    // NEW: Handle session expired event
    void OnSessionExpired()
    {
        Debug.Log("⚠️ Session expired, returning to login");
        ShowLoginPanel();
        ShowErrorPanel("Your session has expired. Please login again.");
    }

    #region Authentication Setup

    void SetupAuthentication()
    {
        // Setup login panel
        if (loginSubmitButton != null)
            loginSubmitButton.onClick.AddListener(OnLoginSubmit);
        if (loginToSignupButton != null)
            loginToSignupButton.onClick.AddListener(ShowSignupPanel);
        if (forgotPasswordButton != null) // NEW
            forgotPasswordButton.onClick.AddListener(ShowEmailCheckPanel);

        // Setup signup panel
        if (signupSubmitButton != null)
            signupSubmitButton.onClick.AddListener(OnSignupSubmit);
        if (signupToLoginButton != null)
            signupToLoginButton.onClick.AddListener(ShowLoginPanel);

        // NEW: Setup email check panel
        if (emailVerifyButton != null)
            emailVerifyButton.onClick.AddListener(OnEmailVerifySubmit);
        if (backFromEmailCheckButton != null)
            backFromEmailCheckButton.onClick.AddListener(ShowLoginPanel);

        // NEW: Setup reset password panel
        if (confirmPasswordButton != null)
            confirmPasswordButton.onClick.AddListener(OnConfirmPasswordSubmit);
        if (backFromResetPasswordButton != null)
            backFromResetPasswordButton.onClick.AddListener(ShowLoginPanel);

        // Setup game choose panel
        if (mahjongGameButton != null)
        {
            mahjongGameButton.onClick.AddListener(() =>
            {
                if (FirebaseManager.Instance != null)
                    FirebaseManager.Instance.SelectMahjongGame();
                ShowGameStartPanel();
            });
        }
        if (taichiGameButton != null)
        {
            taichiGameButton.onClick.AddListener(() =>
            {
                if (FirebaseManager.Instance != null)
                    FirebaseManager.Instance.SelectTaichiGame();
                StartTaichiGame();
            });
        }

        // Setup game start panel
        if (backFromGameStartButton != null)
            backFromGameStartButton.onClick.AddListener(ShowGameChoosePanel);

        // Setup error panel
        if (errorRetryButton != null)
            errorRetryButton.onClick.AddListener(OnErrorRetry);

        // Setup logout button
        if (logoutButton != null)
            logoutButton.onClick.AddListener(OnLogoutButtonClick);

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
        if (gameStartPanel != null) gameStartPanel.SetActive(false);
        if (loadingPanel != null) loadingPanel.SetActive(false);
        if (errorPanel != null) errorPanel.SetActive(false);
        if (instructionPanel != null) instructionPanel.SetActive(false);
        if (emailCheckPanel != null) emailCheckPanel.SetActive(false); // NEW
        if (resetPasswordPanel != null) resetPasswordPanel.SetActive(false); // NEW
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

    // NEW: Show Email Check Panel
    public void ShowEmailCheckPanel()
    {
        PlayButtonSound();
        HideAllPanels();

        if (emailCheckPanel != null)
        {
            emailCheckPanel.SetActive(true);
            previousPanel = emailCheckPanel;

            // Clear and reset email check panel
            if (emailCheckInput != null) emailCheckInput.text = "";
            if (verifyText != null) verifyText.text = "";
            
            // Make sure input and button are visible initially
            if (emailCheckInput != null) emailCheckInput.gameObject.SetActive(true);
            if (emailVerifyButton != null) emailVerifyButton.gameObject.SetActive(true);
        }
    }

    // NEW: Show Reset Password Panel
    public void ShowResetPasswordPanel()
    {
        PlayButtonSound();
        HideAllPanels();

        if (resetPasswordPanel != null)
        {
            resetPasswordPanel.SetActive(true);
            previousPanel = resetPasswordPanel;

            // Clear input fields
            if (newPasswordInput != null) newPasswordInput.text = "";
            if (confirmPasswordInput != null) confirmPasswordInput.text = "";
        }
    }

    public void ShowGameChoosePanel()
    {
        PlayButtonSound();
        HideAllPanels();

        if (gameChoosePanel != null)
        {
            gameChoosePanel.SetActive(true);
            previousPanel = gameChoosePanel;
            
            // Update session info when showing game choose panel
            UpdateSessionInfo();
        }
    }

    public void ShowGameStartPanel()
    {
        PlayButtonSound();
        HideAllPanels();

        if (gameStartPanel != null)
        {
            gameStartPanel.SetActive(true);
            previousPanel = gameStartPanel;
            Debug.Log("✅ Game Start Panel shown - Player can now configure and start Mahjong game");
        }
        else
        {
            Debug.LogWarning("⚠️ Game Start Panel not assigned. Falling back to direct scene load.");
            StartMahjongGameDirect();
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

    // NEW: Handle email verification for password reset
    public async void OnEmailVerifySubmit()
    {
        if (isProcessing) return;

        PlayButtonSound();

        string email = emailCheckInput?.text?.Trim();

        if (string.IsNullOrEmpty(email))
        {
            ShowErrorPanel("Please enter your email address");
            return;
        }

        if (!IsValidEmail(email))
        {
            ShowErrorPanel("Please enter a valid email address");
            return;
        }

        isProcessing = true;
        ShowLoadingPanel("Checking email...");

        // Check if email exists and send reset email
        if (FirebaseManager.Instance != null)
        {
            await FirebaseManager.Instance.SendPasswordResetEmail(email);
        }
        else
        {
            isProcessing = false;
            ShowErrorPanel("Firebase not initialized. Please restart the game.");
        }
    }

    // NEW: Handle password confirmation
    public async void OnConfirmPasswordSubmit()
    {
        if (isProcessing) return;

        PlayButtonSound();

        string newPassword = newPasswordInput?.text;
        string confirmPassword = confirmPasswordInput?.text;

        if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
        {
            ShowErrorPanel("Please fill in both password fields");
            return;
        }

        if (newPassword.Length < 6)
        {
            ShowErrorPanel("Password must be at least 6 characters");
            return;
        }

        if (newPassword != confirmPassword)
        {
            ShowErrorPanel("Passwords do not match");
            return;
        }

        isProcessing = true;
        ShowLoadingPanel("Updating password...");

        // Update password through FirebaseManager
        if (FirebaseManager.Instance != null)
        {
            await FirebaseManager.Instance.UpdateUserPassword(newPassword);
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

    // NEW: Handle password reset email sent
    void OnPasswordResetEmailSent(bool success)
    {
        isProcessing = false;
        HideLoadingPanel();

        if (success)
        {
            // Hide input field and button, show verification message
            if (emailCheckInput != null) emailCheckInput.gameObject.SetActive(false);
            if (emailVerifyButton != null) emailVerifyButton.gameObject.SetActive(false);
            if (verifyText != null)
            {
                verifyText.text = "Check your email and verify, then come back.";
                verifyText.gameObject.SetActive(true);
            }

            // Start checking for email verification
            StartCoroutine(CheckForEmailVerification());
        }
    }

    // NEW: Handle password reset error
    void OnPasswordResetError(string error)
    {
        isProcessing = false;
        ShowErrorPanel(error);
    }

    // NEW: Handle password update result
    void OnPasswordUpdateResult(bool success, string message)
    {
        isProcessing = false;
        HideLoadingPanel();

        if (success)
        {
            ShowErrorPanel("Password updated successfully!\nPlease login with your new password.");
            StartCoroutine(DelayedLoginPanelShow());
        }
        else
        {
            ShowErrorPanel(message);
        }
    }

    // NEW: Check for email verification periodically
    IEnumerator CheckForEmailVerification()
    {
        while (emailCheckPanel != null && emailCheckPanel.activeInHierarchy)
        {
            yield return new WaitForSeconds(2f); // Check every 2 seconds

            if (FirebaseManager.Instance != null)
            {
                // Start the async operation as a coroutine instead of using await
                StartCoroutine(CheckEmailVerificationAsync());
            }
        }
    }

    // Helper method to handle the async operation
    IEnumerator CheckEmailVerificationAsync()
    {
        var task = FirebaseManager.Instance.CheckEmailVerification();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Result)
        {
            Debug.Log("✅ Email verified, showing reset password panel");
            ShowResetPasswordPanel();
        }
    }

    IEnumerator DelayedLoginPanelShow()
    {
        yield return new WaitForSeconds(2f);
        ShowLoginPanel();
    }

    #endregion

    #region Game Navigation

    // Existing game navigation methods remain the same...
    public void StartMahjongGameDirect()
    {
        PlayButtonSound();

        try
        {
            SceneManager.LoadScene(1);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load Mahjong game scene: {e.Message}");
            ShowErrorPanel("Failed to load game. Please try again.");
        }
    }

    public void StartTaichiGame()
    {
        PlayButtonSound();

        try
        {
            SceneManager.LoadScene("TaichiGame");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load TaichiGame scene: {e.Message}");
            ShowErrorPanel("TaichiGame scene not found. Please add it to Build Settings.");
        }
    }

    public async void OnGameStartFromPanel()
    {
        PlayButtonSound();
        Debug.Log("✅ Starting Mahjong game from Game Start Panel");

        if (FirebaseManager.Instance != null)
        {
            await FirebaseManager.Instance.UpdateGamesPlayedCount();
        }

        try
        {
            SceneManager.LoadScene(1);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load game scene: {e.Message}");
            ShowErrorPanel("Failed to start game. Please try again.");
        }
    }

    #endregion

    #region Error Handling

    public void OnErrorRetry()
    {
        PlayButtonSound();

        if (errorPanel != null)
            errorPanel.SetActive(false);

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

    public void OnLogoutButtonClick()
    {
        PlayButtonSound();
        
        if (FirebaseManager.Instance != null)
        {
            FirebaseManager.Instance.LogoutUser();
            ShowLoginPanel();
            
            if (sessionInfoText != null)
                sessionInfoText.text = "";
        }
        else
        {
            Debug.LogError("❌ FirebaseManager not found");
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