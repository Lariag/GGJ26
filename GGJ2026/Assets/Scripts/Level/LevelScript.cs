using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

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
	float viewDistance;
	float cleanupDistance;
	private Dictionary<Color, TileBase> _tileCache;
	private int _generatedMaxX;
	private Transform camera;

	private int _posMostRightTile = -20;
	private int _posTileSectionStarted = -20;
	private int _lastTileSectionLen { get { return _posMostRightTile - _posTileSectionStarted; } }

	private Enums.GameState currentGameState = Enums.GameState.MainMenu;


	private void Awake()
	{
	}

	void Start()
	{
		tilemap = GetComponent<Tilemap>();
		camera = Camera.main.transform;
		viewDistance = Camera.main.orthographicSize * 3f;
		cleanupDistance = viewDistance;
		LoadSections();
		//RenderSection(levelSectionConfigs[0].LevelTilesAll[0]);
	}

	// Update is called once per frame
	void Update()
	{
		if (currentGameState != Enums.GameState.Paused)
		{
			ProcessLevelGeneration();
		}
	}

	private void LoadSections()
	{
		_tileCache = LevelTiles.Where(x => x.levelTile.Color != BackgroundColor && x.levelTile.Color != EnemyColor).ToDictionary(x => x.levelTile.Color, x => (TileBase)x.levelTile.Tile);

		Debug.Log($"Found {LevelTiles.Length} valid colors: {string.Join("\n - ", _tileCache.Keys)}");

		if (levelSectionConfigs.Any(x => !x.Validate(levelHeight, _tileCache.Keys.ToArray()))) return;

	}

	void ProcessLevelGeneration()
	{
		// Check if needs to render more tiles.
		var distanceToRender = camera.position.x + viewDistance - _posMostRightTile;
		if (distanceToRender < 0)
			return;

		var sectionsToRender = new List<Texture2D>();
		var distanceAvailable = 0f;

		for (int i = 0; i < 20; i++)
		{
			var sectionToRender = levelSectionConfigs[UnityEngine.Random.Range(0, levelSectionConfigs.Length)].GetRandomTiles();
			if (sectionsToRender.Contains(sectionToRender))
				continue;

			sectionsToRender.Add(sectionToRender);
			distanceAvailable += sectionToRender.width;
			if (distanceAvailable > distanceToRender + viewDistance)
				break;
		}
		//CleanUpOldTiles();

		foreach (var section in sectionsToRender)
			RenderSection(section);

	}
	private void RenderSection(Texture2D section)
	{
		var startX = tilemap.WorldToCell(new Vector3(_posMostRightTile, 0, 0)).x;
		for (int x = 0; x < section.width; x++)
		{
			for (int y = 0; y < section.height; y++)
			{
				var color = section.GetPixel(x, y);
				var cell = new Vector3Int(startX + x, y, 0);
				if (color == EnemyColor)
				{
					Managers.Ins.Events.OnEnemySpawning(tilemap.CellToWorld(cell) + Vector3.up);
				}
				else if (_tileCache.TryGetValue(color, out var tile))
				{
					tilemap.SetTile(cell, tile);
				}
				else
				{
					tilemap.SetTile(cell, null);
				}
			}
		}
		tilemap.RefreshAllTiles();
		_posMostRightTile = (int)tilemap.CellToWorld(new Vector3Int(startX + section.width, 0, 0)).x;
	}

	private void CleanUpOldTiles()
	{
		var dist = (int)(camera.position.x - viewDistance);
		if (_posTileSectionStarted >= dist)
			return;

		var bounds = new BoundsInt(
			new Vector3Int(_posTileSectionStarted, 0, 0),
			new Vector3Int(dist, levelHeight, 1)
		);

		var tiles = new TileBase[bounds.size.x * bounds.size.y];

		tilemap.SetTilesBlock(bounds, tiles);
		_posTileSectionStarted = _posMostRightTile;
	}

	void OnGameStateChanged(Enums.GameState newState)
	{
		currentGameState = newState;
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
				Debug.Log($"Tile: {tileType}, Normal: {collision.contacts[i].normal}");

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
		if (Mathf.Abs(normal.x) > Mathf.Abs(normal.y)) //) && normal.x > 0.8f)
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
		if (hit.collider == null)
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

	void OnPlayerTeleport(Vector3 distanceMoved)
	{
		_posMostRightTile += (int)(distanceMoved.x - viewDistance);
		_posTileSectionStarted += (int)(distanceMoved.x - viewDistance);
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
		Managers.Ins.Events.OnPlayerTeleportEvent += OnPlayerTeleport;
	}

	private void OnDisable()
	{
		Managers.Ins.Events.OnPlayerTilemapCollisionEvent -= OnPlayerTilemapCollision;
		Managers.Ins.Events.OnPlayerDigEvent -= OnPlayerDig;
		Managers.Ins.Events.OnPlayerMultiDigEvent -= OnPlayerMultiDig;
		Managers.Ins.Events.OnPlayerTeleportEvent -= OnPlayerTeleport;
	}
}
