using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerPieceSO", menuName = "PlayerPieceSO")]
public class PlayerPieceSO : ScriptableObject
{
    public int ID;
    public List<Vector2Int> squares;
    public List<Vector2Int> corners;
    public bool rotable;
    public bool mirrorable;
    public int pointValue;
    public Texture2D miniature;
}
