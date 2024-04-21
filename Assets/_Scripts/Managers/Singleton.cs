using UnityEngine;

namespace GunarajCode
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance;

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
