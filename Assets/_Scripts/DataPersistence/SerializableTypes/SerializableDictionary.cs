using System.Collections.Generic;
using UnityEngine;

namespace GunarajCode
{
    [System.Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<TKey> _keys = new List<TKey>();
        [SerializeField] private List<TValue> _values = new List<TValue>();

        public void OnAfterDeserialize()
        {
            this.Clear();

            if (_keys.Count != _values.Count)
            {
                Debug.LogError("Mismatched key and value counts in SerializableDictionary.");
            }

            for (int i = 0; i < _keys.Count; i++)
            {
                this.Add(_keys[i], _values[i]);
            }
        }

        public void OnBeforeSerialize()
        {
            _keys.Clear();
            _values.Clear();

            foreach (KeyValuePair<TKey, TValue> kvp in this)
            {
                _keys.Add(kvp.Key);
                _values.Add(kvp.Value);
            }
        }
    }
}
