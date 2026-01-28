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
            Image img = newButton.GetComponent<Image>();
            Texture2D pieceTexture = playerPiece.miniature;
            if (pieceTexture != null)
            {
                img.sprite = Sprite.Create(
                    playerPiece.miniature,
                    new Rect(0, 0, playerPiece.miniature.width, playerPiece.miniature.height),
                    new Vector2(0.5f, 0.5f)
                );
            }

            Material mat = new Material(playerColorSwap);
            mat.SetColor("_PlayerColor", playerColor);
            mat.SetTexture("_MainTex", pieceTexture);

            img.material = mat;


            newButton.onClick.AddListener(() =>
            {
                playerPieceManager.StartPlacement(playerPiece.ID);
            });
        }

        //Button passButton = Instantiate(prefab, zone.transform, false);
        //playerColorSwap.SetColor("ColorRange", playerColor);
        //playerColorSwap.SetTexture("Texture", passButtonTexture);
        //passButton.GetComponent<Image>().material = playerColorSwap;
        //passButton.onClick.AddListener(() =>
        //{
        //    //TODO pass            
        //});
    }
}
