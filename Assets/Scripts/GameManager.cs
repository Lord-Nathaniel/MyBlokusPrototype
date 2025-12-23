using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Button bluePlayerButton;
    [SerializeField] private Button orangePlayerButton;
    [SerializeField] private TextMeshProUGUI boardText;
    [SerializeField] private CellItem cell;

    public int bluePlayerTurn { get; private set; }
    private bool hasBluePlayerEnded = false;
    public int orangePlayerTurn { get; private set; }
    private bool hasOrangePlayerEnded = false;

    private State state;
    public enum State
    {
        BluePlayerTurn,
        OrangePlayerTurn,
        EndGame
    }

    private void Awake()
    {
        bluePlayerTurn = 0;
        orangePlayerTurn = 0;

        bluePlayerButton.onClick.AddListener(() => BluePlayerControlAndPass());
        orangePlayerButton.onClick.AddListener(() => OrangePlayerControlAndPass());
    }

    void Start()
    {
        if (Random.value > 0.5f)
        {
            state = State.BluePlayerTurn;
            orangePlayerButton.gameObject.SetActive(false);
            boardText.text = "joueur Bleu commence";
        }
        else
        {
            state = State.OrangePlayerTurn;
            bluePlayerButton.gameObject.SetActive(false);
            boardText.text = "joueur Orange commence";
        }
    }

    private void Update()
    {
        //if (cell == null) { return; }
        //Debug.Log(cell.GetGridPosition(Input.mousePosition));
    }

    private UnityAction OrangePlayerControlAndPass()
    {
        if (state != State.OrangePlayerTurn) return null;

        DraggableItem placedPiece = GetPlacedPiece();

        if (placedPiece == null)
        {
            hasOrangePlayerEnded = true;
            SwitchState();
            return null;
        }

        PlacePieceInGrid(placedPiece);
        EndTurn();

        orangePlayerTurn++;

        SwitchState();

        return null;
    }

    private UnityAction BluePlayerControlAndPass()
    {
        if (state != State.BluePlayerTurn) return null;

        DraggableItem placedPiece = GetPlacedPiece();

        if (placedPiece == null)
        {
            hasBluePlayerEnded = true;
            SwitchState();
            return null;
        }

        PlacePieceInGrid(placedPiece);
        EndTurn();

        bluePlayerTurn++;

        SwitchState();

        return null;
    }

    private void SwitchState()
    {
        if (boardText.enabled)
        {
            boardText.enabled = false;
        }

        if (hasBluePlayerEnded && hasOrangePlayerEnded)
        {
            state = State.EndGame;
            bluePlayerButton.gameObject.SetActive(false);
            orangePlayerButton.gameObject.SetActive(false);
            boardText.enabled = true;
        }

        if (state == State.BluePlayerTurn && !hasOrangePlayerEnded)
        {
            state = State.OrangePlayerTurn;
            bluePlayerButton.gameObject.SetActive(false);
            orangePlayerButton.gameObject.SetActive(true);
        }

        if (state == State.OrangePlayerTurn && !hasBluePlayerEnded)
        {
            state = State.BluePlayerTurn;
            bluePlayerButton.gameObject.SetActive(true);
            orangePlayerButton.gameObject.SetActive(false);
        }
    }

    private DraggableItem GetPlacedPiece()
    {
        throw new System.NotImplementedException("GetPlacedPiece");
        //TODO attraper la pièce qu vient tout juste d'être placée
    }

    private void PlacePieceInGrid(DraggableItem placedPiece)
    {
        throw new System.NotImplementedException("PlacePieceInGrid");
    }

    public void EndTurn()
    {
        throw new System.NotImplementedException("EndTurn");
        //TODO refacto les BluePlayerControlAndPass et OrangePlayerControlAndPass
        //TODO la pièce qui vient d'être attrapée donne ses positions sur la grille,
        // et la grille change les cases correspondantes pour être colorées
    }
}