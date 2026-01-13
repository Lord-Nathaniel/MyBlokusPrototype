using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerPieceData", menuName = "PlayerPieceData")]
public class PlayerPieceDataSO : ScriptableObject
{
    public List<PlayerPieceSO> playerPieces;
}
