using UnityEngine;

public static class Enums
{
	public enum MaskType
	{
		None,
		Dig = 1,
		Jump = 2,
		Fly = 3,
		Harm = 4,
		Mini = 5
	}

	public enum  PlayerStatus
	{
		MenuMode,
		StartingJump,
		Floor,
		Air,
		Flying,
		Water,
		Ceiling,
		Digging
	}

	public enum GameState
	{
		MainMenu,
		LevelStarting,
		Playing,
		Paused,
		GameOver
	}

	public enum TileType
	{
		None,
		Air,
		Ground,
		Floor,
		Wall,
		Platform,
	}

	public enum TileCollisionDirection
	{
		None,
		Front,
		Up,
		Bottom
	}
}
