using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RPG
{
    /// <summary>
    /// Represents a button in a tab group, handling user interactions and sound effects.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class TabButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private AudioClip _clickSound;

        [SerializeField] private AudioClip _enterSound;

        [SerializeField] private AudioSource _audioSource;

        private IButtonSoundPlayer _buttonSoundPlayer;

        // The tab group to which this button belongs.
        [field: SerializeField] public TabGroup TabGroup { get; set; }

        [field: SerializeField] public Image Background { get; private set; }

        private void Awake() => _buttonSoundPlayer = new ButtonSoundPlayer(_audioSource, _clickSound, _enterSound);

        #region IPointerClickHandler Method
        public void OnPointerClick(PointerEventData eventData)
        {
            TabGroup.OnTabClick(this);
            _buttonSoundPlayer.OnPointerClick(eventData);
        }
        #endregion

        #region IPointerEnterHandler Method
        public void OnPointerEnter(PointerEventData eventData)
        {
            TabGroup.OnTabEnter(this);
            _buttonSoundPlayer.OnPointerEnter(eventData);
        }
        #endregion

        #region IPointerExitHandler Method
        public void OnPointerExit(PointerEventData eventData) => TabGroup.OnTabExit(this);
        #endregion
    }
}