using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterScript : MonoBehaviour
{
	public Enums.MaskType CurrenMask = Enums.MaskType.None;
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

	public Rigidbody2D PlayerRb;
	public Transform PlayerTransform;

	private void Awake()
	{
		camera = Camera.main.transform;
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

	#region Inputs
	void OnEnable()
	{
		Managers.Ins.Events.OnGameStateChangedEvent += OnGameStateChanged;

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
		if(newMask == CurrenMask)
			return;
		Debug.Log($"The {newMask} has been activated!");
		CurrenMask = newMask;
		Managers.Ins.Events.OnMaskChanged(CurrenMask);
	}

	void UseMaskPower()
	{
		Debug.Log($"Mask {CurrenMask} power activated!");

		switch(CurrenMask)
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
	#endregion Inputs
}
