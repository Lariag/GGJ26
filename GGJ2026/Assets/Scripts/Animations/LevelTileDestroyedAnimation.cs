using System;
using System.Linq;
using UnityEngine;

public class LevelTileDestroyedAnimation : MonoBehaviour
{
	public TileSpriteWithType[] TileSprites;

	[Serializable]
	public struct TileSpriteWithType
	{
		public Sprite sprite;
		public Enums.TileType tileType;
	}
	private void Awake()
	{
	}

	void OnLevelTileDestroyed(Vector3 position, Enums.TileType tileType)
	{
		var sprite = TileSprites.FirstOrDefault(x => x.tileType == tileType).sprite;
		if(sprite != null)
		{
			// TODO: Play tile destroyed particles with the tile sprite.
		}
	}

	private void OnEnable()
	{
		Managers.Ins.Events.OnTileDestroyedEvent += OnLevelTileDestroyed;
	}
	private void OnDisable()
	{
		Managers.Ins.Events.OnTileDestroyedEvent -= OnLevelTileDestroyed;
	}
}
