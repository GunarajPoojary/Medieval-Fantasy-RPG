using ProjectEmbersteel.Events.EventChannel;
using ProjectEmbersteel.StatSystem;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectEmbersteel.UI
{
    public class StatusBarHUD : MonoBehaviour
    {
        [SerializeField] private RuntimeStatUpdateEventChannel _runtimeStatUpdateEventChannel;
        [SerializeField] private Image _hPFill;
        [SerializeField] private Image _dEFFill;

        private void OnEnable() => SubscribeTOEvents(true);
        private void OnDisable() => SubscribeTOEvents(false);

        private void SubscribeTOEvents(bool subscribe)
        {
            if (subscribe)
                _runtimeStatUpdateEventChannel.OnEventRaised += UpdateUI;
            else
                _runtimeStatUpdateEventChannel.OnEventRaised -= UpdateUI;
        }

        private void UpdateUI(StatType statType, float currentValue, float maxValue)
        {
            if (statType == StatType.HP)
            {
                _hPFill.fillAmount = currentValue / maxValue;
            }

            if (statType == StatType.DEF)
            {
                _dEFFill.fillAmount = currentValue / maxValue;
            }
        }
    }
}