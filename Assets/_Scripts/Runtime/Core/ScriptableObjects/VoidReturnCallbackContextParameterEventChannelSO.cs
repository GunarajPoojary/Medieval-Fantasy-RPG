using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG.Core
{
    /// <summary>
    /// Event Channel that broadcasts an event carrying a single parameter of type InputAction.CallbackContext.
    /// </summary>
    [CreateAssetMenu(menuName = "Events/Void Return Type/Single Parameter/Void Return CallbackContext Parameter Event Channel")]
    public class VoidReturnCallbackContextParameterEventChannelSO : GenericVoidReturnSingleParameterEventChannelSO<InputAction.CallbackContext> { }
}