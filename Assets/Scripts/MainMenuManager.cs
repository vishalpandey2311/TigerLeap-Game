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
    public AudioClip backgroundMusicClip;  // Assign your background music here
    
    [Header("Button Sounds")]
    public AudioClip buttonClickSound;     // ASSIGN THIS IN INSPECTOR!
    
    [Header("Info Panel")]
    public Button infoButton;              // NEW: Reference to info button
    public GameObject instructionPanel;    // NEW: Reference to instruction panel in main menu
    public Button gotItButton;             // NEW: Reference to "Got It" button in instruction panel
    
    [Header("Sound Toggle")]
    public Button soundToggleButton;     // NEW: Reference to sound toggle button
    public Sprite soundOnSprite;         // NEW: Sound ON sprite
    public Sprite soundOffSprite;        // NEW: Sound OFF sprite
    
    // Add local audio source for button sounds
    private AudioSource buttonAudioSource;
    
    void Start()
    {
        SetupInfoButton();
        SetupButtonAudio();
        SetupSoundToggle(); // NEW: Simple setup
        InitializeBackgroundMusic();
    }
    
    // NEW: Setup info button
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
        
        // Hide instruction panel initially
        if (instructionPanel != null)
        {
            instructionPanel.SetActive(false);
        }
    }
    
    // NEW: Show instruction panel
    public void ShowInstructions()
    {
        PlayButtonSound();
        
        if (instructionPanel != null)
        {
            instructionPanel.SetActive(true);
        }
    }
    
    // NEW: Hide instruction panel
    public void HideInstructions()
    {
        PlayButtonSound();
        
        if (instructionPanel != null)
        {
            instructionPanel.SetActive(false);
        }
    }
    
    // NEW: Setup local audio source for immediate button sounds
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
    
    // NEW: Simple sound toggle setup
    private void SetupSoundToggle()
    {
        if (soundToggleButton != null)
        {
            // Add the GlobalSoundToggle component
            GlobalSoundToggle soundToggle = soundToggleButton.GetComponent<GlobalSoundToggle>();
            if (soundToggle == null)
            {
                soundToggle = soundToggleButton.gameObject.AddComponent<GlobalSoundToggle>();
            }
            
            // Assign references
            soundToggle.soundButton = soundToggleButton;
            soundToggle.soundOnSprite = soundOnSprite;
            soundToggle.soundOffSprite = soundOffSprite;
            
            Debug.Log("✅ Sound toggle setup complete");
        }
    }
    
    void InitializeBackgroundMusic()
    {
        // Check if MusicManager already exists
        if (MusicManager.Instance == null && backgroundMusicClip != null)
        {
            // Create a new GameObject for MusicManager
            GameObject musicManagerObj = new GameObject("MusicManager");
            MusicManager musicManager = musicManagerObj.AddComponent<MusicManager>();
            musicManager.backgroundMusic = backgroundMusicClip;
        }
    }
    
    public void StartGame()
    {
        // Play button sound immediately using local audio source
        PlayButtonSound();
        
        // Load your game scene
        SceneManager.LoadScene(1);
    }
    
    public void QuitGame()
    {
        // Play button sound immediately using local audio source
        PlayButtonSound();
        
        Debug.Log("Quitting game...");
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    
    // NEW: Method to play button sound locally
    private void PlayButtonSound()
    {
        if (buttonAudioSource != null && buttonClickSound != null)
        {
            buttonAudioSource.PlayOneShot(buttonClickSound);
        }
    }
}