using UnityEngine;

namespace GunarajCode.Stat
{
    public class Enemy : Character
    {
        [SerializeField] private GameObject _ragdoll;

        public override void Die()
        {
            base.Die();
            Instantiate(_ragdoll, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
