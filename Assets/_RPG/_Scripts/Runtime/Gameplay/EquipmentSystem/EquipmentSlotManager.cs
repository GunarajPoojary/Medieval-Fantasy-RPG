using RPG.Gameplay.Inventories;
using System;
using UnityEngine;

namespace RPG.Gameplay.EquipmentSystem
{
    public class EquipmentSlotManager : MonoBehaviour
    {
        public static EquipmentSlotManager Instance;

        private WeaponSO _selectedWeaponObject;
        private ArmorSO _selectedArmorObject;

        public static event Action<WeaponSO> OnWeaponSlotSelected;
        public static event Action<ArmorSO> OnArmorSlotSelected;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        public static void ShowWeaponSelected(WeaponSO weapon) => OnWeaponSlotSelected?.Invoke(weapon);

        public static void ShowArmorSelected(ArmorSO armor) => OnArmorSlotSelected?.Invoke(armor);
    }
}
