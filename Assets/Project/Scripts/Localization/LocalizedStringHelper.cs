using UnityEngine.Localization;

namespace IdleTycoon
{
    class LocalizedStringHelper
    {
        public LocalizedString localizedAsset;

        public System.Action<string> OnUpdateText;

        private void UpdateText(string value)
        {
            OnUpdateText?.Invoke(value);
        }

        public void LocalizeText(string tableReference, string tableEntryReference)
        {
            if (localizedAsset != null)
            {
                localizedAsset.StringChanged -= UpdateText;
            }

            localizedAsset = new LocalizedString
            {
                TableReference = tableReference,
                TableEntryReference = tableEntryReference
            };

            localizedAsset.StringChanged += UpdateText;
        }

        public void LocalizeText(string tableReference, string tableEntryReference, int argument1)
        {
            if (localizedAsset != null)
            {
                localizedAsset.StringChanged -= UpdateText;
            }

            localizedAsset = new LocalizedString
            {
                TableReference = tableReference,
                TableEntryReference = tableEntryReference
            };

            localizedAsset.Arguments = new object[] { argument1 };
            localizedAsset.StringChanged += UpdateText;
        }

        public void LocalizeText(string tableReference, string tableEntryReference, ref int argument1)
        {
            if (localizedAsset != null)
            {
                localizedAsset.StringChanged -= UpdateText;
            }

            localizedAsset = new LocalizedString
            {
                TableReference = tableReference,
                TableEntryReference = tableEntryReference
            };

            localizedAsset.Arguments = new object[] { argument1 };
            localizedAsset.StringChanged += UpdateText;
        }

        public void OnDisable()
        {
            if (localizedAsset != null)
            {
                localizedAsset.StringChanged -= UpdateText;
            }
        }
    }
}