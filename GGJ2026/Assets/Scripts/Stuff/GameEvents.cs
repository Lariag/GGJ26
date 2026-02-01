using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
	#region Event definition

	public Action<Enums.GameState> OnGameStateChangedEvent; // Invoked by GameScript.
	public Action<Enums.MaskType, Enums.MaskType> OnMaskChangedEvent; // Invoked by CharacterScript.
	public Action<RaycastHit2D> OnPlayerDigEvent; // Invoked by CharacterScript.
	public Action<RaycastHit2D[]> OnPlayerMultiDigEvent; // Invoked by CharacterScript.
	public Action<Collision2D> OnPlayerTilemapCollisionEvent; // Invoked by CharacterScript.
	public Action<Vector3> OnPlayerTeleportEvent; // Invoked by CharacterScript.
	public Action<Enums.TileType, Enums.TileCollisionDirection, Vector2Int> OnPlayerTileCollisionEnterEvent; // Invoked by LevelScript.
	public Action<Enums.TileType, Enums.TileCollisionDirection, Vector2Int> OnPlayerTileCollisionStayEvent; // Invoked by LevelScript.
	public Action<Vector2Int> OnPlayerTileCollisionExitEvent; // Invoked by LevelScript.
	public Action<Vector3, Enums.TileType> OnTileDestroyedEvent; // Invoked by LevelScript.
	public Action<Vector3> OnEnemySpawningEvent; // Invoked by LevelScript.
	public Action<Enums.MaskType> OnMaskCooldownFinishedEvent; // Invoked by CooldownScript.
	public Action<Enums.MaskType> OnMaskEffectFinishedEvent; // Invoked by CooldownScript.
	public Action OnEnemyKilledEvent; // Invoked by EnemyScript.

	#endregion Event definition

	#region Event triggering

	public void OnGameStateChanged( Enums.GameState newState) => OnGameStateChangedEvent?.Invoke(newState);
	public void OnMaskChanged(Enums.MaskType oldMask, Enums.MaskType newMask) => OnMaskChangedEvent?.Invoke(oldMask, newMask);
	public void OnPlayerDig(RaycastHit2D hit) => OnPlayerDigEvent?.Invoke(hit);
	public void OnPlayerMultiDig(RaycastHit2D[] hits) => OnPlayerMultiDigEvent?.Invoke(hits);
	public void OnPlayerTilemapCollision(Collision2D collision) => OnPlayerTilemapCollisionEvent?.Invoke(collision);
	public void OnPlayerTeleport(Vector3 distanceMoved) => OnPlayerTeleportEvent?.Invoke(distanceMoved);
	public void OnPlayerTileCollisionEnter( Enums.TileType tileType, Enums.TileCollisionDirection collisionDirection, Vector2Int tilePosition) 
		=> OnPlayerTileCollisionEnterEvent?.Invoke(tileType, collisionDirection, tilePosition);
	public void OnPlayerTileCollisionStay(Enums.TileType tileType, Enums.TileCollisionDirection collisionDirection, Vector2Int tilePosition)
		=> OnPlayerTileCollisionStayEvent?.Invoke(tileType, collisionDirection, tilePosition);
	public void OnPlayerTileCollisionExit(Vector2Int tilePosition) => OnPlayerTileCollisionExitEvent?.Invoke(tilePosition);
	public void OnTileDestroyed(Vector3 position, Enums.TileType tileType) => OnTileDestroyedEvent?.Invoke(position, tileType);
	public void OnEnemySpawning(Vector3 position) => OnEnemySpawningEvent?.Invoke(position);
	public void OnMaskCooldownFinished( Enums.MaskType maskType) => OnMaskCooldownFinishedEvent?.Invoke(maskType);
	public void OnMaskEffectFinished(Enums.MaskType maskType) => OnMaskEffectFinishedEvent?.Invoke(maskType);
	public void OnEnemyKilled() => OnEnemyKilledEvent?.Invoke();

	#endregion
}
