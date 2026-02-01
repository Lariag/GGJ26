using System.Collections.Generic;
using UnityEngine;

public class Cooldown : MonoBehaviour
{
	class CooldownStatus
	{
		public Enums.MaskType MaskType;
		public float CooldownTime;
		public float EffectTime;
		public float ActivationTime;
		public bool IsOnCooldown;
		public bool IsEffectActive;
	}

	Dictionary<Enums.MaskType, CooldownStatus> cooldownStatus = new();
	float currentTime { get { return Managers.Ins.GameScript.TotalPlayingTime; } }

	void Start()
	{
		Managers.Ins.Events.OnMaskChangedEvent += StartCooldown;
		Managers.Ins.Events.OnGameStateChangedEvent += OnGameStateChanged;
	}
	void Update()
	{
		if (Managers.Ins.GameScript.CurrentGameState != Enums.GameState.Playing)
			return;

		foreach (var status in cooldownStatus.Values)
		{
			/*if (status.IsOnCooldown && currentTime - status.ActivationTime >= status.CooldownTime + status.EffectTime)
			{
				status.IsOnCooldown = false;
				Managers.Ins.Events.OnMaskCooldownFinished(status.MaskType);
				Debug.Log($"Cooldown finished for mask: {status.MaskType}");
			}

			if (status.IsEffectActive && currentTime - status.ActivationTime >= status.EffectTime)
			{
				status.IsEffectActive = false;
				Managers.Ins.Events.OnMaskEffectFinished(status.MaskType);
				Debug.Log($"Effect finished for mask: {status.MaskType}");
			}*/

			if (status.IsOnCooldown && currentTime >= status.CooldownTime)
			{
				status.IsOnCooldown = false;
				Managers.Ins.Events.OnMaskCooldownFinished(status.MaskType);
				Debug.Log($"Cooldown finished for mask: {status.MaskType}");
			}

			if (status.IsEffectActive && currentTime - status.ActivationTime >= status.EffectTime)
			{
				status.IsEffectActive = false;
				Managers.Ins.Events.OnMaskEffectFinished(status.MaskType);
				Debug.Log($"Effect finished for mask: {status.MaskType}");
			}
		}
	}

	public bool IsOnCooldown(Enums.MaskType maskType)
	{
		if (!cooldownStatus.ContainsKey(maskType)) return false;
		return cooldownStatus[maskType].IsOnCooldown;
	}

	public bool IsEffectActive(Enums.MaskType maskType)
	{
		if (!cooldownStatus.ContainsKey(maskType)) return false;
		return cooldownStatus[maskType].IsEffectActive;
	}

	public void ResetCooldown(Enums.MaskType maskType)
	{
		if (!cooldownStatus.ContainsKey(maskType)) return;
		var status = cooldownStatus[maskType];
		status.ActivationTime = currentTime - (status.CooldownTime + status.ActivationTime);
		status.IsEffectActive = false;
		status.IsOnCooldown = false;
	}

	public void AddConfig(Enums.MaskType maskType, float cooldownTime, float effectDuration)
	{
		cooldownStatus.Add(maskType, new CooldownStatus()
		{
			MaskType = maskType,
			CooldownTime = cooldownTime,
			EffectTime = effectDuration,
			ActivationTime = -cooldownTime - effectDuration,
			IsOnCooldown = false
		});
	}

	public void CancelEffect(Enums.MaskType maskType)
	{
		if (!cooldownStatus.ContainsKey(maskType)) return;

		var status = cooldownStatus[maskType];
		if (status.IsEffectActive)
		{
			status.IsEffectActive = false;
			if (status.IsOnCooldown)
				status.ActivationTime = currentTime - status.CooldownTime;
		}
	}

	public void StartCooldown(Enums.MaskType oldMask, Enums.MaskType maskType)
	{
		CancelEffect(oldMask);

		if (!cooldownStatus.ContainsKey(maskType)) return;

		var status = cooldownStatus[maskType];
		status.ActivationTime = currentTime;
		status.IsOnCooldown = true;
		status.IsEffectActive = true;
	}
	public float GetProgress(Enums.MaskType maskType)
	{
		if (!cooldownStatus.ContainsKey(maskType)) return 1f;
		var status = cooldownStatus[maskType];
		return 1f - Mathf.Clamp01((currentTime - status.ActivationTime) / (status.CooldownTime + status.EffectTime));
	}

	public float GetProgressCooldown(Enums.MaskType maskType)
	{
		if (!cooldownStatus.ContainsKey(maskType)) return 1f;
		var status = cooldownStatus[maskType];
		return 1f - Mathf.Clamp01((currentTime - status.ActivationTime - status.EffectTime) / (status.CooldownTime));
	}

	public float GetProgressStatus(Enums.MaskType maskType)
	{
		if (!cooldownStatus.ContainsKey(maskType)) return 1f;
		var status = cooldownStatus[maskType];
		return 1f - Mathf.Clamp01((currentTime - status.ActivationTime) / (status.CooldownTime));
	}

	void OnGameStateChanged(Enums.GameState gameState)
	{
		if (gameState != Enums.GameState.LevelStarting)
			return;

		foreach (var maskType in cooldownStatus.Keys)
		{
			ResetCooldown(maskType);
		}
	}


}
