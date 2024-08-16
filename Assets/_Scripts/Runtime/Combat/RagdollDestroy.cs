using System.Collections;
using UnityEngine;

namespace RPG.Combat
{
    public class RagdollDestroy : MonoBehaviour
    {
        [SerializeField] private float _waitTime;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(_waitTime);
            Destroy(gameObject);
        }
    }
}