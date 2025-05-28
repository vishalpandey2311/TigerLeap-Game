using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(0.5f, 1.5f)]
        public float pitch = 1f;
        [HideInInspector]
        public AudioSource source;
        public bool loop = false;
    }
    
    // Singleton pattern
    public static AudioManager Instance;
    
    [Header("Audio Settings")]
    public Sound[] sounds;
    
    [Header("Global Audio Settings")]
    [Range(0f, 1f)]
    public float masterVolume = 1f;
    public bool isMuted = false;
    
    void Awake()
    {
        // Create singleton
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
        
        // Create audio sources for each sound
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume * masterVolume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }
    
    // Play a sound by name
    public void Play(string name)
    {
        if (isMuted) return;
        
        Sound s = System.Array.Find(sounds, sound => sound.name == name);
        if (s != null)
        {
            s.source.Play();
        }
        else
        {
            Debug.LogWarning("Sound: " + name + " not found!");
        }
    }
    
    // Stop a sound by name
    public void Stop(string name)
    {
        Sound s = System.Array.Find(sounds, sound => sound.name == name);
        if (s != null)
        {
            s.source.Stop();
        }
        else
        {
            Debug.LogWarning("Sound: " + name + " not found!");
        }
    }
    
    // Stop all sounds
    public void StopAll()
    {
        foreach (Sound s in sounds)
        {
            if (s.source != null)
                s.source.Stop();
        }
    }
    
    // Check if a sound is playing
    public bool IsPlaying(string name)
    {
        Sound s = System.Array.Find(sounds, sound => sound.name == name);
        if (s != null)
        {
            return s.source.isPlaying;
        }
        return false;
    }
    
    // Set master volume
    public void SetVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        
        foreach (Sound s in sounds)
        {
            if (s.source != null)
                s.source.volume = s.volume * masterVolume;
        }
    }
    
    // Toggle mute
    public void ToggleMute()
    {
        isMuted = !isMuted;
        
        foreach (Sound s in sounds)
        {
            if (s.source != null)
                s.source.volume = isMuted ? 0 : s.volume * masterVolume;
        }
    }
}
