using UnityEngine;

public class MaskScript : MonoBehaviour
{
	Transform defaultParent;
	Transform targetTransform;


	public Enums.MaskType maskType;
	public float PowerDuration;
	public float PowerCooldown;

	private void Awake()
	{
		defaultParent = transform.parent;
		targetTransform = GameObject.FindGameObjectWithTag("MaskTargetPosition").transform;
		Managers.Ins.Cooldown.AddConfig(maskType, PowerCooldown, PowerDuration);
	}

	private void Start()
	{
		Managers.Ins.Events.OnMaskChangedEvent += OnMaskChanged;
		gameObject.SetActive(false);
	}

	private void Update()
	{
		transform.localPosition = new Vector3(targetTransform.localPosition.x, targetTransform.localPosition.y, 1f);
		transform.localRotation = targetTransform.localRotation;
	}
	public void OnMaskChanged(Enums.MaskType newMask)
	{
		if (maskType == newMask)
			PutOnMask();
		else
			HideMask();
	}

	public void PutOnMask()
	{
		// TODO: Animate mask putting on.
		gameObject.SetActive(true);
	}

	public void HideMask()
	{
		// TODO: Animate mask taking off.
		gameObject.SetActive(false);
	}
	public void FallMask(Transform targetParent)
	{

	}

	public void AnimateAction()
	{

	}


	void OnDestroy()
	{
		Managers.Ins.Events.OnMaskChangedEvent -= OnMaskChanged;
	}
}
