using UnityEngine;
using UnityEngine.SceneManagement;

namespace GunarajCode
{
    public class SceneManagement : Singleton<SceneManagement>
    {
        public void ReloadScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        public void LoadMainMenu() => SceneManager.LoadScene("MainMenu");

        public void LoadGameplay() => SceneManager.LoadScene("Gameplay");
    }
}
