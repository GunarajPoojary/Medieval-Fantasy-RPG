using RPG.Gameplay.Inventories;

namespace RPG.Gameplay.EquipmentSystem
{
    /// <summary>
    /// Class for displaying equipped armor details in the UI.
    /// </summary>
    public class EquippedArmorDetail : EquippedEquipmentDetail
    {
        private ArmorSO[] equippedArmors;

        private void Awake()
        {
            EquipmentManager.Instance.OnArmorChanged += OnArmorChanged;
            equippedArmors = EquipmentManager.Instance.EquippedArmorsSO;
            ShowFirstEquippedArmor();
        }

        private void ShowFirstEquippedArmor()
        {
            foreach (var armor in equippedArmors)
            {
                if (armor != null && !armor.IsDefault)
                {
                    UpdateDetailUI(armor);
                    return;
                }
            }

            UpdateDetailUI(null);
        }

        private void OnArmorChanged(ArmorSO newArmor, ArmorSO oldArmor) => UpdateDetailUI(newArmor);

        private void OnDestroy() => EquipmentManager.Instance.OnArmorChanged -= OnArmorChanged;

        protected override string GetTypeText(EquipmentSO equipment)
        {
            var armor = (ArmorSO)equipment;
            return armor.ArmorType.ToString();
        }
    }
}
