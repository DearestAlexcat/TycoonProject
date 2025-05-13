using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace IdleTycoon
{
    public class LocaleSelector : MonoBehaviour
    {
        public bool IsLocalizationInitialized { get; private set; } = true;

        public void SetLocale(int id)
        {
            //if (!IsLocalizationInitialized) return;
            StartCoroutine(InitLocalization(id));
        }

        IEnumerator InitLocalization(int id)
        {
            IsLocalizationInitialized = false;

            AsyncOperationHandle initOperation = LocalizationSettings.InitializationOperation;
            yield return initOperation;

            if (initOperation.Status == AsyncOperationStatus.Succeeded)
            {
                IsLocalizationInitialized = true;
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[id];
            }
            else
            {
                Debug.LogError("Localization failed to initialize.");
            }
        }
    }
}
