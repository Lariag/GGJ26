using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ScriptCanvas : MonoBehaviour
{
	//-------------VARIABLES-------------
	[SerializeField]
	GameObject[] elementosColor1, elementosColor2;

	[SerializeField]
	TextMeshProUGUI[] textos;

	[SerializeField]
	Color[] color1, color2;

	public GameObject PanelPausa;
	public GameObject PanelGameOver;
	public GameObject Edificios;

	public TextMeshProUGUI scoreTxt;

	InputAction ActionPause;
	//--------------------------------------------

	private void Start()
	{
		Managers.Ins.Events.OnGameStateChangedEvent += GameStateChanged;
		ActionPause = InputSystem.actions.FindAction("Pause");
		if (ActionPause != null) ActionPause.performed += OnPausePressed;
		EscogerPaleta();
	}
	public void SalirJuego()
	{
		Application.Quit();
		Debug.Log("Salir");
	}

	public void EscogerPaleta()
	{
		int paleta = Random.Range(0, color1.Length);

		for (int i = 0; i < elementosColor1.Length; i++)
		{
			elementosColor1[i].GetComponent<Image>().color = color1[paleta];
		}
		for (int i = 0; i < textos.Length; i++)
		{
			textos[i].GetComponent<TextMeshProUGUI>().color = color1[paleta];
		}
		for (int i = 0; i < elementosColor2.Length; i++)
		{
			elementosColor2[i].GetComponent<Image>().color = color2[paleta];
		}

		Edificios.GetComponent<EdificiosDelFondo>().CambiarColoresDeEdificios(color1[paleta], color2[paleta]);
	}

	public void PlayPressed()
	{
		Managers.Ins.GameScript.SetGameState(Enums.GameState.LevelStarting);
		DOVirtual.DelayedCall(Managers.Ins.GameScript.StartingDuration, () => Managers.Ins.GameScript.SetGameState(Enums.GameState.Playing));
	}
	public void MainMenuPressed()
	{
		Managers.Ins.GameScript.SetGameState(Enums.GameState.MainMenu);
	}
	private void OnPausePressed(InputAction.CallbackContext ctx) => TogglePause();

	public void TogglePause()
	{
		if (Managers.Ins.GameScript.CurrentGameState == Enums.GameState.Paused)
		{
			Managers.Ins.GameScript.SetGameState(Enums.GameState.Playing);
		}
		else if (Managers.Ins.GameScript.CurrentGameState == Enums.GameState.Playing)
		{
			Managers.Ins.GameScript.SetGameState(Enums.GameState.Paused);
		}
	}

	void GameStateChanged(Enums.GameState newGameState)
	{
		switch (newGameState)
		{
			case Enums.GameState.Paused:
				PanelPausa.SetActive(true);
				break;
			case Enums.GameState.LevelStarting:
			case Enums.GameState.Playing:
				PanelPausa.SetActive(false);
				PanelGameOver.SetActive(false);
				break;
			case Enums.GameState.GameOver:
				var seconds = Managers.Ins.GameScript.TotalPlayingTime;
				scoreTxt.text = $"{(int)seconds}";
				PanelGameOver.SetActive(true);
				break;
		}
	}

}