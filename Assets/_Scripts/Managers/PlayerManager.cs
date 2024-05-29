using UnityEngine;

namespace GunarajCode
{
    public class PlayerManager : Singleton<PlayerManager>
    {
        [SerializeField] private GameObject _player;
        public GameObject Player { get { return _player; } }
    }
}
