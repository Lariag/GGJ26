using UnityEngine;

public class CharacterAnimations : MonoBehaviour
{
	Vector3 defaultScale;
	public Transform ParticlesJump;
	public Transform ParticlesWallBreak;

	private void Awake()
	{
		defaultScale = transform.localScale;
	}

	public void MakeTiny(float scaleFactor)
	{
		// TODO: Partículas de encogimiento.
		transform.localScale = defaultScale * scaleFactor;
	}
	public void MakeNormalSize(float scaleFactor)
	{
		// TODO: Partículas de crecimiento.
		transform.localScale = defaultScale;
	}

	public void SetForMenu()
	{
		gameObject.SetActive(false);
	}
	public void SetForStartingGame()
	{
		gameObject.SetActive(true);
	}

	public void WallBreak()
	{
		if (ParticlesWallBreak != null)
			ParticlesWallBreak.position = transform.position;
	}
	public void Jump(bool fromGround)
	{
		// TODO: Accionar animación de salto.
		if (ParticlesWallBreak != null)
			ParticlesJump.position = transform.position;
	}

}
