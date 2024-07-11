using TMPro;
using UnityEngine;

namespace RPG.Gameplay.EquipmentSystem
{
    public class AttributesInfoUI : MonoBehaviour
    {
        private Characters.CharacterProfile_SO _profile;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _statsText;

        private void Awake()
        {
            _profile = FindObjectOfType<Stats.CharacterStatsManager>().Profile;
            _nameText.text = _profile.Name;
            _statsText.text = _profile.Stats.ToString();
        }
    }
}
