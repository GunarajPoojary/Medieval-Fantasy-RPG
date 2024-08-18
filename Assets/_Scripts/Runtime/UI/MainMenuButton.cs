using RPG.Core;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class MainMenuButton : MonoBehaviour
    {
        [SerializeField] private VoidReturnIntParameterEventChannelSO _loadSceneChannelSO;
        [Space]
        [SerializeField] private VoidReturnNonParameterEventChannelSO _continueGameChannelSO;

        private Button _button;

        private void Awake() => _button = GetComponent<Button>();

        private void Start()
        {
            _button.onClick.AddListener(() => _continueGameChannelSO.RaiseEvent());
            _button.onClick.AddListener(() => _loadSceneChannelSO.RaiseEvent(0));
        }
    }
}