using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

namespace IdleTycoon
{
    public class UnlockRoomScreen : ScreenBase
    {
        [SerializeField] LocalizedString localizedNameAsset;

        [SerializeField] TMP_Text unlockText;
        //[SerializeField] TMP_Text buyButtonText;

        int unlockCost;
        Action unlockRoomCallback;

        void Awake()
        {
            Service<UnlockRoomScreen>.Set(this);
        }

        public void Initialize(string key, int value)
        {
            if (localizedNameAsset != null)
            {
                localizedNameAsset.StringChanged -= UpdateNameText;
            }

            localizedNameAsset = new LocalizedString
            {
                TableReference = "UnlockRoomData",
                TableEntryReference = key
            };

            localizedNameAsset.Arguments = new object[] { value };
            localizedNameAsset.StringChanged += UpdateNameText;
        }

        private void OnDisable()
        {
            if (localizedNameAsset != null)
            {
                localizedNameAsset.StringChanged -= UpdateNameText;
            }
        }

        private void UpdateNameText(string value)
        {
            unlockText.text = value;
        }

        public void InitUnlockScreen(string key, int unlockCost, Action unlockRoomCallback)
        {
            //nlockText.text = text;

            Initialize(key, unlockCost);

            this.unlockCost = unlockCost;
            //buyButtonText.text = unlockCost + " <sprite name=\"Money\">";
            this.unlockRoomCallback = unlockRoomCallback;
        }

        public void OnUnlock() // Button Event
        {
            Service<AudioManager>.Get().PlaySound("Click");
            Service<MoneyController>.Get().SpendMoney(unlockCost);

            if (Service<MoneyController>.Get().MoneyCount >= unlockCost)
            {
                unlockRoomCallback?.Invoke();

                OnDisplay(false);

                //StartCoroutine(DelayedDisplay(false, 0.15f));
            }
        }

        IEnumerator DelayedDisplay(bool show, float delay)
        {
            yield return new WaitForSeconds(delay);
            OnDisplay(show);
        }
    }
}