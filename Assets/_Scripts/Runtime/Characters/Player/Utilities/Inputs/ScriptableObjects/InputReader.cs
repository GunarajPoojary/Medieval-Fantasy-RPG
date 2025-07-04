using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.Events;
using ProjectEmbersteel.Player.InputActions;

namespace ProjectEmbersteel.Utilities.Inputs.ScriptableObjects
{
	[CreateAssetMenu(fileName = "InputReader", menuName = "Custom/Player/Input/Input Reader")]
	public class InputReader : DescriptionBaseSO, PlayerInputActions.IPlayerActions
	{
		private PlayerInputActions _playerInputActions;

		public Vector2 MoveDirection => _playerInputActions?.Player.Move.ReadValue<Vector2>() ?? Vector2.zero;

		public UnityAction<Vector2> MovePerformedAction = delegate { };
		public UnityAction MoveCanceledAction = delegate { };
		public UnityAction JumpPerformedAction = delegate { };
		public UnityAction JumpCanceledAction = delegate { };
		public UnityAction RunPerformedAction = delegate { };
		public UnityAction RunCanceledAction = delegate { };
		public UnityAction InventoryAction = delegate { };
		public UnityAction InteractPerformedAction = delegate { };
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
				case InputActionType.Interact:
					_playerInputActions?.Player.Interact.Enable();
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
				case InputActionType.Interact:
					_playerInputActions?.Player.Interact.Disable();
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
			}
		}

		public void OnMove(CallbackContext context)
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

		public void OnJump(CallbackContext context)
		{
			switch (context.phase)
			{
				case InputActionPhase.Performed:
					JumpPerformedAction.Invoke();
					break;
				case InputActionPhase.Canceled:
					JumpCanceledAction.Invoke();
					break;
			}
		}

		public void OnRun(CallbackContext context)
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

		public void OnInventory(CallbackContext context)
		{
			if (context.performed)
				InventoryAction?.Invoke();
		}

		public void OnLockCursor(CallbackContext context)
		{

		}

		public void OnLook(CallbackContext context)
		{
			LookAction?.Invoke(context.ReadValue<Vector2>());
		}

		public void OnAttack(CallbackContext context)
		{

		}

		public void OnInteract(CallbackContext context)
		{
			if (context.performed)
				InteractPerformedAction?.Invoke();
		}
	}
}