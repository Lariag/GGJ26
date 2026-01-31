using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
	#region Event definition

	public Action<Enums.GameState> OnGameStateChangedEvent; // Managed by GameScript.
	public Action<Enums.MaskType> OnMaskChangedEvent; // Managed by CharacterScript.

	#endregion Event definition

	#region Event triggering

	public void OnGameStateChanged( Enums.GameState newState) => OnGameStateChangedEvent?.Invoke(newState);
	public void OnMaskChanged( Enums.MaskType newMask) => OnMaskChangedEvent?.Invoke(newMask);

	#endregion
}
