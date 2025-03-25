using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RPG
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

        private IButtonSoundPlayer _buttonSoundPlayer;
        private IMenuButtonStateChanger _buttonStateManager;

        private void Awake()
        {
            _buttonSoundPlayer = new ButtonSoundPlayer(_audioSource, _hoverSound, _selectSound);
            _buttonStateManager = new MenuButtonStateChanger(_newGameButton, _continueGameButton, _settingsButton, _quitButton);

            AddEventTrigger(_newGameButton, EventTriggerType.PointerEnter, _buttonSoundPlayer.OnPointerEnter);
            AddEventTrigger(_newGameButton, EventTriggerType.PointerClick, _buttonSoundPlayer.OnPointerClick);
            
            AddEventTrigger(_continueGameButton, EventTriggerType.PointerEnter, _buttonSoundPlayer.OnPointerEnter);
            AddEventTrigger(_continueGameButton, EventTriggerType.PointerClick, _buttonSoundPlayer.OnPointerClick);
            
            AddEventTrigger(_settingsButton, EventTriggerType.PointerEnter, _buttonSoundPlayer.OnPointerEnter);
            AddEventTrigger(_settingsButton, EventTriggerType.PointerClick, _buttonSoundPlayer.OnPointerClick);
            
            AddEventTrigger(_quitButton, EventTriggerType.PointerEnter, _buttonSoundPlayer.OnPointerEnter);
            AddEventTrigger(_quitButton, EventTriggerType.PointerClick, _buttonSoundPlayer.OnPointerClick);
        }

        public void OnNewGameSelected()
        {
            _buttonStateManager.SetButtonsInteractable(false);
            //_newGameChannelSO.RaiseEvent();
            //_loadSceneChannelSO.RaiseEvent(1);
        }

        public void OnContinueSelected()
        {
            _buttonStateManager.SetButtonsInteractable(false);
            //_continueGameChannelSO.RaiseEvent();
            //_loadSceneChannelSO.RaiseEvent(1);
        }

        public void OnSettingsSelected()
        {
            // Implement settings logic here
        }

        public void OnQuitSelected() => Application.Quit();

        private void HandleDataLoad(bool hasData = false) => _continueGameButton.interactable = hasData;

        private void AddEventTrigger(Button button, EventTriggerType eventType, UnityAction<BaseEventData> action)
        {
            EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry entry = new EventTrigger.Entry { eventID = eventType };

            entry.callback.AddListener(action);

            trigger.triggers.Add(entry);
        }
    }
}
