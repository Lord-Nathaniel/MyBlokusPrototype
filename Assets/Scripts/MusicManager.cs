using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class manages the non-musical audio of the game. 
/// </summary>
public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip clickSound, cassetteRecordSound, checkButtonSound, mirrorSound, successSound, undoSound;
    [SerializeField] private List<AudioClip> buttonPressSound, sprayCanPaintSound, swooshSound, wrongSound;
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
            //case MusicType.Wrong:
            //    i = Random.Range(0, wrongSound.Count);
            //    audioSource.PlayOneShot(wrongSound[i]);
            //    break;
            default:
                break;
        }
    }
}

public enum MusicType
{

}
