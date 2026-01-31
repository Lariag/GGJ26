using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelSectionConfig", menuName = "Scriptable Objects/LevelSectionConfig")]
public class LevelSectionConfig : ScriptableObject
{
	public Texture2D LevelTiles;
	public string Name;

	public int Width => LevelTiles != null ? LevelTiles.width : 0;
	public int Height => LevelTiles != null ? LevelTiles.height : 0;

	public bool Validate(int hegiht, Color[] validColors)
	{
		if (LevelTiles == null)
		{
			Debug.LogError($"LevelTiles {Name} texture is not assigned.");
			return false;
		}

		if (LevelTiles.height != hegiht)
		{
			Debug.LogError($"LevelTiles {Name} texture height is invalid.");
			return false;
		}

		for(int i = 0; i < LevelTiles.width; i++)
		{
			for(int j = 0; j < LevelTiles.height; j++)
			{
				Color pixelColor = LevelTiles.GetPixel(i, j);
				if(!validColors.Contains(pixelColor))
				{
					//Debug.LogError($"LevelTiles {Name} texture contains invalid color at ({i}, {j}): {pixelColor.r}, {pixelColor.g}, {pixelColor.b}, {pixelColor.a}.");
					//return false;
				}
			}
		}

		Debug.Log($"LevelTiles {Name} texture is correct with size ({LevelTiles.width}, {LevelTiles.height}).");
		return true;
	}
}
