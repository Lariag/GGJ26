using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterScript : MonoBehaviour
{
	public Enums.MaskType CurrentMask = Enums.MaskType.None;
	public Enums.PlayerStatus CurrentStatus = Enums.PlayerStatus.MenuMode;
	public Enums.PlayerStatus PreviousStatus = Enums.PlayerStatus.MenuMode;
	public LayerMask GroundLayers;
	public LayerMask EnemyLayers;

	public float PowerJumpForce;
	public float PowerDigDistance;
	public float PowerDigHeight;
	public float PowerMiniScaleFactor = 0.2f;
	public float PowerHarmDistance;
	public float playingSpeed;
	public float menuSpeed;
	public float transitionSpeed;
	public float InvulnerabilityTime;
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
	Sonidos sonidos;
	Vector3 startingPos;

	private void Awake()
	{
		characterAnimations = GetComponentInChildren<CharacterAnimations>();
		sonidos = Camera.main.GetComponent<Sonidos>();
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
			case Enums.PlayerStatus.Dead:
				break;
		}
	}
	void OnPlayerTileCollisionEnter(Enums.TileType tileType, Enums.TileCollisionDirection collisionDirection, Vector2Int tilePosition)
	{
		if (collisionDirection == Enums.TileCollisionDirection.Front)
		{
			if (CurrentMask != Enums.MaskType.Dig)
			{
				KillPlayer(Enums.PlayerDeathType.Wall);
				Debug.Log($"Player collided frontally with {tileType}!");
			}
		}
		else
		{
			PlayerRb.linearVelocityY = 0f;
			// Debug.Log($"Player collided vertically with {tileType}!");
		}
	}

	void KillPlayer(Enums.PlayerDeathType deathType)
	{
		if (Managers.Ins.GameScript.CurrentGameState != Enums.GameState.Playing)
			return;
		if (Managers.Ins.GameScript.TotalPlayingTime < InvulnerabilityTime)
			return;

		Debug.Log($"What killed the player??? This did: {deathType}!!!");
		Managers.Ins.GameScript.SetGameState(Enums.GameState.Dying);
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
			case Enums.GameState.LevelStarting:
				//var oldPos = transform.position;
				currentSpeed = transitionSpeed;
				PlayerRb.linearVelocity = Vector2.zero;
				//DOVirtual.DelayedCall(Managers.Ins.GameScript.StartingDuration, () => Managers.Ins.Events.OnPlayerTeleport(transform.position - oldPos));
				//Managers.Ins.Events.OnPlayerTeleport(transform.position - oldPos);
				DOVirtual.Float(currentSpeed, playingSpeed, Managers.Ins.GameScript.StartingDuration, x => currentSpeed = x);
				//transform.position = startingPos;
				transform.position = new Vector3(transform.position.x, startingPos.y, transform.position.z);
				break;
			case Enums.GameState.Playing:
				if (currentGameState == Enums.GameState.Paused)
					ChangePlayerStatus(Enums.PlayerStatus.Floor);
				SetPlayerLevelStatusPlaying();
				break;
			case Enums.GameState.Paused:
				PlayerRb.simulated = false;
				break;
			case Enums.GameState.Dying:
				ChangePlayerStatus(Enums.PlayerStatus.Dead);
				// SetPlayerLevelStatusDying();
				characterAnimations.SetForMenu();
				PlayerRb.simulated = false;
				DOVirtual.Float(currentSpeed, transitionSpeed, Managers.Ins.GameScript.DeathDuration, x => currentSpeed = x);
				DOVirtual.DelayedCall(Managers.Ins.GameScript.DeathDuration, () =>
				{
					Managers.Ins.GameScript.SetGameState(Enums.GameState.GameOver);
				});
				break;
			case Enums.GameState.GameOver:
				ChangePlayerStatus(Enums.PlayerStatus.Dead);
				currentSpeed = menuSpeed;
				SetPlayerLevelStatusMainMenu();
				break;
		}
	}

	void Update()
	{
		if (currentGameState != Enums.GameState.Paused)
		{
			UseMaskPower(true, false);
			transform.Translate(currentSpeed * Time.deltaTime, 0, 0);
			if (transform.position.y < -13f)
				KillPlayer(Enums.PlayerDeathType.Fall);
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
		// Habilitar sprites.
		currentSpeed = playingSpeed;
		characterAnimations.SetForStartingGame();
		MaskChange(Enums.MaskType.None);

		PlayerRb.simulated = true;
		PlayerRb.linearVelocityY = PowerJumpForce;
	}
	#endregion Player Level Status

	void OnCollisionEnter2D(Collision2D collision)
	{
		Managers.Ins.Events.OnPlayerTilemapCollision(collision);
	}

	void OnCollisionStay2D(Collision2D collision)
	{
		Managers.Ins.Events.OnPlayerTilemapCollision(collision);
	}

	void OnCollisionExit2D(Collision2D collision)
	{
		Managers.Ins.Events.OnPlayerTilemapCollision(collision);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (CurrentMask == Enums.MaskType.Harm)
			return;

		var enemy = collision.gameObject?.GetComponentInParent<EnemyScript>();
		if(enemy != null)
		{
			KillPlayer(Enums.PlayerDeathType.Enemy);
		}
	}

	#region Masks
	void OnEnable()
	{
		Managers.Ins.Events.OnGameStateChangedEvent += OnGameStateChanged;
		Managers.Ins.Events.OnPlayerTileCollisionEnterEvent += OnPlayerTileCollisionEnter;
		Managers.Ins.Events.OnMaskEffectFinishedEvent += OnMaskPowerOver;


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
		Managers.Ins.Events.OnMaskEffectFinishedEvent -= OnMaskPowerOver;

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
		if (currentGameState != Enums.GameState.Playing)
			return;
		if (newMask == CurrentMask)
			return;
		if (Managers.Ins.Cooldown.IsOnCooldown(newMask))
			return;
		if (Managers.Ins.Cooldown.IsEffectActive(CurrentMask))
			Managers.Ins.Cooldown.CancelEffect(CurrentMask);


		var oldMask = CurrentMask;
		CurrentMask = newMask;

		// Debug.Log($"The {newMask} has been activated! Previous mask: {CurrentMask}");
		Managers.Ins.Events.OnMaskChanged(oldMask, CurrentMask);

		switch (oldMask)
		{
			case Enums.MaskType.None:
				break;
			case Enums.MaskType.Jump:
				break;
			case Enums.MaskType.Fly:
				PlayerRb.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
				break;
			case Enums.MaskType.Dig:
				break;
			case Enums.MaskType.Mini:
				sonidos.SonidoAgrandar();
				characterAnimations.MakeNormalSize(PowerMiniScaleFactor);
				break;
		}

		switch (CurrentMask)
		{
			case Enums.MaskType.None:
				characterAnimations.RunAgain();
				break;
			case Enums.MaskType.Jump:
				break;
			case Enums.MaskType.Fly:
				PlayerRb.constraints |= RigidbodyConstraints2D.FreezePositionY;
				break;
			case Enums.MaskType.Dig:
				characterAnimations.RunAgain();
				break;
			case Enums.MaskType.Mini:
				sonidos.SonidoEncoger();
				characterAnimations.MakeTiny(PowerMiniScaleFactor);
				break;
		}
		UseMaskPower(false, true);
	}

	void OnMaskPowerOver(Enums.MaskType maskType)
	{
		if (maskType == CurrentMask)
		{
			// Debug.Log($"The {maskType} mask power has ended!");
			MaskChange(Enums.MaskType.None);
		}
	}

	void UseMaskPower(bool passive, bool firstActivation)
	{
		if (!Managers.Ins.Cooldown.IsEffectActive(CurrentMask))
			return;

		bool isOnFloor = IsGrounded();
		switch (CurrentMask)
		{
			case Enums.MaskType.None:
				characterAnimations.RunAgain();
				break;
			case Enums.MaskType.Jump:
				if (!passive && isOnFloor)
				{
					PlayerRb.linearVelocityY = PowerJumpForce;
					characterAnimations.Jump(isOnFloor);
					sonidos.SonidoSalto();
				}
				else if (!passive)
				{
					// Debug.Log("Not Grounded!");
				}

				if (passive && isOnFloor)
				{
					characterAnimations.RunAgain();
				}
				break;
			case Enums.MaskType.Fly:
				characterAnimations.Fly();
				sonidos.SonidoVuelo();
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
			case Enums.MaskType.Harm:
				var hit = Physics2D.Raycast(characterAnimations.transform.position, Vector2.right, PowerHarmDistance, EnemyLayers);
				if (hit.collider != null)
				{
					var enemy = hit.collider.gameObject.GetComponentInParent<EnemyScript>();
					if (enemy != null)
					{
						enemy.KillEnemy();
						characterAnimations.Harm();
					}
				}
				break;
		}
	}

	private void OnDrawGizmos()
	{
		if (characterAnimations == null)
			return;

		//if (CurrentMask == Enums.MaskType.Dig)
		//{
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(characterAnimations.transform.position, characterAnimations.transform.position + Vector3.right * PowerDigDistance);

			var center = (characterAnimations != null ? (Vector2)characterAnimations.transform.position : (Vector2)transform.position)
						 + Vector2.right * (PowerDigDistance * 0.5f);
			var size = new Vector2(PowerDigDistance + Time.deltaTime * 3f, PowerDigHeight);

			Gizmos.color = Color.yellow;
			Gizmos.DrawWireCube(center, size);
			Gizmos.color = new Color(1f, 1f, 0f, 0.12f);
			Gizmos.DrawCube(center, size);
		//}

		Gizmos.color = Color.green;
		Gizmos.DrawLine(characterAnimations.transform.position, characterAnimations.transform.position + (Vector3.down * 1.2f));
	}

	#endregion Masks
}
