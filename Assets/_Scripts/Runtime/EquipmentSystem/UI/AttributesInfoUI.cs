using RPG.ScriptableObjects.Profile;
using TMPro;
using UnityEngine;

namespace RPG.EquipmentSystem.UI
{
    public class AttributesInfoUI : MonoBehaviour
    {
        [SerializeField] private CharacterProfile _profile;

        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _statsText;
        private void Start()
        {
            _nameText.text = _profile.CharacterName;
            _statsText.text = _profile.RuntimeStats.ToString();
        }
    }
}