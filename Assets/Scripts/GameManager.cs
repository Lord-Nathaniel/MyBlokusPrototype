using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class manages the state of the game. 
/// It receives state change ask as input, and output action to do with the current state.
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private int playerNb = 2;
    private int currentPlayerID = 0;
    private readonly State state;
    List<PlayerData> currentPlayers;

    public enum State
    {
        StartGame,
        PlayerTurn,
        EndGame
    }

    private void Start()
    {
        currentPlayers = new List<PlayerData>(playerNb);
        for (int i = 0; i < playerNb; i++)
        {
            currentPlayers.Add(new PlayerData(true));
        }

        SwitchState(State.StartGame);
    }

    /// <summary>
    /// Switch from StartGame state to the currentPlayer state
    /// </summary>
    public void GameStart()
    {
        SwitchState(State.PlayerTurn);
    }

    /// <summary>
    /// Switch from currentPlayer state to nextPlayer state
    /// </summary>
    public void NextPlayer()
    {
        SwitchState(SelectNextState());
    }

    private State SelectNextState()
    {
        for (int i = 1; i <= currentPlayers.Count; i++)
        {
            int nextPlayerID = (currentPlayerID + i) % currentPlayers.Count;

            if (currentPlayers[nextPlayerID].IsActive)
            {
                currentPlayerID = nextPlayerID;
                return State.PlayerTurn;
            }
        }
        return State.EndGame;
    }

    /// <summary>
    /// Switch from currentPlayer state to EndGame state
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
                break;
            case State.EndGame:
                uiManager.ShowEndScreen();
                break;
        }
    }

    struct PlayerData
    {
        public bool IsActive;

        public PlayerData(bool isActive)
        {
            IsActive = isActive;
        }
    }
}