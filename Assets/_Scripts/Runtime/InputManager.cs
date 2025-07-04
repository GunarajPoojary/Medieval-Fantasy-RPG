using ProjectEmbersteel.Utilities.Inputs.ScriptableObjects;
using UnityEngine;

namespace ProjectEmbersteel
{
    public class InputManager : MonoBehaviour
    {
        [CreateScriptableObject][SerializeField] private InputReader _inputReader;
    }
}