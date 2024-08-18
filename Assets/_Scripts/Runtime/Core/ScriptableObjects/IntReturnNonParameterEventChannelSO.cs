using UnityEngine;

namespace RPG.Core
{
    /// <summary>
    /// Event Channel that broadcasts an event with no parameters and returns int.
    /// </summary>
    [CreateAssetMenu(menuName = "Events/Non-Void Return Type/Non-Parameter/Int Return Non-Parameter Event Channel")]
    public class IntReturnNonParameterEventChannelSO : GenericNonVoidNonParameterEventChannelSO<int> { }
}
