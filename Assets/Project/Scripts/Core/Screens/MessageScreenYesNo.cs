using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdleTycoon
{
    public class MessageScreenYesNo : ScreenBase
    {
        [Space]
        [SerializeField] TMP_Text message;
        [SerializeField] Button buttonYes;

        public Action callbackYes;

        LocalizedStringHelper localizedStringHelper;

        void Awake()
        {
            Service<MessageScreenYesNo>.Set(this);
        }

        private void OnEnable()
        {
            localizedStringHelper = new LocalizedStringHelper();
            localizedStringHelper.OnUpdateText = UpdateNameText;
        }

        public void YesButton()
        {
            Service<AudioManager>.Get().PlaySound("Click");
            callbackYes?.Invoke();
        }

        public void InitializeMessage(string key)
        {
            localizedStringHelper.LocalizeText("MenuScreenTable", key);
        }

        private void OnDisable()
        {
            localizedStringHelper.OnDisable();
        }

        private void UpdateNameText(string value)
        {
            message.text = value;
        }
    }
}