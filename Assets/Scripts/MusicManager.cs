using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class manages the non-musical audio of the game. 
/// </summary>
public class MusicManager : MonoBehaviour
{
    [SerializeField] private List<AudioClip> funkMusic;
    [SerializeField] private AudioSource audioSource;

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
    public void PlaySound(MusicType musicType)
    {
        int i;
        switch (musicType)
        {
            case MusicType.FunkMusic:
                i = Random.Range(0, funkMusic.Count);
                audioSource.PlayOneShot(funkMusic[i]);
                break;
            default:
                break;
        }
    }
}

public enum MusicType
{
    FunkMusic
}
