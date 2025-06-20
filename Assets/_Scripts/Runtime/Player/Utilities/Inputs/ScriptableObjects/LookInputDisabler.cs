using UnityEngine;
using Unity.Cinemachine;

namespace RPG.Utilities.Inputs.ScriptableObjects
{
	public class LookInputDisabler : MonoBehaviour
	{
		[SerializeField] private InputReader _inputReader;
		[SerializeField] private CinemachineInputAxisController _axisController;

		private void OnEnable()
		{
			_inputReader.ToggleLookInput += ToggleLookInput;
		}

		private void OnDisable()
		{
			_inputReader.ToggleLookInput -= ToggleLookInput;
		}

		private void ToggleLookInput(bool toggle)
		{
			foreach (InputAxisControllerBase<CinemachineInputAxisController.Reader>.Controller controller in _axisController.Controllers)
				controller.Enabled = toggle;
		}
	}
}