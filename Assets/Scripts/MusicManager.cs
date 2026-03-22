using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class manages the non-musical audio of the game. 
/// </summary>
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [SerializeField] private List<AudioClip> funkMusic;
    [SerializeField] private AudioSource audioSource;

    private int currentMusic = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        ServiceManager.Register(this);
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        ServiceManager.Unregister<GameManager>();
    }

    /// <summary>
    /// Plays the needed music from the calling element.
    /// </summary>
    /// <param name="musicType"></param>
    public void PlayMusic(MusicType musicType)
    {
        if (!audioSource.isPlaying)
        {
            switch (musicType)
            {
                case MusicType.FunkMusic:
                    audioSource.resource = funkMusic[currentMusic];
                    audioSource.Play();
                    break;
                default:
                    break;
            }
        }
    }

    private void Update()
    {
        if (!audioSource.isPlaying)
        {
            currentMusic = (currentMusic + 1) % funkMusic.Count;
            PlayMusic(MusicType.FunkMusic);
        }
    }
}

public enum MusicType
{
    FunkMusic
}
