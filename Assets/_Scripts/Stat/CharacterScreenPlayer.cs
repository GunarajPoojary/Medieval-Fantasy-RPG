using UnityEngine;

namespace GunarajCode
{
    public class CharacterScreenPlayer : MonoBehaviour
    {
        public CharacterProfile _profile;
        [SerializeField] private Animator _animator;
        [SerializeField] private Stats _baseStats;
    }
}
