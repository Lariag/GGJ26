using UnityEngine;

public class CharacterAnimations : MonoBehaviour
{
	Vector3 defaultScale;
	public ParticleSystem ParticlesJump;
	public ParticleSystem ParticlesWallBreak;
	public ParticleSystem ParticlesFly;
	public ParticleSystem[] ParticlesHit;

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
		{
			ParticlesWallBreak.Stop();
			ParticlesWallBreak.Play();
		}
	}

public void Jump(bool fromGround)
{
	if (ParticlesJump != null)
	{
		ParticlesJump.Stop();
		ParticlesJump.Play();
	}
}

public void Fly()
{
	if (ParticlesFly != null)
	{
		ParticlesFly.Stop();
		ParticlesFly.Play();
	}
}

}
