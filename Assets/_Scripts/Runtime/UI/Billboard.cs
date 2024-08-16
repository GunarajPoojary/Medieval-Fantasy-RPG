using UnityEngine;

namespace RPG.UI
{
    /// <summary>
    /// Responsible for making enemy HealthBar UI face the user camera
    /// </summary>
    public class Billboard : MonoBehaviour
    {
        [SerializeField] private Transform _cam;

        private void LateUpdate() => transform.LookAt(transform.position + _cam.forward);
    }
}