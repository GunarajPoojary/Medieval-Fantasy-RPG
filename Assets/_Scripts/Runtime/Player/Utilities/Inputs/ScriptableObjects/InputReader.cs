using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using ProjectEmbersteel.Player.InputActions;

namespace ProjectEmbersteel.Utilities.Inputs.ScriptableObjects
{
	public enum InputActionType
	{
		Move,
		Jump,
		Run
	}

	[CreateAssetMenu(fileName = "InputReader", menuName = "Custom/Player/Input/Input Reader")]
	public class InputReader : DescriptionBaseSO, PlayerInputActions.IPlayerActions
	{
		// Assign delegate{} to events to initialise them with an empty delegate
		// so we can skip the null check when we use them

		private PlayerInputActions _playerInputActions;
		private bool _isPlayerMovementActionsEnabled = true;

		public Vector2 MoveDirection => _playerInputActions?.Player.Move.ReadValue<Vector2>() ?? Vector2.zero;

		// Gameplay
		public UnityAction<Vector2> MovePerformedAction = delegate { };
		public UnityAction MoveCanceledAction = delegate { };
		public UnityAction JumpStartedAction = delegate { };
		public UnityAction JumpPerformedAction = delegate { };
		public UnityAction JumpCanceledAction = delegate { };
		public UnityAction RunPerformedAction = delegate { };
		public UnityAction RunCanceledAction = delegate { };
		public UnityAction InventoryAction = delegate { };
		public UnityAction EquipmentMenuAction = delegate { };

		public UnityAction<bool> ToggleLookInput = delegate { };
		public UnityAction<Vector2> LookAction = delegate { };

		public void Initialize() => _playerInputActions ??= new PlayerInputActions();

		public void EnablePlayerActions()
		{
			_playerInputActions?.Player.Enable();
			_playerInputActions?.Player.SetCallbacks(this);
		}

		public void DisablePlayerActions() => _playerInputActions?.Player.Disable();

		public void EnableActionFor(InputActionType actionType)
		{
			switch (actionType)
			{
				case InputActionType.Move:
					_playerInputActions?.Player.Move.Enable();
					break;
				case InputActionType.Jump:
					_playerInputActions?.Player.Jump.Enable();
					break;
				case InputActionType.Run:
					_playerInputActions?.Player.Run.Enable();
					break;
			}
		}

		public void EnablePlayerMovementActions()
		{
			if (_playerInputActions != null)
			{
				_playerInputActions.Player.Move.Enable();
				_playerInputActions.Player.Jump.Enable();
				_playerInputActions.Player.Run.Enable();
				ToggleLookInput?.Invoke(true);
				_isPlayerMovementActionsEnabled = true;
			}
		}

		public void DisableActionFor(InputActionType actionType)
		{
			switch (actionType)
			{
				case InputActionType.Move:
					_playerInputActions?.Player.Move.Disable();
					break;
				case InputActionType.Jump:
					_playerInputActions?.Player.Jump.Disable();
					break;
				case InputActionType.Run:
					_playerInputActions?.Player.Run.Disable();
					break;
			}
		}

		public void DisablePlayerMovementActions()
		{
			if (_playerInputActions != null)
			{
				_playerInputActions.Player.Move.Disable();
				_playerInputActions.Player.Jump.Disable();
				_playerInputActions.Player.Run.Disable();
				ToggleLookInput?.Invoke(false);
				_isPlayerMovementActionsEnabled = false;
			}
		}

		public void OnMove(InputAction.CallbackContext context)
		{
			switch (context.phase)
			{
				case InputActionPhase.Performed:
					MovePerformedAction.Invoke(context.ReadValue<Vector2>());
					break;
				case InputActionPhase.Canceled:
					MoveCanceledAction.Invoke();
					break;
			}
		}

		public void OnJump(InputAction.CallbackContext context)
		{
			switch (context.phase)
			{
				case InputActionPhase.Started:
					JumpStartedAction.Invoke();
					break;
				case InputActionPhase.Performed:
					JumpPerformedAction.Invoke();
					break;
				case InputActionPhase.Canceled:
					JumpCanceledAction.Invoke();
					break;
			}
		}

		public void OnRun(InputAction.CallbackContext context)
		{
			switch (context.phase)
			{
				case InputActionPhase.Performed:
					RunPerformedAction.Invoke();
					break;
				case InputActionPhase.Canceled:
					RunCanceledAction.Invoke();
					break;
			}
		}

		public void OnInventory(InputAction.CallbackContext context)
		{
			if (context.performed)
				InventoryAction?.Invoke();
		}

		public void OnLockCursor(InputAction.CallbackContext context)
		{

		}

		public void OnEquipmentMenu(InputAction.CallbackContext context)
		{
			if (context.performed)
				EquipmentMenuAction?.Invoke();
		}

		public void OnLook(InputAction.CallbackContext context)
		{
			LookAction?.Invoke(context.ReadValue<Vector2>());
		}

		public void OnAttack(InputAction.CallbackContext context)
		{

		}

		public void OnInteract(InputAction.CallbackContext context)
		{

		}

		public void OnCrouch(InputAction.CallbackContext context)
		{

		}

		public void OnPrevious(InputAction.CallbackContext context)
		{

		}

		public void OnNext(InputAction.CallbackContext context)
		{

		}
	}
}