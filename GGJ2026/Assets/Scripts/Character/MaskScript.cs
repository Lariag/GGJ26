using UnityEngine;

public class MaskScript : MonoBehaviour
{
	Transform defaultParent;
	Transform targetTransform;

	public Enums.MaskType maskType;
	private void Awake()
	{
		defaultParent = transform.parent;
		targetTransform = GameObject.FindGameObjectWithTag("MaskTargetPosition").transform;
	}

	private void Start()
	{
		Managers.Ins.Events.OnMaskChangedEvent += OnMaskChanged;
		gameObject.SetActive(false);
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
		transform.position = targetTransform.position;
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
