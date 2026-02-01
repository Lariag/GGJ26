using UnityEngine;

public class GameScript : MonoBehaviour
{
	public Enums.GameState CurrentGameState = Enums.GameState.MainMenu;
	public bool IsPaused { get { return CurrentGameState == Enums.GameState.Paused; } }
	public GameObject[] Enemies;

	public float DeathDuration;
	public float StartingDuration;
	public float TotalPlayingTime { get; private set; } = 0f;
	public float GravityScale = 1f;
	private void Awake()
	{
		Physics2D.gravity = Physics2D.gravity * GravityScale;
	}
	void Start()
	{
		Managers.Ins.Events.OnGameStateChanged(CurrentGameState);
	}
	private void Update()
	{
		if(CurrentGameState == Enums.GameState.Playing)
			TotalPlayingTime += Time.deltaTime;
	}

	public void SetGameState(Enums.GameState newState)
	{
		if (CurrentGameState != newState)
		{
			CurrentGameState = newState;
			Managers.Ins.Events.OnGameStateChanged(CurrentGameState);
		}

		if (newState == Enums.GameState.LevelStarting || newState == Enums.GameState.Playing)
			TotalPlayingTime = 0f;
	}
	void OnEnemySpawning(Vector3 position)
	{
		Instantiate(Enemies[Random.Range(0, Enemies.Length)], position, Quaternion.identity);
	}

	private void OnEnable()
	{
		Managers.Ins.Events.OnEnemySpawningEvent += OnEnemySpawning;
	}
	private void OnDisable()
	{
		Managers.Ins.Events.OnEnemySpawningEvent -= OnEnemySpawning;
	}

}
