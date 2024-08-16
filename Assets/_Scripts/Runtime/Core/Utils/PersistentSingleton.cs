using UnityEngine;

namespace RPG.Core.Utils
{
    public class PersistentSingleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (_instance == null)
                    {
                        SetupInstance();
                    }
                }

                return _instance;
            }
        }

        protected virtual void Awake() => RemoveDuplicates();

        private static void SetupInstance()
        {
            _instance = (T)FindObjectOfType(typeof(T));

            if (_instance == null)
            {
                GameObject singleton = new GameObject
                {
                    name = typeof(T).Name
                };

                _instance = singleton.AddComponent<T>();

                DontDestroyOnLoad(singleton);
            }
        }

        private void RemoveDuplicates()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}