using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] private Button prefab;
    [SerializeField] private PlayerPieceDataSO database;
    [SerializeField] private GameObject zone;
    [SerializeField] private PlayerPieceManager playerPieceManager;
    [SerializeField] private Sprite passButtonSprite;

    private void Start()
    {
        foreach (PlayerPieceSO playerPiece in database.playerPieces)
        {
            Button newButton = Instantiate(prefab, zone.transform, false);
            Sprite pieceSprite = playerPiece.miniature;
            if (pieceSprite != null)
            {
                newButton.GetComponent<Image>().sprite = pieceSprite;
            }
            newButton.onClick.AddListener(() =>
            {
                playerPieceManager.StartPlacement(playerPiece.ID);
            });
        }

        Button passButton = Instantiate(prefab, zone.transform, false);
        passButton.GetComponent<Image>().sprite = passButtonSprite;
        passButton.onClick.AddListener(() =>
        {
            //TODO pass            
        });
    }
}
