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
	public LevelTile[] LevelTiles;

	Tilemap tilemap;
	Transform player;
	Transform camera;
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
		_tileCache = LevelTiles.Where(x => x.Color != BackgroundColor && x.Color != EnemyColor).ToDictionary(x => x.Color, x => (TileBase)x.Tile);

		Debug.Log($"Found {LevelTiles.Length} valid colors: {string.Join("\n - ", _tileCache.Keys)}");

		if (levelSectionConfigs.Any(x => !x.Validate(levelHeight, _tileCache.Keys.ToArray()))) return;

	}

	private void RenderSection(LevelSectionConfig section)
	{
		for (int i = 0; i < section.Width; i++)
		{
			for (int j = 0; j < section.Height; j++)
			{
				if (_tileCache.TryGetValue(section.LevelTiles.GetPixel(i, j), out var tile))
				{
					var cell = new Vector3Int(_mostRightTile + i, j, 0);
					tilemap.SetTile(cell, tile);
				}
			}
		}
		tilemap.RefreshAllTiles();
		_mostRightTile += section.Width;
	}

}
