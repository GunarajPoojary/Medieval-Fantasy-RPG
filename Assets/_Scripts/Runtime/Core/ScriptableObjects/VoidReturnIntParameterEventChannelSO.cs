using UnityEngine;

namespace RPG.Core
{
    /// <summary>
    /// General Event Channel that broadcasts and carries Int payload.
    /// </summary>
    [CreateAssetMenu(menuName = "Events/Void Return Type/Single Parameter/Void Return Int Parameter Event Channel")]
    public class VoidReturnIntParameterEventChannelSO : GenericVoidReturnSingleParameterEventChannelSO<int> { }
}
