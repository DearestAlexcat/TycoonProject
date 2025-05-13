using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace IdleTycoon
{
    public class LoadingScreen : MonoBehaviour
    {
        AsyncOperation AsyncLoading { get; set; }

        [field: SerializeField] float DefaultAnimationTime { get; set; }
        [field: SerializeField] float CurrentAnimationTime { get; set; }

        [field: SerializeField] GameObject LoadingObject { get; set; }
        [field: SerializeField] Animator Animator { get; set; }

        [field: SerializeField] bool Loading { get; set; }

        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;

            ScenesController.LoadingScreen = this;

            LoadingObject.SetActive(false);

            DontDestroyOnLoad(gameObject);
        }

        private IEnumerator LoadingLoop()
        {
            while (Loading)
            {
                if (CurrentAnimationTime > 0f)
                {
                    CurrentAnimationTime -= Time.deltaTime / DefaultAnimationTime;
                }
                else if (AsyncLoading.progress >= 0.9f)
                {
                    Activate();
                }

                yield return null;
            }
        }

        public bool IsActive()
        {
            return LoadingObject.activeInHierarchy;
        }

        public void Load(string value = null)
        {
            if (!Loading)
            {
                Time.timeScale = 1f;

                Loading = true;

                CurrentAnimationTime = DefaultAnimationTime;

                LoadingObject.SetActive(true);

                Animator.SetTrigger("Show");

                AsyncLoading = SceneManager.LoadSceneAsync(value, LoadSceneMode.Single);
                AsyncLoading.allowSceneActivation = false;

                ScenesController.LastActiveScene = ScenesController.GetActiveSceneName();

                StartCoroutine(LoadingLoop());
            }
        }

        private void Activate()
        {
            Loading = false;

            Animator.SetTrigger("Hide");

            AsyncLoading.allowSceneActivation = true;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Resources.UnloadUnusedAssets();
        }
    }
}