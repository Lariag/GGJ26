using System;
using System.Collections.Generic;
using UnityEngine;

public class Cooldown : MonoBehaviour
{
	[Serializable]
	public struct CooldownConfig
	{
		public Enums.MaskType MaskType;
		public float CooldownTime;
	}

	struct CooldownStatus
	{
		public Enums.MaskType MaskType;
		public float CooldownTime;
		public float ActivationTime;
	}

	public CooldownConfig[] CooldownConfigs;
	Dictionary<Enums.MaskType, CooldownStatus> cooldownStatus;

	private void Awake()
	{
		cooldownStatus = new Dictionary<Enums.MaskType, CooldownStatus>();
		foreach(var config in CooldownConfigs)
		{
			var status = cooldownStatus[config.MaskType];
			//status = -config.CooldownTime;
		}
	}

	public bool IsOnCooldown(Enums.MaskType maskType)
	{

		return false;
		//return activationTimes[maskType] currentTime - timeActivated >= CooldownTime;

	}

	float currentTime { get { return Managers.Ins.GameScript.TotalPlayingTime; } }
	public void ResetCooldown()
	{
		//timeActivated = -CooldownTime;
	}
	//public void StartCooldown();
	public float GetProgress()
	{
		return 1f;
		//return Mathf.Clamp01((currentTime + CooldownTime - timeActivated) / CooldownTime);
	}

	private void OnEnable()
	{
		ResetCooldown();
	}
}
