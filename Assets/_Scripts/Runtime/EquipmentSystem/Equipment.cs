using ProjectEmbersteel.StatSystem;
using UnityEngine;

namespace ProjectEmbersteel.EquipmentSystem
{
    /// <summary>
    /// Example equipment class that implements IStatModifierProvider.
    /// Demonstrates how to create items that provide stat bonuses.
    /// </summary>
    public class Equipment : MonoBehaviour, IEquippable
    {
        [SerializeField] private EquipmentStatsSO _baseStats;

        public bool IsEquipped { get; private set; }

        public void Equip(IStatModifiable statModifiable)
        {
            gameObject.SetActive(true);
            _baseStats.AddBonuses(statModifiable);
            IsEquipped = true;
        }

        public void Unequip(IStatModifiable statModifiable)
        {
            _baseStats.RemoveBonuses(statModifiable);
            gameObject.SetActive(false);
            IsEquipped = false;
        }
    }
}
