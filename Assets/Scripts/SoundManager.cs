using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip clickSound, placeFloorSound, placeFurnitureSound, removeSound, wrongPlacementSound;
    [SerializeField] private AudioSource audioSource;

    public void PlaySound(SoundType soundtype)
    {
        switch (soundtype)
        {
            case SoundType.Click:
                audioSource.PlayOneShot(clickSound);
                break;
            case SoundType.PlaceFloor:
                audioSource.PlayOneShot(placeFloorSound);
                break;
            case SoundType.PlaceFurniture:
                audioSource.PlayOneShot(placeFurnitureSound);
                break;
            case SoundType.Remove:
                audioSource.PlayOneShot(removeSound);
                break;
            case SoundType.WrongPlacement:
                audioSource.PlayOneShot(wrongPlacementSound);
                break;
            default:
                break;
        }
    }
}

public enum SoundType
{
    Click,
    PlaceFloor,
    PlaceFurniture,
    Remove,
    WrongPlacement
}
