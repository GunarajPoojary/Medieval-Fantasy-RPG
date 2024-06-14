using GunarajCode.Stat;
using UnityEngine;
using UnityEngine.UI;

namespace GunarajCode
{
    public class CharacterStatsUI : MonoBehaviour
    {
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private Slider _defenseSlider;
        [SerializeField] private Image _characterImage;
        [SerializeField] private Character _character;

        private void Awake()
        {
            setCharacterImage();
            MaxDefense(_character.BaseStats.GetStatValue(StatType.Defense));
            MaxHealth(_character.BaseStats.GetStatValue(StatType.Health));
        }

        private void MaxHealth(int health)
        {
            _healthSlider.maxValue = health;
            _healthSlider.value = health;
        }

        private void MaxDefense(int defense)
        {
            _defenseSlider.maxValue = defense;
            _defenseSlider.value = defense;
        }

        private void Update()
        {
            SetHalth(_character.CurrentHealth);
            SetDefense(_character.CurrentDefense);
        }

        private void SetHalth(float health)
        {
            _healthSlider.value = health;
        }

        private void SetDefense(float defense)
        {
            _defenseSlider.value = defense;
        }

        private void setCharacterImage()
        {
            _characterImage.sprite = _character.Profile.Image;
        }
    }
}
