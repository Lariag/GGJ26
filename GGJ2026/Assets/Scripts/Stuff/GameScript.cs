using UnityEngine;

public class GameScript : MonoBehaviour
{
	public Enums.GameState CurrentGameState = Enums.GameState.MainMenu;

	public float GravityScale = 1f;
	CharacterScript playerScript;
	LevelScript levelScript;

	private void Awake()
	{
		playerScript = FindFirstObjectByType<CharacterScript>();
		levelScript = FindFirstObjectByType<LevelScript>();
		Physics2D.gravity = Physics2D.gravity * GravityScale;
	}
	void Start()
	{
		Managers.Ins.Events.OnGameStateChanged(CurrentGameState);
	}

}
