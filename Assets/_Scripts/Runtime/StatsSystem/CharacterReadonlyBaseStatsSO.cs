using UnityEngine;

namespace ProjectEmbersteel.StatSystem
{
    [CreateAssetMenu(fileName = "NewCharacterReadonlyBaseStats", menuName = "Custom/Stats/ReadonlyBaseStats/CharacterReadonlyBaseStats")]
    public class CharacterReadonlyBaseStatsSO : DescriptionBaseSO
    {
        [field: SerializeField] public ReadonlyBaseStat[] ReadonlyBaseStats { get; private set; }
    }
}