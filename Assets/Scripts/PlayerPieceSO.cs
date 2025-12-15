using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerPieceSO", menuName = "PlayerPieceSO")]
public class PlayerPieceSO : ScriptableObject
{
    [SerializeField] private string pieceName;
    public List<Vector2Int> blocks;
    public List<Vector2Int> corners;
    public bool rotable;
    public bool mirrorable;
}
