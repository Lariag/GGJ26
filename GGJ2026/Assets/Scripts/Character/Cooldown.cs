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

	void Update()
	{
		if (Managers.Ins.GameScript.CurrentGameState != Enums.GameState.Playing)
			return;

		foreach (var status in cooldownStatus.Values)
		{
			if (!status.IsOnCooldown && currentTime - status.ActivationTime >= status.CooldownTime + status.EffectTime)
			{
				status.IsOnCooldown = false;
				Managers.Ins.Events.OnMaskCooldownFinished(status.MaskType);
			}

			if (status.IsEffectActive && currentTime - status.ActivationTime >= status.EffectTime)
			{
				status.IsEffectActive = false;
				Managers.Ins.Events.OnMaskEffectFinished(status.MaskType);
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
		status.ActivationTime = -status.CooldownTime;
		status.IsEffectActive = false;
	}

	public void AddConfig(Enums.MaskType maskType, float cooldownTime)
	{
		cooldownStatus.Add(maskType, new CooldownStatus()
		{
			MaskType = maskType,
			CooldownTime = cooldownTime,
			ActivationTime = -cooldownTime,
			IsOnCooldown = false
		});
	}

	public void StartCooldown(Enums.MaskType maskType)
	{
		if (!cooldownStatus.ContainsKey(maskType)) return;
		cooldownStatus[maskType].ActivationTime = currentTime;
		cooldownStatus[maskType].IsOnCooldown = true;
		cooldownStatus[maskType].IsEffectActive = true;
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

	void OnEnable()
	{
		foreach (var maskType in cooldownStatus.Keys)
		{
			ResetCooldown(maskType);
		}
		Managers.Ins.Events.OnMaskChangedEvent += StartCooldown;
	}

	void OnDisable()
	{
		Managers.Ins.Events.OnMaskChangedEvent -= StartCooldown;
	}
}
