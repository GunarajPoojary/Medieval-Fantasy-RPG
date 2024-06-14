using UnityEngine;

namespace GunarajCode
{
    [CreateAssetMenu(fileName = "New Character Profile", menuName = "Character Profile", order = 0)]
    public class CharacterProfile : ScriptableObject
    {
        public string Name;

        [TextArea(20, 20)]
        public string Description;

        public Sprite Image;
    }
}
