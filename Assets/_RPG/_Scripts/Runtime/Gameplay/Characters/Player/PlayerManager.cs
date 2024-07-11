using RPG.Core;
using UnityEngine;

namespace RPG.Gameplay.Player
{
    public class PlayerManager : GenericSingleton<PlayerManager>
    {
        public GameObject Player { get; private set; }

        private GameObject gameplayPlayerCanvas;
    }
}
