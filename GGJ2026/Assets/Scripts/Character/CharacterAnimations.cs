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

	public void WallBreak(Vector3 pos, Enums.TileType tileType)
	{
		if (ParticlesWallBreak != null
			&& tileType != Enums.TileType.None
			&& Vector3.Distance(pos, transform.position) < 1f)
		{
			ParticlesWallBreak.transform.position = new Vector3(ParticlesWallBreak.transform.position.x, pos.y, ParticlesWallBreak.transform.position.z);
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

	void OnEnable()
	{
		Managers.Ins.Events.OnTileDestroyedEvent += WallBreak;
	}
	private void OnDisable()
	{
		Managers.Ins.Events.OnTileDestroyedEvent -= WallBreak;
	}
}
