using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterScript : MonoBehaviour
{

	public Enums.MaskType CurrenMask = Enums.MaskType.None;
	public Enums.PlayerStatus CurrentStatus = Enums.PlayerStatus.Floor;
	public Enums.PlayerStatus PreviousStatus = Enums.PlayerStatus.Floor;

	public float defaultSpeed;

	InputAction ActionMask1;
	InputAction ActionMask2;
	InputAction ActionMask3;
	InputAction ActionMask4;
	InputAction ActionPower;

	void OnEnable()
	{
		ActionMask1 = InputSystem.actions.FindAction("Mask1");
		ActionMask2 = InputSystem.actions.FindAction("Mask2");
		ActionMask3 = InputSystem.actions.FindAction("Mask3");
		ActionMask4 = InputSystem.actions.FindAction("Mask4");
		ActionPower = InputSystem.actions.FindAction("Jump");

		if (ActionMask1 != null) ActionMask1.performed += OnMask1;
		if (ActionMask2 != null) ActionMask2.performed += OnMask2;
		if (ActionMask3 != null) ActionMask3.performed += OnMask3;
		if (ActionMask4 != null) ActionMask4.performed += OnMask4;
		if (ActionPower != null) ActionPower.performed += OnActionPower;
	}

	void OnDisable()
	{
		if (ActionMask1 != null) ActionMask1.performed -= OnMask1;
		if (ActionMask2 != null) ActionMask2.performed -= OnMask2;
		if (ActionMask3 != null) ActionMask3.performed -= OnMask3;
		if (ActionMask4 != null) ActionMask4.performed -= OnMask4;
		if (ActionPower != null) ActionPower.performed -= OnActionPower;
	}

	void Update()
	{
	}

	private void OnMask1(InputAction.CallbackContext ctx) => MaskChange(1);
	private void OnMask2(InputAction.CallbackContext ctx) => MaskChange(2);
	private void OnMask3(InputAction.CallbackContext ctx) => MaskChange(3);
	private void OnMask4(InputAction.CallbackContext ctx) => MaskChange(4);
	private void OnActionPower(InputAction.CallbackContext ctx) => Debug.Log($"JUMP");
	void MaskChange(int newMask)
	{
		Debug.Log($"{newMask} activated");
	}
}
