using UnityEngine;

namespace GunarajCode.Gameplay
{
    public class OpenCrate : MonoBehaviour, IInteractable
    {
        [SerializeField] private Transform _upperLid;

        public Transform InteractableTransform => transform;

        public void Interact()
        {
            _upperLid.rotation = Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(-90, 0, 0), 0.5f);
            Destroy(gameObject, 3);
        }
    }
}
