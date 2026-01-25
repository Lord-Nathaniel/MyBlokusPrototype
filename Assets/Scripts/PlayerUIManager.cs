using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] private Button prefab;
    [SerializeField] private PlayerPieceDataSO database;
    [SerializeField] private GameObject zone;
    [SerializeField] private PlayerPieceManager playerPieceManager;
    [SerializeField] private Texture2D passButtonTexture;
    [SerializeField] private Color playerColor;
    [SerializeField] private Material playerColorSwap;

    private void Start()
    {
        foreach (PlayerPieceSO playerPiece in database.playerPieces)
        {
            Button newButton = Instantiate(prefab, zone.transform, false);
            Texture2D pieceTexture = playerPiece.miniature;
            if (pieceTexture != null)
            {
                Material colorSwap = playerColorSwap;
                colorSwap.SetColor("ColorRange", playerColor);
                colorSwap.SetTexture("Texture", pieceTexture);
                newButton.GetComponent<Image>().material = colorSwap;
            }
            newButton.onClick.AddListener(() =>
            {
                playerPieceManager.StartPlacement(playerPiece.ID);
            });
        }

        Button passButton = Instantiate(prefab, zone.transform, false);
        playerColorSwap.SetColor("ColorRange", playerColor);
        playerColorSwap.SetTexture("Texture", passButtonTexture);
        passButton.GetComponent<Image>().material = playerColorSwap;
        passButton.onClick.AddListener(() =>
        {
            //TODO pass            
        });
    }
}
