using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class manages the non-musical audio of the game. 
/// </summary>
public class MusicManager : MonoBehaviour
{
    [SerializeField] private List<AudioClip> funkMusic;
    [SerializeField] private AudioSource audioSource;

    private int currentMusic = 0;

    private void Awake()
    {
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
        switch (musicType)
        {
            case MusicType.FunkMusic:
                audioSource.PlayOneShot(funkMusic[currentMusic]);
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        if (!audioSource.isPlaying)
        {
            currentMusic++;
            PlayMusic(MusicType.FunkMusic);
        }
    }
}

public enum MusicType
{
    FunkMusic
}
