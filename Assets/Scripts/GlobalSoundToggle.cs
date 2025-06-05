using UnityEngine;
using UnityEngine.UI;

public class GlobalSoundToggle : MonoBehaviour
{
    [Header("Sound Toggle UI")]
    public Button soundButton;
    public Sprite soundOnSprite;
    public Sprite soundOffSprite;
    
    private Image buttonImage;
    private bool isSoundEnabled = true;
    
    void Start()
    {
        // Get button image
        buttonImage = soundButton.GetComponent<Image>();
        
        // Setup button listener
        soundButton.onClick.AddListener(ToggleSound);
        
        // Get initial state from PersistentSoundManager
        UpdateFromPersistentManager();
        
        Debug.Log($"🔊 GlobalSoundToggle initialized - Sound: {isSoundEnabled}");
    }
    
    void OnEnable()
    {
        // Update state when button becomes active
        UpdateFromPersistentManager();
    }
    
    // Update state from PersistentSoundManager
    private void UpdateFromPersistentManager()
    {
        if (PersistentSoundManager.Instance != null)
        {
            isSoundEnabled = PersistentSoundManager.Instance.IsGlobalSoundEnabled();
            UpdateButtonSprite();
        }
    }
    
    public void ToggleSound()
    {
        // Use PersistentSoundManager instead of local logic
        if (PersistentSoundManager.Instance != null)
        {
            PersistentSoundManager.Instance.ToggleGlobalSound();
        }
        else
        {
            Debug.LogError("❌ PersistentSoundManager not found!");
        }
    }
    
    // Called by PersistentSoundManager to update visual state
    public void UpdateVisualState(bool soundEnabled)
    {
        isSoundEnabled = soundEnabled;
        UpdateButtonSprite();
    }
    
    private void UpdateButtonSprite()
    {
        if (buttonImage != null && soundOnSprite != null && soundOffSprite != null)
        {
            buttonImage.sprite = isSoundEnabled ? soundOnSprite : soundOffSprite;
            Debug.Log($"🖼️ Button sprite updated to: {(isSoundEnabled ? "ON" : "OFF")}");
        }
        else
        {
            Debug.LogError($"❌ Missing components for button sprite update");
        }
    }
}