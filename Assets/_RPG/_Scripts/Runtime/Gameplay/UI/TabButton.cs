using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RPG.Gameplay.UI
{
    /// <summary>
    /// Represents a button used within a tab group.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class TabButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public TabGroup TabGroup;
        [HideInInspector] public Image Background;
        [SerializeField] private AudioClip _clickSound;
        [SerializeField] private AudioClip _enterSound;
        [SerializeField] private AudioSource _audioSource;

        private void Awake() => Background = GetComponent<Image>();

        public void OnPointerClick(PointerEventData eventData)
        {
            TabGroup.OnTabClick(this);
            _audioSource.clip = _clickSound;
            _audioSource.Play();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            TabGroup.OnTabEnter(this);
            _audioSource.clip = _enterSound;
            _audioSource.Play();
        }

        public void OnPointerExit(PointerEventData eventData) => TabGroup.OnTabExit(this);
    }
}
