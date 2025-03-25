using UnityEngine;
using UnityEngine.UI;

namespace RPG
{
    public class CharacterHUD : MonoBehaviour
    {
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private Slider _defenseSlider;
        [SerializeField] private Image _characterImage;

        private ICharacterStatsProvider _characterHealthDefenseProvider;

        private void Awake() => _characterHealthDefenseProvider = GetComponentInParent<CharacterStatsHandler>();

        private void Start()
        {
            SetCharacterImage();
            SetMaxHealth();
            SetMaxDefense();
        }

        private void SetMaxHealth()
        {
            int health = (int)_characterHealthDefenseProvider.GetOverallHealth();
            _healthSlider.maxValue = health;
            _healthSlider.value = health;
        }

        private void SetMaxDefense()
        {
            int defense = (int)_characterHealthDefenseProvider.GetOverallDefense();
            _defenseSlider.maxValue = defense;
            _defenseSlider.value = defense;
        }

        private void Update()
        {
            _healthSlider.value = _characterHealthDefenseProvider.GetCurrentHealth();
            _defenseSlider.value = _characterHealthDefenseProvider.GetCurrentDefense();
        }

        private void SetCharacterImage()
        {
            //_characterImage.sprite = Profile?.Image;
        }
    }
}