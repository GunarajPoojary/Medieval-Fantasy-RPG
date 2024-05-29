using GunarajCode.ScriptableObjects;

namespace GunarajCode.Stat
{
    /// <summary>
    /// Represents player-specific statistics that are affected by equipped items.
    /// </summary>
    public class PlayerStats : CharacterStat
    {
        private EquipmentManager _equipmentManager;

        private void Awake()
        {
            _equipmentManager = EquipmentManager.Instance;
        }

        private void Start() => EquipmentManager.Instance.OnEquipmentChanged += OnEquipmentChanged;

        private void OnDestroy()
        {
            if (_equipmentManager != null)
                _equipmentManager.OnEquipmentChanged -= OnEquipmentChanged;
        }

        private void OnEquipmentChanged(EquipmentObject newItem, EquipmentObject oldItem)
        {
            // Check if the new item is a defensive armor and not default
            if (newItem is ArmorObject newArmorItem && !newArmorItem.IsDefault)
            {
                foreach (var stat in newArmorItem.EquipmentStats.StatTypeToValueMap)
                {
                    // If the character already has this stat, update its value
                    if (_characterStats.StatTypeToValueMap.ContainsKey(stat.Key))
                        _characterStats.ChangeStatValue(stat.Key, newArmorItem.EquipmentStats.GetStatValue(stat.Key));
                }
            }

            if (oldItem is ArmorObject oldDefensiveItem && !oldDefensiveItem.IsDefault)
            {
                foreach (var stat in oldDefensiveItem.EquipmentStats.StatTypeToValueMap)
                {
                    if (_characterStats.StatTypeToValueMap.ContainsKey(stat.Key))
                        _characterStats.ChangeStatValue(stat.Key, -oldDefensiveItem.EquipmentStats.GetStatValue(stat.Key));
                }
            }
        }
    }
}
