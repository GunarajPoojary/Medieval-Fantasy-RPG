using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG
{
    [System.Serializable]
    public class TabData
    {
        public TabButton tabButton;
        public GameObject tabContent;
    }

    public class TabGroup : MonoBehaviour
    {
        [SerializeField] private TabData[] _tabs;

        [SerializeField] private AudioClip _hoverSound;
        [SerializeField] private AudioClip _selectSound;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private Color _defaultColor;
        [SerializeField] private Color _hoverColor;
        [SerializeField] private Color _selectedColor;
        private int _selectedButtonIndex = 0;
        private Image _selectedImage;
        public event Action<int> OnTabChanged;

        private void OnEnable()
        {
            for (int i = 0; i < _tabs.Length; i++)
            {
                if (_tabs[i].tabButton != null)
                    _tabs[i].tabButton.Setup(this, i);
            }
        }

        public void SetDefaultTab(int index) => OnTabSelected(index, _tabs[index].tabButton.GetComponent<Image>());

        public void OnHoverEnter(Image image)
        {
            _audioSource.PlayOneShot(_hoverSound);

            if (image != _selectedImage)
                image.color = _hoverColor;
        }

        public void OnHoverExit(Image image)
        {
            if (image != _selectedImage)
                image.color = _defaultColor;
        }

        public void OnTabSelected(int index, Image image)
        {
            if (_selectedImage != null)
            {
                _selectedImage.color = _defaultColor;
                _selectedImage = null;
            }

            _selectedImage = image;
            _selectedImage.color = _selectedColor;
            _selectedButtonIndex = index;
            ShowTab();
        }

        private void ShowTab()
        {
            _audioSource.PlayOneShot(_selectSound);

            for (int i = 0; i < _tabs.Length; i++)
                _tabs[i].tabContent.SetActive(i == _selectedButtonIndex);

            OnTabChanged?.Invoke(_selectedButtonIndex);
        }
    }
}