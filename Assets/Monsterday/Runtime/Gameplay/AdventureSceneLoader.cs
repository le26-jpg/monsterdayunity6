using UnityEngine;
using UnityEngine.SceneManagement;

namespace Monsterday.Gameplay
{
    public sealed class AdventureSceneLoader : MonoBehaviour
    {
        public string SceneName = "Exploration";

        public void LoadScene()
        {
            if (string.IsNullOrWhiteSpace(SceneName)) return;
            SceneManager.LoadScene(SceneName);
        }
    }
}
