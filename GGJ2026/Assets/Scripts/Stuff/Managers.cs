using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Managers : MonoBehaviour
{
    public static Managers Ins { get; private set; }
	public GameEvents Events;


	void Awake()
    {
        Ins = Util.MakeSingleton(this);
		Events = this.AddComponent<GameEvents>();
	}

	private void Start()
	{
		Console.WriteLine($"Managers starting. Current scene: {SceneManager.GetActiveScene().name}");
		if(SceneManager.GetActiveScene().name != "GameScene")
			SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
	}
}
