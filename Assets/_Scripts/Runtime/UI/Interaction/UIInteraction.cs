using UnityEngine;

namespace ProjectEmbersteel
{
    public class UIInteraction : MonoBehaviour
    {
        [SerializeField] private GameObject _interactionPanel;

        public void ToggleUI(bool toggle) => _interactionPanel.SetActive(toggle);
    }
}