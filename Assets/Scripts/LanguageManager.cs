using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

/// <summary>
/// Manages language switching and localization for the TigerLeap game.
/// This script handles changing languages and persisting the player's language preference.
/// </summary>
public class LanguageManager : MonoBehaviour
{
    [Header("Language Settings")]
    [SerializeField] private bool initializeOnStart = true;
    [SerializeField] private bool showDebugLogs = true;
    
    [Header("Available Languages")]
    [SerializeField] private List<LanguageData> availableLanguages = new List<LanguageData>();
    
    [Header("Events")]
    public UnityEvent<string> OnLanguageChanged;
    
    // Static instance for easy access
    public static LanguageManager Instance { get; private set; }
    
    // Current language
    private string currentLanguageCode = "en";
    private const string LANGUAGE_PREF_KEY = "SelectedLanguage";
    
    [System.Serializable]
    public class LanguageData
    {
        public string languageCode;     // e.g., "en", "zh", "es"
        public string displayName;     // e.g., "English", "中文", "Español"
        public Sprite flagIcon;        // Optional flag icon
    }
    
    #region Unity Lifecycle
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        if (initializeOnStart)
        {
            StartCoroutine(InitializeLocalization());
        }
    }
    
    #endregion
    
    #region Initialization
    
    /// <summary>
    /// Initializes the localization system and loads saved language preference
    /// </summary>
    private IEnumerator InitializeLocalization()
    {
        // Wait for localization to initialize
        yield return LocalizationSettings.InitializationOperation;
        
        // Load saved language preference
        string savedLanguage = PlayerPrefs.GetString(LANGUAGE_PREF_KEY, "en");
        
        if (showDebugLogs)
        {
            Debug.Log($"LanguageManager: Initializing with saved language: {savedLanguage}");
        }
        
        // Set the language
        yield return SetLanguage(savedLanguage);
    }
    
    #endregion
    
    #region Public Methods
    
    /// <summary>
    /// Changes the language to the specified language code
    /// Call this method from button OnClick events
    /// </summary>
    /// <param name="languageCode">Language code (e.g., "en", "zh", "es")</param>
    public void ChangeLanguage(string languageCode)
    {
        if (showDebugLogs)
        {
            Debug.Log($"LanguageManager: Changing language to: {languageCode}");
        }
        
        StartCoroutine(SetLanguage(languageCode));
    }
    
    /// <summary>
    /// Changes to English language - for button OnClick
    /// </summary>
    public void ChangeToEnglish()
    {
        ChangeLanguage("en");
    }
    
    /// <summary>
    /// Changes to Chinese language - for button OnClick
    /// </summary>
    public void ChangeToChinese()
    {
        ChangeLanguage("zh");
    }
    
    /// <summary>
    /// Changes to Hindi language - for button OnClick
    /// </summary>
    public void ChangeToHindi()
    {
        ChangeLanguage("hi");
    }
    
    /// <summary>
    /// Gets the current language code
    /// </summary>
    /// <returns>Current language code</returns>
    public string GetCurrentLanguage()
    {
        return currentLanguageCode;
    }
    
    /// <summary>
    /// Gets the display name for the current language
    /// </summary>
    /// <returns>Current language display name</returns>
    public string GetCurrentLanguageDisplayName()
    {
        var langData = availableLanguages.Find(x => x.languageCode == currentLanguageCode);
        return langData != null ? langData.displayName : currentLanguageCode;
    }
    
    /// <summary>
    /// Gets all available languages
    /// </summary>
    /// <returns>List of available language data</returns>
    public List<LanguageData> GetAvailableLanguages()
    {
        return availableLanguages;
    }
    
    #endregion
    
    #region Private Methods
    
    /// <summary>
    /// Sets the language in the localization system
    /// </summary>
    /// <param name="languageCode">Language code to set</param>
    private IEnumerator SetLanguage(string languageCode)
    {
        // Find the locale by code
        var locales = LocalizationSettings.AvailableLocales.Locales;
        var targetLocale = locales.Find(locale => locale.Identifier.Code == languageCode);
        
        if (targetLocale == null)
        {
            if (showDebugLogs)
            {
                Debug.LogWarning($"LanguageManager: Language '{languageCode}' not found. Using default.");
            }
            yield break;
        }
        
        // Change the locale
        LocalizationSettings.SelectedLocale = targetLocale;
        var operation = LocalizationSettings.SelectedLocaleAsync;
        yield return operation;
        
        if (operation.IsDone)
        {
            currentLanguageCode = languageCode;
            
            // Save the preference
            PlayerPrefs.SetString(LANGUAGE_PREF_KEY, languageCode);
            PlayerPrefs.Save();
            
            // Trigger event
            OnLanguageChanged?.Invoke(languageCode);
            
            if (showDebugLogs)
            {
                Debug.Log($"LanguageManager: Successfully changed language to: {languageCode}");
            }
        }
        else
        {
            if (showDebugLogs)
            {
                Debug.LogError($"LanguageManager: Failed to change language to: {languageCode}");
            }
        }
    }
    
    #endregion
    
    #region Editor Helper Methods
    
    /// <summary>
    /// Helper method to setup default languages in the inspector
    /// </summary>
    [ContextMenu("Setup Default Languages")]
    private void SetupDefaultLanguages()
    {
        availableLanguages.Clear();
        
        availableLanguages.Add(new LanguageData
        {
            languageCode = "en",
            displayName = "English"
        });
        
        availableLanguages.Add(new LanguageData
        {
            languageCode = "zh",
            displayName = "中文"
        });
        
        availableLanguages.Add(new LanguageData
        {
            languageCode = "hi",
            displayName = "हिन्दी"
        });
        
        if (showDebugLogs)
        {
            Debug.Log("LanguageManager: Default languages setup complete");
        }
    }
    
    #endregion
}
