using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.Extensions;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.SceneManagement;

/// <summary>
/// This class manages the state of the game. 
/// It receives state change ask as input, and output action to do with the current state.
/// -IN- UIManager
/// -OUT- UIManager | PlayerPieceManager
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerPieceDataSO database;
    private int currentPlayerID = 0;
    private List<PlayerData> currentPlayers;
    private int firstPlacedPieceNb = 0;
    private int playerNb = 4;

    const string MENU_SCENE = "MenuScene";

    private PersistentVariablesSource source;

    // Needed services
    private UIManager uiManager;
    private PlayerPieceManager playerPieceManager;
    private GridManager gridManager;
    private PlayerSetup playerSetup;
    private SoundManager soundManager;

    public enum State
    {
        StartGame,
        PlayerTurn,
        EndGame
    }

    private void Awake()
    {
        ServiceManager.Register(this);
        source = LocalizationSettings.StringDatabase.SmartFormatter.GetSourceExtension<PersistentVariablesSource>();
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
        playerSetup = ServiceManager.Get<PlayerSetup>();
        soundManager = ServiceManager.Get<SoundManager>();

        InitPlayers();
        //ProtoInitPlayers();
        //TODO temporary, to bypass the start screen

        gridManager.InitGridVisuals(currentPlayers.Count);
        PlaceCamera();

        SwitchState(State.StartGame);
    }

    private void PlaceCamera()
    {
        int posY = 0;
        if (currentPlayers.Count == 2)
        {
            posY = 20;
        }
        else if (currentPlayers.Count > 2)
        {
            posY = 28;
        }

        Vector3 camPosition = new Vector3(mainCamera.transform.position.x, posY, mainCamera.transform.position.z);
        mainCamera.transform.position = camPosition;
    }

    private void InitPlayers()
    {
        List<PlayerSetting> playerSettings = playerSetup.playerSettings;
        playerNb = playerSettings.Count;
        currentPlayers = new List<PlayerData>(playerNb);
        List<Color> currentPlayersColors = new List<Color>();

        for (int i = 0; i < playerNb; i++)
        {
            List<int> playerPieces = new();
            for (int j = 0; j < database.playerPieces.Count; j++)
            {
                playerPieces.Add(j);
            }

            currentPlayers.Add(new PlayerData(
                true,
                playerSettings[i].playerName,
                playerSettings[i].playerColor,
                playerSettings[i].playerTextureID,
                playerPieces,
                0
            ));
            currentPlayersColors.Add(playerSettings[i].playerColor);
        }
        SetPlayerNameGlobalVar();
        uiManager.GenerateRemainingPlayerPieceImages(currentPlayersColors);
    }

    private void ProtoInitPlayers()
    {
        playerNb = 4;
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
                "Player " + i,
                currentPlayerColor,
                i,
                playerPieces,
                0
            ));
            currentPlayersColors.Add(currentPlayerColor);
        }
        SetPlayerNameGlobalVar();
        uiManager.GenerateRemainingPlayerPieceImages(currentPlayersColors);
    }

    private void SetPlayerNameGlobalVar()
    {
        var firstPlayerNameVar = source["global"]["firstPlayerName"] as StringVariable;
        if (currentPlayers[0].playerName == "")
            currentPlayers[0].playerName = "Player One";
        firstPlayerNameVar.Value = currentPlayers[0].playerName;
        var secondPlayerNameVar = source["global"]["secondPlayerName"] as StringVariable;
        if (currentPlayers[1].playerName == "")
            currentPlayers[1].playerName = "Player Two";
        secondPlayerNameVar.Value = currentPlayers[1].playerName;
        if (currentPlayers.Count >= 3)
        {
            var thirdPlayerNameVar = source["global"]["thirdPlayerName"] as StringVariable;
            if (currentPlayers[2].playerName == "")
                currentPlayers[2].playerName = "Player Three";
            thirdPlayerNameVar.Value = currentPlayers[2].playerName;
        }
        if (currentPlayers.Count == 4)
        {
            var fourthPlayerNameVar = source["global"]["fourthPlayerName"] as StringVariable;
            if (currentPlayers[3].playerName == "")
                currentPlayers[3].playerName = "Player Four";
            fourthPlayerNameVar.Value = currentPlayers[3].playerName;
        }
    }

    /// <summary>
    /// Switch from StartGame state to the currentPlayer state
    /// -IN- UIManager from Start()
    /// </summary>
    public void GameStart()
    {
        SwitchState(State.PlayerTurn);
    }

    /// <summary>
    /// Switch from currentPlayer state to nextPlayer state
    /// -IN- UIManager from Start()
    /// </summary>
    public void NextPlayerTurn(bool isPlayerPassing)
    {
        if (isPlayerPassing)
        {
            PlayerPasses();
            ControlFirstTurn();
        }
        int placedPieceID = playerPieceManager.PlacedPlayerPieceID();
        if (placedPieceID < 0)
        {
            return;
        }
        else
        {
            soundManager.PlaySound(SoundType.ButtonPress);
            ControlFirstTurn();

            RemovePieceFromPlayerData(placedPieceID);
            uiManager.UpdateRemainingPlayerPieceImages(currentPlayerID, placedPieceID, currentPlayers[currentPlayerID].score);
            gridManager.SaveCurrentPiece(currentPlayerID, currentPlayers[currentPlayerID].playerTextureID);
            playerPieceManager.StopPlacement();
            SwitchState(SelectNextPlayer());
        }
    }

    private void ControlFirstTurn()
    {
        if (firstPlacedPieceNb < playerNb - 1)
        {
            firstPlacedPieceNb++;
        }
        else if (firstPlacedPieceNb == playerNb - 1)
        {
            gridManager.PurgeStartingCells(playerNb);
            firstPlacedPieceNb++;
        }
    }

    /// <summary>
    /// Player has selected the pass button and passed
    /// -IN- UIManager from Start()
    /// </summary>
    public void PlayerPasses()
    {
        currentPlayers[currentPlayerID].isActive = false;
        uiManager.UpdateRemainingPlayerPieceImages(currentPlayerID, -1, currentPlayers[currentPlayerID].score);
        SwitchState(SelectNextPlayer());
    }

    private void RemovePieceFromPlayerData(int pieceID)
    {
        currentPlayers[currentPlayerID].remainingPlayerPieces.Remove(pieceID);
        currentPlayers[currentPlayerID].score += database.playerPieces[pieceID].pointValue;
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
    /// Quit the game and go back to the menu scene after the game has ended.
    /// -IN- UIManager from Start()
    /// </summary>
    public void GameEnd()
    {
        playerSetup.PurgePlayerSettings();
        SceneManager.LoadScene(MENU_SCENE);
    }

    private void SwitchState(State newState)
    {
        switch (newState)
        {
            case State.StartGame:
                uiManager.ShowStartScreen();
                break;

            case State.PlayerTurn:
                uiManager.HideStartScreen();
                uiManager.GeneratePlayerPieceButtons(
                    currentPlayers[currentPlayerID].playerColor,
                    currentPlayers[currentPlayerID].remainingPlayerPieces,
                    currentPlayerID);
                uiManager.UpdateScreenColor(currentPlayers[currentPlayerID].playerColor);
                break;

            case State.EndGame:
                CompturePlayerScores();
                uiManager.ShowEndScreen();
                break;
        }
    }

    private void CompturePlayerScores()
    {
        int totalPiecesNb = database.playerPieces.Count;
        var sortedCurrentPlayers = currentPlayers.OrderByDescending(FindObjectOfType => FindObjectOfType.score).ToList();
        var isThirdPlayerActiveVar = source["global"]["isThirdPlayerActive"] as BoolVariable;
        isThirdPlayerActiveVar.Value = false;
        var isFourthPlayerActiveVar = source["global"]["isFourthPlayerActive"] as BoolVariable;
        isFourthPlayerActiveVar.Value = false;

        var firstPlayerNameVar = source["global"]["firstPlayerName"] as StringVariable;
        firstPlayerNameVar.Value = sortedCurrentPlayers[0].playerName;
        var firstPlayerScoreVar = source["global"]["firstPlayerScore"] as IntVariable;
        firstPlayerScoreVar.Value = sortedCurrentPlayers[0].score;
        var firstPlayerPlacedPiecesVar = source["global"]["firstPlayerPlacedPieces"] as IntVariable;
        firstPlayerPlacedPiecesVar.Value = totalPiecesNb - sortedCurrentPlayers[0].remainingPlayerPieces.Count;

        var secondPlayerNameVar = source["global"]["secondPlayerName"] as StringVariable;
        secondPlayerNameVar.Value = sortedCurrentPlayers[1].playerName;
        var secondPlayerScoreVar = source["global"]["secondPlayerScore"] as IntVariable;
        secondPlayerScoreVar.Value = sortedCurrentPlayers[1].score;
        var secondPlayerPlacedPiecesVar = source["global"]["secondPlayerPlacedPieces"] as IntVariable;
        secondPlayerPlacedPiecesVar.Value = totalPiecesNb - sortedCurrentPlayers[1].remainingPlayerPieces.Count;

        if (sortedCurrentPlayers.Count >= 3)
        {
            isThirdPlayerActiveVar.Value = true;

            var thirdPlayerNameVar = source["global"]["thirdPlayerName"] as StringVariable;
            thirdPlayerNameVar.Value = sortedCurrentPlayers[2].playerName;
            var thirdPlayerScoreVar = source["global"]["thirdPlayerScore"] as IntVariable;
            thirdPlayerScoreVar.Value = sortedCurrentPlayers[2].score;
            var thirdPlayerPlacedPiecesVar = source["global"]["thirdPlayerPlacedPieces"] as IntVariable;
            thirdPlayerPlacedPiecesVar.Value = totalPiecesNb - sortedCurrentPlayers[2].remainingPlayerPieces.Count;
        }

        if (sortedCurrentPlayers.Count == 4)
        {
            isFourthPlayerActiveVar.Value = true;

            var fourthPlayerNameVar = source["global"]["fourthPlayerName"] as StringVariable;
            fourthPlayerNameVar.Value = sortedCurrentPlayers[3].playerName;
            var fourthPlayerScoreVar = source["global"]["fourthPlayerScore"] as IntVariable;
            fourthPlayerScoreVar.Value = sortedCurrentPlayers[3].score;
            var fourthPlayerPlacedPiecesVar = source["global"]["fourthPlayerPlacedPieces"] as IntVariable;
            fourthPlayerPlacedPiecesVar.Value = totalPiecesNb - sortedCurrentPlayers[3].remainingPlayerPieces.Count;
        }
    }

    private class PlayerData
    {
        public bool isActive;
        public string playerName;
        public Color playerColor;
        public int playerTextureID;
        public List<int> remainingPlayerPieces;
        public int score;

        public PlayerData(bool isActive, string playerName, Color playerColor, int playerTextureID, List<int> playerPieces, int score)
        {
            this.isActive = isActive;
            this.playerName = playerName;
            this.playerColor = playerColor;
            this.playerTextureID = playerTextureID;
            this.remainingPlayerPieces = playerPieces;
            this.score = score;
        }
    }
}