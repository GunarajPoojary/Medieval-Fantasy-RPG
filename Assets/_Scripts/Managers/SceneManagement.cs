using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace GunarajCode
{
    public class SceneManagement : Singleton<SceneManagement>
    {
        public void ReloadScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        public void LoadMainMenu() => SceneManager.LoadScene(0);

        public void LoadGameplay() => SceneManager.LoadScene(1);
    }
}
