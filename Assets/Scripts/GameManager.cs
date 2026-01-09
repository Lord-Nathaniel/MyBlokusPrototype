using UnityEngine;

public class GameManager : MonoBehaviour
{
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
        state = State.StartGame;
        Debug.Log(state);
    }

    private void SwitchState()
    {
        throw new System.NotImplementedException("SwitchState");
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