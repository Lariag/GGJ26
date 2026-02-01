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
	public LayerMask GroundLayers;

	public float PowerJumpForce;
	public float PowerDigDistance;
	public float PowerDigHeight;
	public float PowerMiniScaleFactor = 0.2f;
	public float playingSpeed;
	public float menuSpeed;
	private float currentSpeed;

	public bool EnableDeath = true;

	public Transform MaskPositioner;

	InputAction ActionMask1;
	InputAction ActionMask2;
	InputAction ActionMask3;
	InputAction ActionMask4;
	InputAction ActionMask5;
	InputAction ActionPower;

	private Enums.GameState currentGameState = Enums.GameState.MainMenu;

	public Rigidbody2D PlayerRb;
	CharacterAnimations characterAnimations;

	private void Awake()
	{
		characterAnimations = GetComponentInChildren<CharacterAnimations>();
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
		if (collisionDirection == Enums.TileCollisionDirection.Front)
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
		if (currentGameState != Enums.GameState.Paused)
		{
			UseMaskPower(true, false);
			transform.Translate(currentSpeed * Time.deltaTime, 0, 0);
		}
	}

	#region Player Level Status

	bool IsGrounded()
	{
		RaycastHit2D hit = Physics2D.Raycast(characterAnimations.transform.position, Vector2.down, 1.2f, GroundLayers);
		return hit.transform != null;
	}
	void SetPlayerLevelStatusMainMenu()
	{
		currentSpeed = menuSpeed;
		characterAnimations.SetForMenu();
		PlayerRb.simulated = false;
		// Deshabilitar sprites y fisicas.
		// Poner movimiento de menu.
	}
	void SetPlayerLevelStatusPlaying()
	{
		// Avanzar rápido unos metros y poner velocidad de juego.
		currentSpeed = playingSpeed;
		// Habilitar sprites.
		characterAnimations.SetForStartingGame();
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
	private void OnActionPower(InputAction.CallbackContext ctx) => UseMaskPower(false, false);
	void MaskChange(Enums.MaskType newMask)
	{
		if (newMask == CurrentMask)
			return;
		Debug.Log($"The {newMask} has been activated! Previous mask: {CurrentMask}");
		var oldMask = CurrentMask;
		CurrentMask = newMask;
		Managers.Ins.Events.OnMaskChanged(CurrentMask);

		switch (oldMask)
		{
			case Enums.MaskType.Jump:
				break;
			case Enums.MaskType.Fly:
				PlayerRb.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
				break;
			case Enums.MaskType.Dig:
				break;
			case Enums.MaskType.Mini:
				characterAnimations.MakeNormalSize(PowerMiniScaleFactor);
				break;

		}

		switch (CurrentMask)
		{
			case Enums.MaskType.None:
				break;
			case Enums.MaskType.Jump:
				break;
			case Enums.MaskType.Fly:
				PlayerRb.constraints |= RigidbodyConstraints2D.FreezePositionY;
				break;
			case Enums.MaskType.Dig:
				break;
			case Enums.MaskType.Mini:
				characterAnimations.MakeTiny(PowerMiniScaleFactor);
				break;
		}
		UseMaskPower(false, true);
	}

	void UseMaskPower(bool passive, bool firstActivation)
	{
		bool isOnFloor = IsGrounded();
		switch (CurrentMask)
		{
			case Enums.MaskType.None:
				break;
			case Enums.MaskType.Jump:
				if (!passive && isOnFloor)
				{
					PlayerRb.linearVelocityY = PowerJumpForce;
					characterAnimations.Jump(isOnFloor);
				}
				else if (!passive)
				{
					Debug.Log("Not Grounded!");
				}
				break;
			case Enums.MaskType.Fly:
				characterAnimations.Fly();
				break;
			case Enums.MaskType.Dig:
				var area2dhit = Physics2D.BoxCastAll(
					(Vector2)characterAnimations.transform.position + Vector2.right * (PowerDigDistance * 0.5f),
					new Vector2(PowerDigDistance + Time.deltaTime * 3f, PowerDigHeight),
					0f, Vector2.right, 0f, GroundLayers);

				if (area2dhit.Length > 0)
					Managers.Ins.Events.OnPlayerMultiDig(area2dhit);
				break;
			case Enums.MaskType.Mini:
				break;
		}
	}

	private void OnDrawGizmos()
	{
		if (CurrentMask == Enums.MaskType.Dig)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(characterAnimations.transform.position, characterAnimations.transform.position + Vector3.right * PowerDigDistance);

			var center = (characterAnimations != null ? (Vector2)characterAnimations.transform.position : (Vector2)transform.position)
						 + Vector2.right * (PowerDigDistance * 0.5f);
			var size = new Vector2(PowerDigDistance + Time.deltaTime * 3f, PowerDigHeight);

			Gizmos.color = Color.yellow;
			Gizmos.DrawWireCube(center, size);
			Gizmos.color = new Color(1f, 1f, 0f, 0.12f);
			Gizmos.DrawCube(center, size);
		}

		Gizmos.color = Color.green;
		Gizmos.DrawLine(characterAnimations.transform.position, characterAnimations.transform.position + (Vector3.down * 1.2f));
	}

	#endregion Masks
}
