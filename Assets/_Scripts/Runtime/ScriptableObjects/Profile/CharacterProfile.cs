using RPG.ScriptableObjects.Stats;
using UnityEngine;

namespace RPG.ScriptableObjects.Profile
{
    [CreateAssetMenu(fileName = "New Character Profile", menuName = "Characters/Profile", order = 0)]
    public class CharacterProfile : ScriptableObject
    {
        public string CharacterName;
        public Sprite Image;
        public BaseStats Stats;
        public RuntimeStats RuntimeStats;

        [Multiline(5)]
        public string Description;
    }
}