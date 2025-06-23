using UnityEngine;
using Unity.Cinemachine;

namespace RPG.Utilities.Inputs.ScriptableObjects
{
	public class LookInputDisabler : MonoBehaviour
	{
		[SerializeField] private InputReader _inputReader;
		[SerializeField] private CinemachineInputAxisController _axisController;

		private void OnEnable() => SubscribeToInputReader(true);

		private void OnDisable() => SubscribeToInputReader(false);

		private void SubscribeToInputReader(bool subscribe)
		{
			if (subscribe)
				_inputReader.ToggleLookInput += ToggleLookInput;
			else
				_inputReader.ToggleLookInput -= ToggleLookInput;
		}

		private void ToggleLookInput(bool toggle)
		{
			foreach (InputAxisControllerBase<CinemachineInputAxisController.Reader>.Controller controller in _axisController.Controllers)
				controller.Enabled = toggle;
		}
	}
}