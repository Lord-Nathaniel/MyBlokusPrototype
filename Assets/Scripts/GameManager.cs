using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class manages the state of the game. 
/// It receives state change ask as input, and output action to do with the current state.
/// -IN- UIManager
/// -OUT- UIManager | PlayerPieceManager
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField] private int playerNb = 4;
    [SerializeField] private PlayerPieceDataSO database;
    private int currentPlayerID = 0;
    private readonly State state;
    List<PlayerData> currentPlayers;

    // Needed services
    private UIManager uiManager;
    private PlayerPieceManager playerPieceManager;
    private GridManager gridManager;

    public enum State
    {
        StartGame,
        PlayerTurn,
        EndGame
    }

    private void Awake()
    {
        ServiceManager.Register(this);
    }

    private void OnDestroy()
    {
        ServiceManager.Unregister<GameManager>();
    }

    private void Start()
    {
        uiManager = ServiceManager.Get<UIManager>();
        playerPieceManager = ServiceManager.Get<PlayerPieceManager>();
        gridManager = ServiceManager.Get<GridManager>();

        InitPlayers();
        //TODO temporary, to bypass the start screen
        FirstPlayerTurn();
        //SwitchState(State.StartGame);
    }

    private void InitPlayers()
    {
        currentPlayers = new List<PlayerData>(playerNb);
        List<Color> currentPlayersColors = new List<Color>();
        for (int i = 0; i < playerNb; i++)
        {
            List<int> playerPieces = new();
            for (int j = 0; j < database.playerPieces.Count; j++)
            {
                playerPieces.Add(j);
            }

            Color currentPlayerColor = new Color((float)(i * 0.3), (float)(i * 0.3), (float)(i * 0.3));

            currentPlayers.Add(new PlayerData(
                true,
                currentPlayerColor,
                playerPieces,
                0
            ));
            currentPlayersColors.Add(currentPlayerColor);
        }

        uiManager.GenerateRemainingPlayerPieceImages(currentPlayersColors);
    }

    /// <summary>
    /// Switch from StartGame state to the currentPlayer state
    /// -IN- UIManager from Start()
    /// </summary>
    public void GameStart()
    {
        SwitchState(State.PlayerTurn);
    }

    private void FirstPlayerTurn()
    {
        SwitchState(SelectNextPlayer());
    }

    /// <summary>
    /// Switch from currentPlayer state to nextPlayer state
    /// -IN- UIManager from Start()
    /// </summary>
    public void NextPlayerTurn()
    {
        int placedPiece = playerPieceManager.PlacedPlayerPieceID();
        if (placedPiece >= 0)
        {
            gridManager.SaveCurrentPiece(placedPiece);
            playerPieceManager.StopPlacement();
            RemovePieceFromPlayerData(placedPiece);
            SwitchState(SelectNextPlayer());
        }
    }

    /// <summary>
    /// Player has selected the pass button and passed
    /// -IN- UIManager from Start()
    /// </summary>
    public void PlayerPasses()
    {
        currentPlayers[currentPlayerID].isActive = false;
        SelectNextPlayer();
    }

    private void RemovePieceFromPlayerData(int pieceID)
    {
        currentPlayers[currentPlayerID].remainingPlayerPieces.Remove(pieceID);
    }

    private State SelectNextPlayer()
    {
        for (int i = 1; i <= currentPlayers.Count; i++)
        {
            int nextPlayerID = (currentPlayerID + i) % currentPlayers.Count;

            if (currentPlayers[nextPlayerID].isActive)
            {
                currentPlayerID = nextPlayerID;
                return State.PlayerTurn;
            }
        }
        return State.EndGame;
    }

    /// <summary>
    /// Switch from currentPlayer state to EndGame state
    /// -IN- UIManager from Start()
    /// </summary>
    public void GameEnd()
    {
        SwitchState(State.EndGame);
    }

    private void SwitchState(State newState)
    {
        if (state == newState)
            return;

        switch (newState)
        {
            case State.StartGame:
                uiManager.ShowStartScreen();
                break;

            case State.PlayerTurn:
                uiManager.HideStartScreen();
                uiManager.GeneratePlayerPieceButtons(currentPlayers[currentPlayerID].playerColor, currentPlayers[currentPlayerID].remainingPlayerPieces);
                break;

            case State.EndGame:
                uiManager.ShowEndScreen();
                break;
        }
    }

    private class PlayerData
    {
        public bool isActive;
        public Color playerColor;
        public List<int> remainingPlayerPieces;
        public int score;

        public PlayerData(bool isActive, Color playerColor, List<int> playerPieces, int score)
        {
            this.isActive = isActive;
            this.playerColor = playerColor;
            this.remainingPlayerPieces = playerPieces;
            this.score = score;
        }
    }
}