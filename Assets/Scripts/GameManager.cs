using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class manages the state of the game. 
/// It receives state change ask as input, and output action to do with the current state.
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;

    private int playerNb = 2;
    private int currentPlayer = 0;
    private List<Tuple<int, bool>> currentPlayers;
    private State state;
    public enum State
    {
        StartGame,
        PlayerOneTurn,
        PlayerTwoTurn,
        PlayerThreeTurn,
        PlayerFourTurn,
        EndGame
    }

    private void Start()
    {
        currentPlayers = new List<Tuple<int, bool>>();
        currentPlayers.Add(new Tuple<int, bool>(0, true));
        currentPlayers.Add(new Tuple<int, bool>(1, true));
        if (playerNb > 2)
            currentPlayers.Add(new Tuple<int, bool>(2, true));
        if (playerNb > 3)
            currentPlayers.Add(new Tuple<int, bool>(3, true));

        SwitchState(State.StartGame);
    }

    /// <summary>
    /// Switch from StartGame state to the currentPlayer state
    /// </summary>
    public void GameStart()
    {
        SwitchState(State.PlayerOneTurn);
    }

    /// <summary>
    /// Switch from currentPlayer state to nextPlayer state
    /// </summary>
    /// <param name="playerID"></param>
    public void NextPlayer(int playerID)
    {
        State nextPlayerState = SelectNextPlayerState(playerID);
        SwitchState(nextPlayerState);
    }

    private State SelectNextPlayerState(int playerID)
    {
        //TODO search in currentPlayers
        return State.EndGame;
    }

    /// <summary>
    /// Switch from currentPlayer state to EndGame state
    /// </summary>
    public void GameEnd()
    {
        SwitchState(State.EndGame);
    }

    private void SwitchState(State state)
    {
        this.state = state;
        switch (state)
        {
            case State.StartGame:
                uiManager.ShowStartScreen();
                break;
            case State.PlayerOneTurn:
                uiManager.HideStartScreen();
                break;
            case State.PlayerTwoTurn:
                uiManager.HideStartScreen();
                break;
            case State.EndGame:
                uiManager.ShowEndScreen();
                break;

        }
        //if (boardText.enabled)
        //{
        //    boardText.enabled = false;
        //}

        //if (hasBluePlayerEnded && hasOrangePlayerEnded)
        //{
        //    state = State.EndGame;
        //    bluePlayerButton.gameObject.SetActive(false);
        //    orangePlayerButton.gameObject.SetActive(false);
        //    boardText.enabled = true;
        //}

        //if (state == State.BluePlayerTurn && !hasOrangePlayerEnded)
        //{
        //    state = State.OrangePlayerTurn;
        //    bluePlayerButton.gameObject.SetActive(false);
        //    orangePlayerButton.gameObject.SetActive(true);
        //}

        //if (state == State.OrangePlayerTurn && !hasBluePlayerEnded)
        //{
        //    state = State.BluePlayerTurn;
        //    bluePlayerButton.gameObject.SetActive(true);
        //    orangePlayerButton.gameObject.SetActive(false);
        //}
    }
}