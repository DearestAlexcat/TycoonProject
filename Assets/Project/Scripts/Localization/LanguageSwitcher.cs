using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace IdleTycoon
{
    public class LanguageSwitcher : MonoBehaviour
    {
        [SerializeField] Image flagView;
        [SerializeField] LocaleSelector localeSelector;
        [SerializeField] List<Sprite> flags;

        public int CurrentId { get; private set; }

        public Action OnUpdateStorageData;

        //void Start()
        //{
        //    SetFlagView(0);
        //}

        public void SetFlagView(int id)
        {
            if (localeSelector.IsLocalizationInitialized)
            {
                localeSelector.SetLocale(id);

                CurrentId = id;
                flagView.sprite = flags[id];

                OnUpdateStorageData?.Invoke();
            }
            else
            {
                Debug.LogError("Failed to change localization.");
            }
        }

        public void ChangeFlag(int step) // Button Event
        {
            Service<AudioManager>.Get().PlaySound("Click");

            int id = CurrentId + step;

            if (id == flags.Count) id = 0;
            if (id < 0f) id = flags.Count - 1;

            SetFlagView(id);
        }
    }
}