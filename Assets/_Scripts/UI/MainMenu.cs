using UnityEngine;
using UnityEngine.UI;

namespace GunarajCode
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Button _newGameButton;
        [SerializeField] private Button _continueGameButton;

        private void Start()
        {
            if (!DataPersistenceManager.Instance.HasGameData())
            {
                _continueGameButton.interactable = false;
            }
        }

        public void OnNewGameSelected()
        {
            DisableMenuButton();

            DataPersistenceManager.Instance.NewGame();

            SceneManagement.Instance.LoadGameplay();
        }

        public void OnContinueSelected()
        {
            DisableMenuButton();

            DataPersistenceManager.Instance.SaveGame();

            SceneManagement.Instance.LoadGameplay();
        }

        private void DisableMenuButton()
        {
            _newGameButton.interactable = false;
            _continueGameButton.interactable = false;
        }
    }
}
