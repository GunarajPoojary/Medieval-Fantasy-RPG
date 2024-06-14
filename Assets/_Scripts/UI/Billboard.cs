using UnityEngine;

namespace GunarajCode
{
    public class Billboard : MonoBehaviour
    {
        [SerializeField] private Transform _cam;

        private void LateUpdate() => transform.LookAt(transform.position + _cam.forward);
    }
}
