using UnityEngine;
using UnityEngine.UI;

namespace RPG.Core.UI
{
    public class MainMenuButton : MonoBehaviour
    {
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(ViewManager.Instance.LoadMainMenu);
        }
    }
}
