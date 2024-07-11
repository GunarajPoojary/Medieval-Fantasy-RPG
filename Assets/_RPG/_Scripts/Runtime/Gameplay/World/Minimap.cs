using UnityEngine;

namespace RPG.Gameplay.World
{
    public class Minimap : MonoBehaviour
    {
        [SerializeField] private Transform _player;

        private void LateUpdate()
        {
            Vector3 newPosition = _player.position;
            newPosition.y = transform.position.y;
            transform.position = newPosition;

            transform.rotation = Quaternion.Euler(90f, _player.eulerAngles.y, 0f);
        }
    }
}
