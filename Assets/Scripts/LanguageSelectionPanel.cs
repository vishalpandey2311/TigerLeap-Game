using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Manages the Language Selection Panel UI
/// This script handles showing/hiding the language panel and setting up language buttons
/// </summary>
public class LanguageSelectionPanel : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] private GameObject languagePanel;
    [SerializeField] private Button languageButton;        // Main menu language button
    [SerializeField] private Button backButton;           // Back button in language panel
    
    [Header("Language Buttons")]
    [SerializeField] private Button englishButton;
    [SerializeField] private Button chineseButton;
    [SerializeField] private Button hindiButton;
    
    [Header("Button Text References (Optional)")]
    [SerializeField] private TextMeshProUGUI englishButtonText;
    [SerializeField] private TextMeshProUGUI chineseButtonText;
    [SerializeField] private TextMeshProUGUI hindiButtonText;
    
    [Header("Audio")]
    [SerializeField] private bool playButtonSounds = true;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;
    
    private void Start()
    {
        InitializePanel();
        SetupButtonListeners();
    }
    
    #region Initialization
    
    /// <summary>
    /// Initialize the language panel
    /// </summary>
    private void InitializePanel()
    {
        // Hide language panel at start
        if (languagePanel != null)
        {
            languagePanel.SetActive(false);
        }
        
        // Setup button texts if not using localized components
        SetupButtonTexts();
        
        if (showDebugLogs)
        {
            Debug.Log("LanguageSelectionPanel: Panel initialized");
        }
    }
    
    /// <summary>
    /// Setup button listeners - Note: You should set these up in Inspector OnClick events instead
    /// This is just for reference
    /// </summary>
    private void SetupButtonListeners()
    {
        // Note: As per your requirement, you should set these up in Inspector OnClick events
        // This is just showing what methods to call
        
        if (showDebugLogs)
        {
            Debug.Log("LanguageSelectionPanel: Button listeners setup guide:");
            Debug.Log("- Set Language Button OnClick to: LanguageSelectionPanel.ShowLanguagePanel");
            Debug.Log("- Set English Button OnClick to: LanguageManager.ChangeToEnglish + LanguageSelectionPanel.HideLanguagePanel");
            Debug.Log("- Set Chinese Button OnClick to: LanguageManager.ChangeToChinese + LanguageSelectionPanel.HideLanguagePanel");
            Debug.Log("- Set Hindi Button OnClick to: LanguageManager.ChangeToHindi + LanguageSelectionPanel.HideLanguagePanel");
            Debug.Log("- Set Back Button OnClick to: LanguageSelectionPanel.HideLanguagePanel");
        }
    }
    
    /// <summary>
    /// Setup button texts (only if not using localized text components)
    /// </summary>
    private void SetupButtonTexts()
    {
        if (englishButtonText != null)
            englishButtonText.text = "English";
            
        if (chineseButtonText != null)
            chineseButtonText.text = "中文";
            
        if (hindiButtonText != null)
            hindiButtonText.text = "हिन्दी";
    }
    
    #endregion
    
    #region Public Methods (For Button OnClick Events)
    
    /// <summary>
    /// Shows the language selection panel
    /// Call this from the main menu Language button OnClick event
    /// </summary>
    public void ShowLanguagePanel()
    {
        if (playButtonSounds)
        {
            PlayButtonSound();
        }
        
        if (languagePanel != null)
        {
            languagePanel.SetActive(true);
            
            if (showDebugLogs)
            {
                Debug.Log("LanguageSelectionPanel: Language panel shown");
            }
        }
        else
        {
            Debug.LogWarning("LanguageSelectionPanel: Language panel reference is null!");
        }
    }
    
    /// <summary>
    /// Hides the language selection panel
    /// Call this from Back button OnClick event and after language selection
    /// </summary>
    public void HideLanguagePanel()
    {
        if (playButtonSounds)
        {
            PlayButtonSound();
        }
        
        if (languagePanel != null)
        {
            languagePanel.SetActive(false);
            
            if (showDebugLogs)
            {
                Debug.Log("LanguageSelectionPanel: Language panel hidden");
            }
        }
    }
    
    /// <summary>
    /// Handles English button click
    /// Call this from English button OnClick event
    /// </summary>
    public void OnEnglishButtonClick()
    {
        if (showDebugLogs)
        {
            Debug.Log("LanguageSelectionPanel: English button clicked");
        }
        
        if (playButtonSounds)
        {
            PlayButtonSound();
        }
        
        // Change language
        if (LanguageManager.Instance != null)
        {
            LanguageManager.Instance.ChangeToEnglish();
        }
        
        // Hide panel
        HideLanguagePanel();
    }
    
    /// <summary>
    /// Handles Chinese button click
    /// Call this from Chinese button OnClick event
    /// </summary>
    public void OnChineseButtonClick()
    {
        if (showDebugLogs)
        {
            Debug.Log("LanguageSelectionPanel: Chinese button clicked");
        }
        
        if (playButtonSounds)
        {
            PlayButtonSound();
        }
        
        // Change language
        if (LanguageManager.Instance != null)
        {
            LanguageManager.Instance.ChangeToChinese();
        }
        
        // Hide panel
        HideLanguagePanel();
    }
    
    /// <summary>
    /// Handles Hindi button click
    /// Call this from Hindi button OnClick event
    /// </summary>
    public void OnHindiButtonClick()
    {
        if (showDebugLogs)
        {
            Debug.Log("LanguageSelectionPanel: Hindi button clicked");
        }
        
        if (playButtonSounds)
        {
            PlayButtonSound();
        }
        
        // Change language
        if (LanguageManager.Instance != null)
        {
            LanguageManager.Instance.ChangeToHindi();
        }
        
        // Hide panel
        HideLanguagePanel();
    }
    
    #endregion
    
    #region Helper Methods
    
    /// <summary>
    /// Plays button click sound using the same system as MainMenuManager
    /// </summary>
    private void PlayButtonSound()
    {
        // Try to use MainMenuManager's sound system if available
        var mainMenuManager = Object.FindFirstObjectByType<MainMenuManager>();
        if (mainMenuManager != null)
        {
            // MainMenuManager has a private PlayButtonSound method
            // We'll use the button sound system they already have
        }
        
        // Alternative: Use your own audio source
        var audioSource = GetComponent<AudioSource>();
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }
    }
    
    #endregion
    
    #region Editor Helper Methods
    
    /// <summary>
    /// Validates the setup in the inspector
    /// </summary>
    private void OnValidate()
    {
        if (Application.isPlaying) return;
        
        // Check if all required references are assigned
        bool hasErrors = false;
        
        if (languagePanel == null)
        {
            Debug.LogWarning("LanguageSelectionPanel: Language Panel reference not assigned!");
            hasErrors = true;
        }
        
        if (englishButton == null || chineseButton == null || hindiButton == null)
        {
            Debug.LogWarning("LanguageSelectionPanel: Some language button references not assigned!");
            hasErrors = true;
        }
        
        if (!hasErrors && showDebugLogs)
        {
            Debug.Log("LanguageSelectionPanel: All references properly assigned ✓");
        }
    }
    
    #endregion
}
