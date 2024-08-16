using RPG.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RPG.UI
{
    /// <summary>
    /// Handles the main menu interactions, including button sound effects, 
    /// scene loading, and checking for saved game data.
    /// </summary>
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Button _newGameButton;
        [SerializeField] private Button _continueGameButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _quitButton;

        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _hoverSound;
        [SerializeField] private AudioClip _selectSound;
        [Space]
        [SerializeField] private VoidReturnBoolParameterEventChannelSO _hasDataEventChannelSO; // This event is raised with a boolean parameter indicating whether saved data is available.
        [Space]
        [SerializeField] private VoidReturnNonParameterEventChannelSO _newGameChannelSO; // This event is raised when the "New Game" button is selected, initiating the process to start a new game.
        [Space]
        [SerializeField] private VoidReturnNonParameterEventChannelSO _continueGameChannelSO; // This event is raised when the "Continue" button is selected, initiating the process to continue the game from saved data.
        [Space]
        [SerializeField] private VoidReturnIntParameterEventChannelSO _loadSceneChannelSO; // This event is raised with an integer parameter representing the scene index to load.

        private IButtonSoundPlayer _buttonSoundPlayer;
        private IMenuButtonStateManager _buttonStateManager;

        private void Awake()
        {
            _buttonSoundPlayer = new ButtonSoundPlayer(_audioSource, _hoverSound, _selectSound);
            _buttonStateManager = new MenuButtonStateManager(_newGameButton, _continueGameButton, _settingsButton, _quitButton);

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
            _buttonStateManager.SetButtonsInteractable(false);
            _newGameChannelSO.RaiseEvent();
            _loadSceneChannelSO.RaiseEvent(1);
        }

        public void OnContinueSelected()
        {
            _buttonStateManager.SetButtonsInteractable(false);
            _continueGameChannelSO.RaiseEvent();
            _loadSceneChannelSO.RaiseEvent(1);
        }

        public void OnSettingsSelected()
        {
            // Implement settings logic here
        }

        public void OnQuitSelected() => Application.Quit();

        private void HandleDataLoad(bool hasData) => _continueGameButton.interactable = hasData;

        private void AddEventTrigger(Button button, EventTriggerType eventType, UnityAction<BaseEventData> action)
        {
            EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry entry = new EventTrigger.Entry { eventID = eventType };

            entry.callback.AddListener(action);

            trigger.triggers.Add(entry);
        }
    }
}
