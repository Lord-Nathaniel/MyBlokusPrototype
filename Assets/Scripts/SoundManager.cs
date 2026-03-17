using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class manages the non-musical audio of the game. 
/// </summary>
public class SoundManager : MonoBehaviour
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
    /// Plays the needed sound from the calling element.
    /// </summary>
    /// <param name="soundtype"></param>
    public void PlaySound(SoundType soundtype)
    {
        int i;
        switch (soundtype)
        {
            case SoundType.Click:
                audioSource.PlayOneShot(clickSound);
                break;
            case SoundType.CassetteRecord:
                audioSource.PlayOneShot(cassetteRecordSound);
                break;
            case SoundType.CheckButton:
                audioSource.PlayOneShot(checkButtonSound);
                break;
            case SoundType.Mirror:
                audioSource.PlayOneShot(mirrorSound);
                break;
            case SoundType.Success:
                audioSource.PlayOneShot(successSound);
                break;
            case SoundType.Undo:
                audioSource.PlayOneShot(undoSound);
                break;
            case SoundType.ButtonPressed:
                i = Random.Range(0, buttonPressSound.Count);
                audioSource.PlayOneShot(buttonPressSound[i]);
                break;
            case SoundType.SprayCanPaint:
                i = Random.Range(0, sprayCanPaintSound.Count);
                audioSource.PlayOneShot(sprayCanPaintSound[i]);
                break;
            case SoundType.Swoosh:
                i = Random.Range(0, swooshSound.Count);
                audioSource.PlayOneShot(swooshSound[i]);
                break;
            case SoundType.Wrong:
                i = Random.Range(0, wrongSound.Count);
                audioSource.PlayOneShot(wrongSound[i]);
                break;
            default:
                break;
        }
    }
}

public enum SoundType
{
    Click,
    CassetteRecord,
    CheckButton,
    Mirror,
    Success,
    Undo,
    ButtonPressed,
    SprayCanPaint,
    Swoosh,
    Wrong
}
