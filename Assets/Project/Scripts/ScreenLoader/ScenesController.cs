using UnityEngine.SceneManagement;

namespace IdleTycoon
{
    public static class ScenesController
    {
        public static string LastActiveScene { get; set; }
        public static LoadingScreen LoadingScreen { get; set; }

        public static string GetActiveSceneName()
        {
            return SceneManager.GetActiveScene().name;
        }

        public static bool IsActiveScene(string value)
        {
            return GetActiveSceneName() == value;
        }

        public static void LoadScene(string value)
        {
            LastActiveScene = GetActiveSceneName();
            SceneManager.LoadScene(value);
        }

        public static void LoadSceneAsync(string value)
        {
            LoadingScreen.Load(value);
        }
    }
}