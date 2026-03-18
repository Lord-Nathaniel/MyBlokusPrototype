using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class manages the non-musical audio of the game. 
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField]
    private List<AudioClip> buttonPressSounds,
        cancelSounds,
        checkButtonClickSounds,
        cassetteRecordClickSounds,
        chessPieceSounds,
        clickSounds,
        notificationSounds,
        mirrorSounds,
        sprayCanPaintSounds,
        sucessFanfareSounds,
        swooshSounds,
        undoSounds,
        wrongSounds;
    [SerializeField] private AudioSource audioSource;

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
    /// Plays the needed sound from the calling element.
    /// </summary>
    /// <param name="soundtype"></param>
    public void PlaySound(SoundType soundtype)
    {
        int i;
        switch (soundtype)
        {
            case SoundType.ButtonPress:
                i = Random.Range(0, buttonPressSounds.Count);
                audioSource.PlayOneShot(buttonPressSounds[i]);
                break;
            case SoundType.Cancel:
                i = Random.Range(0, cancelSounds.Count);
                audioSource.PlayOneShot(cancelSounds[i]);
                break;
            case SoundType.CheckButtonClick:
                i = Random.Range(0, checkButtonClickSounds.Count);
                audioSource.PlayOneShot(checkButtonClickSounds[i]);
                break;
            case SoundType.CassetteRecordClick:
                i = Random.Range(0, cassetteRecordClickSounds.Count);
                audioSource.PlayOneShot(cassetteRecordClickSounds[i]);
                break;
            case SoundType.ChessPiece:
                i = Random.Range(0, chessPieceSounds.Count);
                audioSource.PlayOneShot(chessPieceSounds[i]);
                break;
            case SoundType.Click:
                i = Random.Range(0, clickSounds.Count);
                audioSource.PlayOneShot(clickSounds[i]);
                break;
            case SoundType.Notification:
                i = Random.Range(0, notificationSounds.Count);
                audioSource.PlayOneShot(notificationSounds[i]);
                break;
            case SoundType.Mirror:
                i = Random.Range(0, mirrorSounds.Count);
                audioSource.PlayOneShot(mirrorSounds[i]);
                break;
            case SoundType.SprayCanPaint:
                i = Random.Range(0, sprayCanPaintSounds.Count);
                audioSource.PlayOneShot(sprayCanPaintSounds[i]);
                break;
            case SoundType.SucessFanfare:
                i = Random.Range(0, sucessFanfareSounds.Count);
                audioSource.PlayOneShot(sucessFanfareSounds[i]);
                break;
            case SoundType.Swoosh:
                i = Random.Range(0, swooshSounds.Count);
                audioSource.PlayOneShot(swooshSounds[i]);
                break;
            case SoundType.Undo:
                i = Random.Range(0, undoSounds.Count);
                audioSource.PlayOneShot(undoSounds[i]);
                break;
            case SoundType.Wrong:
                i = Random.Range(0, wrongSounds.Count);
                audioSource.PlayOneShot(wrongSounds[i]);
                break;
            default:
                break;
        }
    }
}

public enum SoundType
{
    ButtonPress,
    Cancel,
    CheckButtonClick,
    CassetteRecordClick,
    ChessPiece,
    Click,
    Notification,
    Mirror,
    SprayCanPaint,
    SucessFanfare,
    Swoosh,
    Undo,
    Wrong
}
