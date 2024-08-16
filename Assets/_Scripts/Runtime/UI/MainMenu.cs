using RPG.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RPG.UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private Button _newGameButton;

        [SerializeField]
        private Button _continueGameButton;

        [SerializeField]
        private Button _settingsButton;

        [SerializeField]
        private Button _quitButton;

        [SerializeField]
        private AudioSource _audioSource;

        [SerializeField]
        private AudioClip _hoverSound;

        [SerializeField]
        private AudioClip _selectSound;

        // Event channel for checking if there is saved data.
        [SerializeField]
        private VoidReturnBoolParameterEventChannelSO _hasDataEventChannelSO;

        // Event channel for starting a new game.
        [SerializeField]
        private VoidReturnNonParameterEventChannelSO _newGameChannelSO;

        // Event channel for continuing a game.
        [SerializeField]
        private VoidReturnNonParameterEventChannelSO _continueGameChannelSO;

        // Event channel for loading a scene.
        [SerializeField]
        private VoidReturnIntParameterEventChannelSO _loadSceneChannelSO;

        private IButtonSoundPlayer _buttonSoundPlayer;

        private void Awake()
        {
            _buttonSoundPlayer = new ButtonSoundPlayer(_audioSource, _hoverSound, _selectSound);

            AddEventTrigger(_newGameButton, EventTriggerType.PointerEnter, _buttonSoundPlayer.OnPointerEnter);
            AddEventTrigger(_continueGameButton, EventTriggerType.PointerEnter, _buttonSoundPlayer.OnPointerEnter);
            AddEventTrigger(_newGameButton, EventTriggerType.PointerClick, _buttonSoundPlayer.OnPointerClick);
            AddEventTrigger(_continueGameButton, EventTriggerType.PointerClick, _buttonSoundPlayer.OnPointerClick);
            AddEventTrigger(_settingsButton, EventTriggerType.PointerEnter, _buttonSoundPlayer.OnPointerEnter);
            AddEventTrigger(_quitButton, EventTriggerType.PointerEnter, _buttonSoundPlayer.OnPointerEnter);
            AddEventTrigger(_settingsButton, EventTriggerType.PointerClick, _buttonSoundPlayer.OnPointerClick);
            AddEventTrigger(_quitButton, EventTriggerType.PointerClick, _buttonSoundPlayer.OnPointerClick);
        }

        private void OnEnable() => _hasDataEventChannelSO.OnEventRaised += HandleDataLoad;

        private void OnDisable() => _hasDataEventChannelSO.OnEventRaised -= HandleDataLoad;

        public void OnNewGameSelected()
        {
            DisableMenuButton();

            _newGameChannelSO.RaiseEvent();

            _loadSceneChannelSO.RaiseEvent(1);
        }

        public void OnContinueSelected()
        {
            DisableMenuButton();

            _continueGameChannelSO.RaiseEvent();

            _loadSceneChannelSO.RaiseEvent(1);
        }

        public void OnSettingsSelected()
        {
            //
        }

        public void OnQuitSelected() => Application.Quit();

        private void HandleDataLoad(bool hasData) => _continueGameButton.interactable = hasData;

        private void DisableMenuButton()
        {
            _newGameButton.interactable = false;
            _continueGameButton.interactable = false;
            _settingsButton.interactable = false;
            _quitButton.interactable = false;
        }

        private void AddEventTrigger(Button button, EventTriggerType eventType, UnityAction<BaseEventData> action)
        {
            EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry { eventID = eventType };
            entry.callback.AddListener(action);
            trigger.triggers.Add(entry);
        }
    }
}