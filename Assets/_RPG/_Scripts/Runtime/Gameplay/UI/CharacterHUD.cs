using RPG.Gameplay.EquipmentSystem;
using RPG.Gameplay.Stats;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Gameplay.UI
{
    public class CharacterHUD : MonoBehaviour
    {
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private Slider _defenseSlider;
        [SerializeField] private Image _characterImage;
        [SerializeField] private PlayerStatsHandler _character;

        private void Start()
        {
            SetCharacterImage();
            SetMaxHealth();
            SetMaxDefense();
        }

        private int GetTotalStatValue(StatType statType)
        {
            int totalValue = 0;

            if (EquipmentManager.Instance.EquippedWeaponSO != null && EquipmentManager.Instance.EquippedWeaponSO.EquipmentStats != null)
                totalValue += EquipmentManager.Instance.EquippedWeaponSO.EquipmentStats.GetStatValue(statType);

            foreach (var armor in EquipmentManager.Instance.EquippedArmorsSO)
            {
                if (armor != null && armor.EquipmentStats != null)
                    totalValue += armor.EquipmentStats.GetStatValue(statType);
            }

            return totalValue;
        }


        private void SetMaxHealth()
        {
            int health = _character.Profile.Stats.GetStatValue(StatType.Health) + GetTotalStatValue(StatType.Health);
            _healthSlider.maxValue = health;
            _healthSlider.value = health;
        }

        private void SetMaxDefense()
        {
            int defense = _character.Profile.Stats.GetStatValue(StatType.Defense) + GetTotalStatValue(StatType.Defense);
            _defenseSlider.maxValue = defense;
            _defenseSlider.value = defense;
        }

        private void Update()
        {
            _healthSlider.value = _character.RuntimeStats.GetStatValue(StatType.Health);
            _defenseSlider.value = _character.RuntimeStats.GetStatValue(StatType.Defense);
        }

        private void SetCharacterImage() => _characterImage.sprite = _character.Profile.Image;
    }
}
