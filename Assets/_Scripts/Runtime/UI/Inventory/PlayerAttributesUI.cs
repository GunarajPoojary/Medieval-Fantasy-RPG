using System;
using RPG.Events.EventChannel;
using RPG.StatSystem;
using TMPro;
using UnityEngine;

namespace RPG.UI.Inventory
{
    public class PlayerAttributesUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _hPText;
        [SerializeField] private TMP_Text _aTKText;
        [SerializeField] private TMP_Text _dEFText;

        public void OnUpdateBaseStats(StatType type, Stat stat)
        {
            switch (type)
            {
                case StatType.HP:
                    _hPText.text = $"+{stat.Value}";
                    break;
                case StatType.ATK:
                    _aTKText.text = $"+{stat.Value}";
                    break;
                case StatType.DEF:
                    _dEFText.text = $"+{stat.Value}";
                    break;
            }
        }
    }
}