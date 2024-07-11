using UnityEngine;

namespace RPG.Gameplay.Characters
{
    [CreateAssetMenu(fileName = "New Character Profile", menuName = "Character Profile", order = 0)]
    public class CharacterProfile_SO : ScriptableObject
    {
        public string Name;

        [Multiline(5)]
        public string Description;

        public Sprite Image;

        public Stats.BaseStats Stats;
    }
}
