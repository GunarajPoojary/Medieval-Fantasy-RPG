using RPG.SaveLoad;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RPG.Core.UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Button _newGameButton;
        [SerializeField] private Button _continueGameButton;

        [SerializeField] private AudioSource _audioSource;

        [SerializeField] private AudioClip _hoverSound;
        [SerializeField] private AudioClip _selectSound;

        private void OnEnable() => DataPersistenceManager.Instance.OnDataLoaded += HandleDataLoaded;

        private void OnDisable() => DataPersistenceManager.Instance.OnDataLoaded -= HandleDataLoaded;

        public event Action OnContinueButtonPressed;
        public event Action OnNewGameButtonPressed;

        private void Awake()
        {
            AddEventTrigger(_newGameButton, EventTriggerType.PointerEnter, OnPointerEnter);
            AddEventTrigger(_continueGameButton, EventTriggerType.PointerEnter, OnPointerEnter);
            AddEventTrigger(_newGameButton, EventTriggerType.PointerClick, OnPointerClick);
            AddEventTrigger(_continueGameButton, EventTriggerType.PointerClick, OnPointerClick);
        }

        private void HandleDataLoaded()
        {
            if (!DataPersistenceManager.Instance.HasGameData())
                _continueGameButton.interactable = false;
            else
                _continueGameButton.interactable = true;
        }

        public void OnNewGameSelected()
        {
            DisableMenuButton();

            DataPersistenceManager.Instance.NewGame();
            StartCoroutine(ViewManager.Instance.LoadGameplay());
            OnNewGameButtonPressed?.Invoke();
        }

        public void OnContinueSelected()
        {
            DisableMenuButton();

            DataPersistenceManager.Instance.SaveGame();

            StartCoroutine(ViewManager.Instance.LoadGameplay());

            OnContinueButtonPressed?.Invoke();
        }

        public void LoadOptionsMenu()
        {
            //
        }

        public void Quit() => Application.Quit();

        private void DisableMenuButton()
        {
            _newGameButton.interactable = false;
            _continueGameButton.interactable = false;
        }
        private void OnPointerEnter(BaseEventData data)
        {
            _audioSource.clip = _hoverSound;
            _audioSource.Play();
        }

        private void OnPointerClick(BaseEventData data)
        {
            _audioSource.clip = _selectSound;
            _audioSource.Play();
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
