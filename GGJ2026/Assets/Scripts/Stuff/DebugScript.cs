using UnityEngine;

public class DebugScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Debug controls: \n - G: StartLevel\n - H: MainMenu)");
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.F))
		{
			Managers.Ins.GameScript.SetGameState(Enums.GameState.LevelStarting);
		}
		if (Input.GetKeyDown(KeyCode.G))
        {
            Managers.Ins.GameScript.SetGameState(Enums.GameState.Playing);
		}
		if (Input.GetKeyDown(KeyCode.H))
		{
			Managers.Ins.GameScript.SetGameState(Enums.GameState.MainMenu);
		}
		if (Input.GetKeyDown(KeyCode.P))
		{
			Managers.Ins.GameScript.SetGameState(Enums.GameState.Paused);
		}
	}
}
