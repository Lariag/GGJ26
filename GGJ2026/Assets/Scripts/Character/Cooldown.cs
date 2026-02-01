using System.Collections.Generic;
using UnityEngine;

public class Cooldown : MonoBehaviour
{
	class CooldownStatus
	{
		public Enums.MaskType MaskType;
		public float CooldownTime;
		public float ActivationTime;
		public bool IsOnCooldown;
	}

	Dictionary<Enums.MaskType, CooldownStatus> cooldownStatus = new();
	float currentTime { get { return Managers.Ins.GameScript.TotalPlayingTime; } }

	void Update()
	{
		if(Managers.Ins.GameScript.CurrentGameState != Enums.GameState.Playing)
			return;

		foreach(var status in cooldownStatus.Values)
		{
			if(!status.IsOnCooldown && currentTime - status.ActivationTime >= status.CooldownTime)
			{
				status.IsOnCooldown = false;
				Managers.Ins.Events.OnMaskCooldownFinished(status.MaskType);
			}
		}
	}

	public bool IsOnCooldown(Enums.MaskType maskType)
	{
		if (!cooldownStatus.ContainsKey(maskType)) return false;
		return cooldownStatus[maskType].IsOnCooldown;
	}

	public void ResetCooldown(Enums.MaskType maskType)
	{
		if (!cooldownStatus.ContainsKey(maskType)) return;
		var status = cooldownStatus[maskType];
		status.ActivationTime = -status.CooldownTime;
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
	}
	public float GetProgress(Enums.MaskType maskType)
	{
		if (!cooldownStatus.ContainsKey(maskType)) return 1f;
		var status = cooldownStatus[maskType];
		return Mathf.Clamp01((currentTime + status.CooldownTime - status.ActivationTime) / status.CooldownTime);
	}

	private void OnEnable()
	{
		foreach (var maskType in cooldownStatus.Keys)
		{
			ResetCooldown(maskType);
		}
	}
}
