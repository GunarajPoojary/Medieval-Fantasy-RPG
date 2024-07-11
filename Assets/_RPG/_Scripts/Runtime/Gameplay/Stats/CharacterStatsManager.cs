using RPG.Gameplay.EquipmentSystem;
using RPG.Gameplay.Inventories;
using RPG.SaveLoad;
using UnityEngine;

namespace RPG.Gameplay.Stats
{
    public class CharacterStatsManager : MonoBehaviour, ISaveable
    {
        [field: SerializeField] public Characters.CharacterProfile_SO Profile { get; private set; }
        public StatModifier RuntimeStats { get; private set; }

        private void Awake() => RuntimeStats = new(Profile.Stats);

        private void OnEnable()
        {
            var equipmentManager = EquipmentManager.Instance;
            equipmentManager.OnWeaponChanged += OnWeaponChanged;
            equipmentManager.OnArmorChanged += OnArmorChanged;
        }

        private void OnDisable()
        {
            var equipmentManager = EquipmentManager.Instance;
            equipmentManager.OnWeaponChanged -= OnWeaponChanged;
            equipmentManager.OnArmorChanged -= OnArmorChanged;
        }

        private void OnWeaponChanged(WeaponSO newWeapon, WeaponSO oldWeapon)
        {
            ChangeEquipmentStats(oldWeapon, -1);
            ChangeEquipmentStats(newWeapon, 1);
        }

        private void OnArmorChanged(ArmorSO newArmor, ArmorSO oldArmor)
        {
            ChangeEquipmentStats(oldArmor, -1);
            ChangeEquipmentStats(newArmor, 1);
        }

        private void ChangeEquipmentStats(EquipmentSO equipment, int operation)
        {
            if (equipment == null || (equipment is ArmorSO armor && armor.IsDefault)) return;

            foreach (var stat in equipment.EquipmentStats.BaseStatTypeToValueMap)
            {
                if (RuntimeStats.RuntimeStatTypeToValueMap.TryGetValue(stat.Key, out int value))
                    RuntimeStats.ChangeStatValue(stat.Key, stat.Value * operation);
            }
        }

        public void LoadData(GameData data) => RuntimeStats.RuntimeStatTypeToValueMap = data.PlayerStats;

        public void SaveData(GameData data) => data.PlayerStats = RuntimeStats.RuntimeStatTypeToValueMap;
    }
}
