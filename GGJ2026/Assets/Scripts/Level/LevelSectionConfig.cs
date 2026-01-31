using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelSectionConfig", menuName = "Scriptable Objects/LevelSectionConfig")]
public class LevelSectionConfig : ScriptableObject
{
	public Texture2D[] LevelTilesAll;
	public string Name;

	//public int Width => LevelTilesAll != null ? LevelTilesAll[0].width : 0;
	//public int Height => LevelTilesAll != null ? LevelTilesAll[0].height : 0;

	public Texture2D GetRandomTiles()
	{
		return LevelTilesAll[Random.Range(0, LevelTilesAll.Length)];
	}
	public bool Validate(int hegiht, Color[] validColors)
	{
		if (LevelTilesAll == null || LevelTilesAll.Any(x => x == null))
		{
			Debug.LogError($"LevelTiles {Name} texture is not assigned.");
			return false;
		}

		foreach (var LevelTiles in LevelTilesAll)
		{
			if (LevelTiles.height != hegiht)
			{
				Debug.LogError($"LevelTiles {Name} texture height is invalid.");
				return false;
			}

			for (int i = 0; i < LevelTiles.width; i++)
			{
				for (int j = 0; j < LevelTiles.height; j++)
				{
					Color pixelColor = LevelTiles.GetPixel(i, j);
					if (!validColors.Contains(pixelColor))
					{
						//Debug.LogError($"LevelTiles {Name} texture contains invalid color at ({i}, {j}): {pixelColor.r}, {pixelColor.g}, {pixelColor.b}, {pixelColor.a}.");
						//return false;
					}
				}
			}
		}

		Debug.Log($"LevelTiles {Name} textures are correct.");
		return true;
	}
}
