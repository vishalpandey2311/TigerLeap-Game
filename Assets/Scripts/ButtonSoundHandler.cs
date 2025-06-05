using UnityEngine;
using UnityEngine.UI;

public class ButtonSoundHandler : MonoBehaviour
{
    [Header("Button Sound Settings")]
    public AudioClip customButtonSound;  // Optional: Custom sound for this specific button
    public float volume = 0.7f;
    
    private Button button;
    private AudioSource localAudioSource;
    
    void Start()
    {
        // Get the button component
        button = GetComponent<Button>();
        
        if (button != null)
        {
            // Add click listener
            button.onClick.AddListener(PlayButtonSound);
        }
        
        // Setup local audio source if custom sound is provided
        if (customButtonSound != null)
        {
            localAudioSource = gameObject.AddComponent<AudioSource>();
            localAudioSource.clip = customButtonSound;
            localAudioSource.volume = volume;
            localAudioSource.playOnAwake = false;
        }
    }
    
    void PlayButtonSound()
    {
        // Check PersistentSoundManager first
        bool soundEnabled = true;
        if (PersistentSoundManager.Instance != null)
        {
            soundEnabled = PersistentSoundManager.Instance.IsGlobalSoundEnabled();
        }
        
        if (!soundEnabled) return;
        
        // Use custom sound if available, otherwise use AudioManager
        if (customButtonSound != null && localAudioSource != null)
        {
            localAudioSource.PlayOneShot(customButtonSound);
        }
        else if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }
    }
    
    void OnDestroy()
    {
        // Clean up listener
        if (button != null)
        {
            button.onClick.RemoveListener(PlayButtonSound);
        }
    }
}