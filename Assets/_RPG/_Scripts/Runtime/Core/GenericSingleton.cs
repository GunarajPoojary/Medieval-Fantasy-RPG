using UnityEngine;

namespace RPG.Core
{
    public class GenericSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static readonly object _lock = new object();
        private static T _instance;

        public static T Instance
        {
            get
            {
                lock (_lock)
                {
                    return _instance;
                }
            }
            private set
            {
                lock (_lock)
                {
                    _instance = value;
                }
            }
        }

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
