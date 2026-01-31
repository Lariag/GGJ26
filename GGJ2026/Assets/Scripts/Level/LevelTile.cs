using UnityEngine;

[CreateAssetMenu(fileName = "LevelTile", menuName = "Scriptable Objects/LevelTile")]
public class LevelTile : ScriptableObject
{
    public Color Color;
    public RuleTile Tile;
}
