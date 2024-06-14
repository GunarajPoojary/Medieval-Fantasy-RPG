using UnityEngine;

namespace GunarajCode
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; set; }

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                if (Instance != this)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
