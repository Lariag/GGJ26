using UnityEngine;

public class FollowObject : MonoBehaviour
{
	public bool UseRelativeInitialPosition;
	public Transform TargetObject;

	public bool FollowX = true;
	public bool FollowY = true;
	public bool FollowRotation = true;

	private Vector3 relativePosition;

	private void Awake()
	{
		if (UseRelativeInitialPosition)
		{
				relativePosition = TargetObject.position - transform.position;
		}
	}

	private void Update()
	{
		transform.position = new Vector3(
			FollowX ? (TargetObject.position.x - relativePosition.x) : transform.position.x,
			FollowY ? (TargetObject.position.y - relativePosition.y) : transform.position.y,
			transform.position.z
		);

		if (FollowRotation)
		{
			transform.rotation = TargetObject.rotation;
		}
	}
}
