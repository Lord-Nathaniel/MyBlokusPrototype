using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerPieceData", menuName = "PlayerPieceData")]
public class PlayerPieceData : ScriptableObject
{
    public List<PlayerPieceSO> playerPieces;
}
