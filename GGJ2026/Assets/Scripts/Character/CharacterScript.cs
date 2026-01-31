using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class CharacterScript : MonoBehaviour
{
	public Enums.MaskType CurrentMask = Enums.MaskType.None;
	public Enums.PlayerStatus CurrentStatus = Enums.PlayerStatus.MenuMode;
	public Enums.PlayerStatus PreviousStatus = Enums.PlayerStatus.MenuMode;

	public float jumpForce;
	public float playingSpeed;
	public float menuSpeed;
	private float currentSpeed;

	public Transform MaskPositioner;

	Transform camera;

	InputAction ActionMask1;
	InputAction ActionMask2;
	InputAction ActionMask3;
	InputAction ActionMask4;
	InputAction ActionMask5;
	InputAction ActionPower;

	private Enums.GameState currentGameState = Enums.GameState.MainMenu;
	private Vector3 defaultPlayerScale;

	public Rigidbody2D PlayerRb;
	public Transform PlayerTransform;
	public float MiniaturePlayerScaleFactor = 0.2f;

	private void Awake()
	{
		camera = Camera.main.transform;
		defaultPlayerScale = PlayerTransform.localScale;
	}

	private void Start()
	{
	}

	void ChangePlayerStatus(Enums.PlayerStatus newStatus)
	{
		PreviousStatus = CurrentStatus;
		CurrentStatus = newStatus;

		switch (CurrentStatus)
		{
			case Enums.PlayerStatus.MenuMode:
				break;
			case Enums.PlayerStatus.Floor:
				break;
			case Enums.PlayerStatus.Air:
				break;
			case Enums.PlayerStatus.Flying:
				break;
			case Enums.PlayerStatus.Water:
				break;
			case Enums.PlayerStatus.Ceiling:
				break;
			case Enums.PlayerStatus.Digging:
				break;
		}
	}
	void OnPlayerTileCollisionEnter(Enums.TileType tileType, Enums.TileCollisionDirection collisionDirection, Vector2Int tilePosition)
	{
		if(collisionDirection == Enums.TileCollisionDirection.Front)
		{
			Debug.Log($"Player collided frontally with {tileType}!");
		}
		else
		{
			Debug.Log($"Player collided vertically with {tileType}!");
		}
	}

	void OnGameStateChanged(Enums.GameState newState)
	{
		Debug.Log($"New player game state: {newState}");
		currentGameState = newState;
		switch (newState)
		{
			case Enums.GameState.MainMenu:
				ChangePlayerStatus(Enums.PlayerStatus.MenuMode);
				SetPlayerLevelStatusMainMenu();
				break;
			case Enums.GameState.Playing:
				ChangePlayerStatus(Enums.PlayerStatus.StartingJump);
				SetPlayerLevelStatusPlaying();
				break;
			case Enums.GameState.Paused:

				break;
			case Enums.GameState.GameOver:
				ChangePlayerStatus(Enums.PlayerStatus.MenuMode);
				break;
		}
	}

	void Update()
	{
		if(currentGameState != Enums.GameState.Paused)
		{
			transform.Translate(currentSpeed * Time.deltaTime, 0, 0);
		}
	}

	#region Player Level Status

	void SetPlayerLevelStatusMainMenu()
		{
		currentSpeed = menuSpeed;
		PlayerTransform.gameObject.SetActive(false);
		PlayerRb.simulated = false;
		// Deshabilitar sprites y fisicas.
		// Poner movimiento de menu.
	}
	void SetPlayerLevelStatusPlaying()
	{
		// Avanzar rápido unos metros y poner velocidad de juego.
		currentSpeed = playingSpeed;
		// Habilitar sprites.
		PlayerTransform.gameObject.SetActive(true);
		// Colocar al jugador por atras y "lanzarlo" al juego.
		// Habilitar físicas.
		PlayerRb.simulated = true;
	}
	#endregion Player Level Status

	private void OnCollisionEnter2D(Collision2D collision)
	{
		Managers.Ins.Events.OnPlayerTilemapCollision(collision);
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
		Managers.Ins.Events.OnPlayerTilemapCollision(collision);
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		Managers.Ins.Events.OnPlayerTilemapCollision(collision);
	}

	#region Masks
	void OnEnable()
	{
		Managers.Ins.Events.OnGameStateChangedEvent += OnGameStateChanged;
		Managers.Ins.Events.OnPlayerTileCollisionEnterEvent += OnPlayerTileCollisionEnter;


		ActionMask1 = InputSystem.actions.FindAction("Mask1");
		ActionMask2 = InputSystem.actions.FindAction("Mask2");
		ActionMask3 = InputSystem.actions.FindAction("Mask3");
		ActionMask4 = InputSystem.actions.FindAction("Mask4");
		ActionMask5 = InputSystem.actions.FindAction("Mask5");
		ActionPower = InputSystem.actions.FindAction("Jump");

		if (ActionMask1 != null) ActionMask1.performed += OnMask1;
		if (ActionMask2 != null) ActionMask2.performed += OnMask2;
		if (ActionMask3 != null) ActionMask3.performed += OnMask3;
		if (ActionMask4 != null) ActionMask4.performed += OnMask4;
		if (ActionMask5 != null) ActionMask5.performed += OnMask5;
		if (ActionPower != null) ActionPower.performed += OnActionPower;
	}

	void OnDisable()
	{
		Managers.Ins.Events.OnGameStateChangedEvent -= OnGameStateChanged;
		Managers.Ins.Events.OnPlayerTileCollisionEnterEvent -= OnPlayerTileCollisionEnter;

		if (ActionMask1 != null) ActionMask1.performed -= OnMask1;
		if (ActionMask2 != null) ActionMask2.performed -= OnMask2;
		if (ActionMask3 != null) ActionMask3.performed -= OnMask3;
		if (ActionMask4 != null) ActionMask4.performed -= OnMask4;
		if (ActionMask5 != null) ActionMask5.performed -= OnMask5;
		if (ActionPower != null) ActionPower.performed -= OnActionPower;
	}
	private void OnMask1(InputAction.CallbackContext ctx) => MaskChange(Enums.MaskType.Dig);
	private void OnMask2(InputAction.CallbackContext ctx) => MaskChange(Enums.MaskType.Jump);
	private void OnMask3(InputAction.CallbackContext ctx) => MaskChange(Enums.MaskType.Fly);
	private void OnMask4(InputAction.CallbackContext ctx) => MaskChange(Enums.MaskType.Harm);
	private void OnMask5(InputAction.CallbackContext ctx) => MaskChange(Enums.MaskType.Mini);
	private void OnActionPower(InputAction.CallbackContext ctx) => UseMaskPower();
	void MaskChange(Enums.MaskType newMask)
	{
		if(newMask == CurrentMask)
			return;
		Debug.Log($"The {newMask} has been activated! Previous mask: {CurrentMask}");
		var oldMask = CurrentMask;
		CurrentMask = newMask;
		Managers.Ins.Events.OnMaskChanged(CurrentMask);

		switch(CurrentMask)
		{
			case Enums.MaskType.None:
				PlayerTransform.localScale = defaultPlayerScale;
				break;
			case Enums.MaskType.Jump:
				PlayerTransform.localScale = defaultPlayerScale;
				break;
			case Enums.MaskType.Fly:
				PlayerTransform.localScale = defaultPlayerScale;
				break;
			case Enums.MaskType.Dig:
				PlayerTransform.localScale = defaultPlayerScale;
				break;
			case Enums.MaskType.Mini:
				PlayerTransform.localScale = defaultPlayerScale * MiniaturePlayerScaleFactor;
				break;
		}
		UseMaskPower();
	}

	void UseMaskPower()
	{
		Debug.Log($"Mask {CurrentMask} power activated!");

		switch(CurrentMask)
		{
			case Enums.MaskType.None:
				break;
			case Enums.MaskType.Jump:
				PlayerRb.linearVelocityY = jumpForce;
				break;
			case Enums.MaskType.Fly:
				break;
			case Enums.MaskType.Dig:
				break;
			case Enums.MaskType.Mini:
				break;
		}
	}
	#endregion Masks
}
