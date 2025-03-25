using UnityEngine;

namespace RPG
{
    [CreateAssetMenu(fileName = "New Character Profile", menuName = "Characters/Profile", order = 0)]
    public class Character : ScriptableObject
    {
        public string CharacterName;

        [Multiline(5)]
        public string Description;

        public Sprite Image;
        public BaseStats Stats;
        public RuntimeStats RuntimeStats;
    }
}