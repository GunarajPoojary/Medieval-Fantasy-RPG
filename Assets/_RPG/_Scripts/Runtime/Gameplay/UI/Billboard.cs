using UnityEngine;

namespace RPG.Gameplay.UI
{
    public class Billboard : MonoBehaviour
    {
        [SerializeField] private Transform _cam;

        private void LateUpdate() => transform.LookAt(transform.position + _cam.forward);
    }
}
