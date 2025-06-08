using RPG.Utilities.Inputs.ScriptableObjects;
using UnityEngine;

namespace RPG
{
    public class CharacterEquipmentController : MonoBehaviour
    {
        [SerializeField] private GameObject _menuContainer;
        [SerializeField] private GameObject _cam;
        [SerializeField] private GameObject[] _gameobjects;
        [SerializeField] private LayerMask _blurrLayer;
        [SerializeField] private InputReader _inputReader;

        private void Update()
        {
            // Toggle menu and start camera + blur transition
            if (Input.GetKeyDown(KeyCode.H))
            {
                bool isActive = _menuContainer.activeSelf;

                if (!isActive)
                {
                    int layerIndex = (int)Mathf.Log(_blurrLayer.value, 2); // Only works if 1 layer is set

                    foreach (var gameObject in _gameobjects)
                    {
                        gameObject.layer = layerIndex;
                    }

                    _cam.SetActive(true); // Activates the blending cam

                    _inputReader.DisablePlayerActions();
                }
                else
                {
                    foreach (var gameObject in _gameobjects)
                    {
                        gameObject.layer = 0;
                    }

                    _cam.SetActive(false); // Will trigger blend back to original cam

                    _inputReader.EnablePlayerActions();
                }

                _menuContainer.SetActive(!isActive);
            }
        }
    }
}