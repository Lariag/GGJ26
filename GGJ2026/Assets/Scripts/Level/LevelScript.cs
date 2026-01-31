using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

public class LevelScript : MonoBehaviour
{
	public int levelHeight;
	public Color BackgroundColor;
	public Color EnemyColor;
	public LevelSectionConfig[] levelSectionConfigs;
	public LevelTileWithType[] LevelTiles;

	[Serializable]
	public struct LevelTileWithType
	{
		public LevelTile levelTile;
		public Enums.TileType tileType;
	}

	Tilemap tilemap;
	public int viewDistance = 30;
	public int cleanupDistance = 20;
	private Dictionary<Color, TileBase> _tileCache;
	private int _generatedMaxX;

	private int _mostRightTile = 0;
	private int _posStarted = 0;



	private void Awake()
	{
		//player = GameObject.FindGameObjectWithTag("Player").transform;
		//camera = Camera.main.transform;
	}

	void Start()
	{
		tilemap = GetComponent<Tilemap>();
		LoadSections();
		RenderSection(levelSectionConfigs[0]);
	}

	// Update is called once per frame
	void Update()
	{

	}

	private void LoadSections()
	{
		_tileCache = LevelTiles.Where(x => x.levelTile.Color != BackgroundColor && x.levelTile.Color != EnemyColor).ToDictionary(x => x.levelTile.Color, x => (TileBase)x.levelTile.Tile);

		Debug.Log($"Found {LevelTiles.Length} valid colors: {string.Join("\n - ", _tileCache.Keys)}");

		if (levelSectionConfigs.Any(x => !x.Validate(levelHeight, _tileCache.Keys.ToArray()))) return;

	}

	private void RenderSection(LevelSectionConfig section)
	{
		for (int x = 0; x < section.Width; x++)
		{
			for (int y = 0; y < section.Height; y++)
			{
				if (_tileCache.TryGetValue(section.LevelTiles.GetPixel(x, y), out var tile))
				{
					var cell = new Vector3Int(_mostRightTile + x, y, 0);
					tilemap.SetTile(cell, tile);
				}
			}
		}
		tilemap.RefreshAllTiles();
		_mostRightTile += section.Width;
	}

	List<Vector3> debugCollidingTiles = new List<Vector3>(10);
	List<Vector3Int> lastTileCollisions = new(10);
	List<Vector3Int> newTileCollisions = new(10);

	void OnPlayerTilemapCollision(Collision2D collision)
	{
		debugCollidingTiles.Clear();
		newTileCollisions.Clear();
		for (int i = 0; i < collision.contacts.Length; i++)
		{
			Vector3 worldPoint = collision.contacts[i].point;
			Vector3 inCellWorldPoint = worldPoint - (Vector3)(collision.contacts[i].normal / 2f);
			Vector3Int cell = tilemap.WorldToCell(inCellWorldPoint);
			TileBase tile = tilemap.GetTile(cell);
			if (tile == null) continue;

			Sprite tileSprite = tilemap.GetSprite(cell);
			debugCollidingTiles.Add(inCellWorldPoint + Vector3.forward * 5);
			newTileCollisions.Add(cell);
			if (lastTileCollisions.Contains(cell))
			{
				//OnTileCollisionStay(cell, collision, i, Enums.TileType.None);
				lastTileCollisions.Remove(cell);
			}
			else
			{
				var tileType = LevelTiles.FirstOrDefault(x => x.levelTile.Tile == tile).tileType;
				Managers.Ins.Events.OnPlayerTileCollisionEnter(tileType, GetCollisionDirection(collision.contacts[i].normal), (Vector2Int)cell);
				//Debug.Log($"Collision entered with normal: {}");
				//OnTileCollisionEnter(cell, collision, i, Enums.TileType.None);
			}

			foreach (var lastCell in lastTileCollisions)
			{
				//OnTileCollisionLeave(lastCell, collision, i, Enums.TileType.None);
			}
			lastTileCollisions.Clear();
			lastTileCollisions.AddRange(newTileCollisions);

			//contactsString += $"\n -> cell: {cell}, normal: {contact.normal}, tile: {(tile != null ? tile.name : "null")}, sprite: {(tileSprite != null ? tileSprite.name : "null")}";
		}
		//Debug.Log($"Tilemap has {collision.contacts.Length} collision: {contactsString}");
	}

	Enums.TileCollisionDirection GetCollisionDirection(Vector2 normal)
	{
		if (Mathf.Abs(normal.x) > Mathf.Abs(normal.y))
		{
			return Enums.TileCollisionDirection.Front;
		}
		else
		{
			if (normal.y > 0)
				return Enums.TileCollisionDirection.Bottom;
			else
				return Enums.TileCollisionDirection.Up;
		}
	}

	List<Vector3> lastDigPositions = new(100);
	void OnPlayerDig(RaycastHit2D hit)
	{
		if(hit.collider == null)
			return;

		Vector3 inCellWorldPoint = hit.point - (hit.normal / 2f);
		Vector3Int cell = tilemap.WorldToCell(inCellWorldPoint);
		TileBase tile = tilemap.GetTile(cell);
		var tileType = LevelTiles.FirstOrDefault(x => x.levelTile.Tile == tile).tileType;
		if (tile != null)
		{
			tilemap.SetTile(cell, null);
			tilemap.RefreshTile(cell);
			Managers.Ins.Events.OnTileDestroyed(tilemap.CellToWorld(cell), tileType);
		}
		lastDigPositions.Add(inCellWorldPoint);
	}

	void OnPlayerMultiDig(RaycastHit2D[] hits)
	{
		lastDigPositions.Clear();
		foreach (var hit in hits)
		{
			OnPlayerDig(hit);
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		foreach (var col in debugCollidingTiles)
			Gizmos.DrawSphere(col, 0.2f);

		Gizmos.color = Color.yellow;
		foreach (var lastDigPosition in lastDigPositions)
			Gizmos.DrawSphere(lastDigPosition, 0.3f);
	}

	private void OnEnable()
	{
		Managers.Ins.Events.OnPlayerTilemapCollisionEvent += OnPlayerTilemapCollision;
		Managers.Ins.Events.OnPlayerDigEvent += OnPlayerDig;
		Managers.Ins.Events.OnPlayerMultiDigEvent += OnPlayerMultiDig;
	}

	private void OnDisable()
	{
		Managers.Ins.Events.OnPlayerTilemapCollisionEvent -= OnPlayerTilemapCollision;
		Managers.Ins.Events.OnPlayerDigEvent -= OnPlayerDig;
		Managers.Ins.Events.OnPlayerMultiDigEvent -= OnPlayerMultiDig;
	}
}
