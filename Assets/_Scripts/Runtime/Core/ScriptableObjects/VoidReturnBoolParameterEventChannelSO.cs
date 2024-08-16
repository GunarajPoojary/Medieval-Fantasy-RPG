using UnityEngine;

namespace RPG.Core
{
    /// <summary>
    /// Event Channel that broadcasts an event carrying a single boolean parameter.
    /// </summary>
    [CreateAssetMenu(menuName = "Events/Void Return Type/Single Parameter/Void Return Bool Parameter Event Channel")]
    public class VoidReturnBoolParameterEventChannelSO : GenericVoidReturnSingleParameterEventChannelSO<bool> { }
}
