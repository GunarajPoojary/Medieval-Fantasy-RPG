using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using RPG.Player.InputActions;

namespace RPG.Utilities.Inputs.ScriptableObjects
{
	public enum InputActionType
	{
		Move,
		Jump,
		Run
	}

	[CreateAssetMenu(fileName = "InputReader", menuName = "Game/Player/Input/Input Reader")]
	public class InputReader : DescriptionBaseSO, PlayerInputActions.IPlayerActions
	{
		// Assign delegate{} to events to initialise them with an empty delegate
		// so we can skip the null check when we use them

		private PlayerInputActions _playerInputActions;

		public Vector2 MoveDirection => _playerInputActions?.Player.Move.ReadValue<Vector2>() ?? Vector2.zero;

		// Gameplay
		public UnityAction<Vector2> MoveStartedAction = delegate { };
		public UnityAction<Vector2> MovePerformedAction = delegate { };
		public UnityAction<Vector2> MoveCanceledAction = delegate { };
		public UnityAction JumpStartedAction = delegate { };
		public UnityAction JumpPerformedAction = delegate { };
		public UnityAction JumpCanceledAction = delegate { };
		public UnityAction<bool> RunStartedAction = delegate { };
		public UnityAction<bool> RunPerformedAction = delegate { };
		public UnityAction<bool> RunCanceledAction = delegate { };
		public UnityAction InventoryAction = delegate { };

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
			}
		}

		public void OnMove(InputAction.CallbackContext context)
		{
			switch (context.phase)
			{
				case InputActionPhase.Started:
					MoveStartedAction.Invoke(context.ReadValue<Vector2>());
					break;
				case InputActionPhase.Performed:
					MovePerformedAction.Invoke(context.ReadValue<Vector2>());
					break;
				case InputActionPhase.Canceled:
					MoveCanceledAction.Invoke(Vector2.zero);
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
				case InputActionPhase.Started:
					RunStartedAction.Invoke(true);
					break;
				case InputActionPhase.Performed:
					RunPerformedAction.Invoke(true);
					break;
				case InputActionPhase.Canceled:
					RunCanceledAction.Invoke(false);
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

		public void OnCharacterMenu(InputAction.CallbackContext context)
		{

		}

		public void OnLook(InputAction.CallbackContext context)
		{

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