using UnityEngine;
using UnityEngine.UI;

public class PlayerPiece : MonoBehaviour
{
    public Sprite playerPieceSprite;
    public player playerName;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        image.sprite = playerPieceSprite;
    }
}

public enum player
{
    BluePlayer,
    OrangePlayer
}
