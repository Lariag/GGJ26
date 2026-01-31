using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
	#region Event definition

	public Action<Enums.GameState> OnGameStateChangedEvent; // Invoked by GameScript.
	public Action<Enums.MaskType> OnMaskChangedEvent; // Invoked by CharacterScript.
	public Action<Collision2D> OnPlayerTilemapCollisionEvent; // Invoked by CharacterScript.
	public Action<Enums.TileType, Enums.TileCollisionDirection, Vector2Int> OnPlayerTileCollisionEnterEvent; // Invoked by LevelScript.
	public Action<Enums.TileType, Enums.TileCollisionDirection, Vector2Int> OnPlayerTileCollisionStayEvent; // Invoked by LevelScript.
	public Action<Vector2Int> OnPlayerTileCollisionExitEvent; // Invoked by LevelScript.

	#endregion Event definition

	#region Event triggering

	public void OnGameStateChanged( Enums.GameState newState) => OnGameStateChangedEvent?.Invoke(newState);
	public void OnMaskChanged( Enums.MaskType newMask) => OnMaskChangedEvent?.Invoke(newMask);
	public void OnPlayerTilemapCollision(Collision2D collision) => OnPlayerTilemapCollisionEvent?.Invoke(collision);
	public void OnPlayerTileCollisionEnter( Enums.TileType tileType, Enums.TileCollisionDirection collisionDirection, Vector2Int tilePosition) 
		=> OnPlayerTileCollisionEnterEvent?.Invoke(tileType, collisionDirection, tilePosition);
	public void OnPlayerTileCollisionStay(Enums.TileType tileType, Enums.TileCollisionDirection collisionDirection, Vector2Int tilePosition)
		=> OnPlayerTileCollisionStayEvent?.Invoke(tileType, collisionDirection, tilePosition);
	public void OnPlayerTileCollisionExit(Vector2Int tilePosition)
		=> OnPlayerTileCollisionExitEvent?.Invoke(tilePosition);

	#endregion
}
