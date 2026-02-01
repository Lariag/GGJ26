using UnityEngine;
using UnityEngine.Rendering;

public class SetBlurOnSpeedUp : MonoBehaviour
{
	public VolumeProfile BlurProfile;

	void OnGameStateChanged(Enums.GameState newState)
	{
		switch (newState)
		{
			case Enums.GameState.LevelStarting:
			case Enums.GameState.GameOver:
				BlurProfile.components[0].active = true;
				break;
			default:
				BlurProfile.components[0].active = false;
				break;
		}
	}
	private void OnEnable()
	{
		Managers.Ins.Events.OnGameStateChangedEvent += OnGameStateChanged;
	}
	private void OnDisable()
	{
		Managers.Ins.Events.OnGameStateChangedEvent -= OnGameStateChanged;
	}
}
