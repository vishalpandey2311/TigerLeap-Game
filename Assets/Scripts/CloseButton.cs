using UnityEngine;
using UnityEngine.UI;

public class CloseButton : MonoBehaviour
{
    [Header("Close Button Settings")]
    public GameObject targetPanel;           // Panel to close
    public CloseButtonType buttonType;       // Type of close action
    public bool playSound = true;           // Play button click sound
    
    [Header("Optional Custom Actions")]
    public UnityEngine.Events.UnityEvent customCloseAction; // Custom actions on close
    
    private Button button;
    
    public enum CloseButtonType
    {
        SimpleClose,        // Just close the panel
        ResumeGame,         // Close and resume game (for pause panel)
        RestartGame,        // Close and restart game
        LoadMainMenu,       // Close and go to main menu
        CustomAction        // Use custom action
    }
    
    void Start()
    {
        // Get button component
        button = GetComponent<Button>();
        
        if (button != null)
        {
            button.onClick.AddListener(OnCloseButtonClicked);
        }
        else
        {
            Debug.LogError("CloseButton script requires a Button component!");
        }
    }
    
    public void OnCloseButtonClicked()
    {
        // Play button sound
        if (playSound)
        {
            PlayButtonSound();
        }
        
        // Perform action based on button type
        switch (buttonType)
        {
            case CloseButtonType.SimpleClose:
                SimpleClosePanel();
                break;
                
            case CloseButtonType.ResumeGame:
                ResumeGameAndClose();
                break;
                
            case CloseButtonType.RestartGame:
                RestartGame();
                break;
                
            case CloseButtonType.LoadMainMenu:
                LoadMainMenu();
                break;
                
            case CloseButtonType.CustomAction:
                ExecuteCustomAction();
                break;
        }
    }
    
    private void SimpleClosePanel()
    {
        if (targetPanel != null)
        {
            targetPanel.SetActive(false);
            Debug.Log($"Closed panel: {targetPanel.name}");
        }
    }
    
    private void ResumeGameAndClose()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResumeGame();
        }
        
        if (targetPanel != null)
        {
            targetPanel.SetActive(false);
        }
    }
    
    private void RestartGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
    }
    
    private void LoadMainMenu()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadMainMenu();
        }
    }
    
    private void ExecuteCustomAction()
    {
        customCloseAction?.Invoke();
        
        // Still close the panel after custom action
        if (targetPanel != null)
        {
            targetPanel.SetActive(false);
        }
    }
    
    private void PlayButtonSound()
    {
        // Check PersistentSoundManager first
        bool soundEnabled = true;
        if (PersistentSoundManager.Instance != null)
        {
            soundEnabled = PersistentSoundManager.Instance.IsGlobalSoundEnabled();
        }
        
        if (soundEnabled)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayButtonClick();
            }
        }
    }
    
    void OnDestroy()
    {
        // Clean up listener
        if (button != null)
        {
            button.onClick.RemoveListener(OnCloseButtonClicked);
        }
    }
}
