using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;

namespace IdleTycoon
{
    public class CombineResultScreen : MonoBehaviour
    {
        public Image iconResult;
        public TMP_Text itemDescription, itemLevel;

        [SerializeField] Animator animator;

        [SerializeField] LocalizedString localizedNameAsset;
        [SerializeField] LocalizedString localizedDescriptionAsset;

        public void InitializeName(string key, int value)
        {
            if (localizedNameAsset != null)
            {
                localizedNameAsset.StringChanged -= UpdateNameText;
            }

            localizedNameAsset = new LocalizedString
            {
                TableReference = "MachineUpgradeUIData",
                TableEntryReference = key
            };

            localizedNameAsset.Arguments = new object[] { value };
            localizedNameAsset.StringChanged += UpdateNameText;
        }

        public void InitializeDescription(string key, int value)
        {
            if (localizedDescriptionAsset != null)
            {
                localizedDescriptionAsset.StringChanged -= UpdateDescriptionText;
            }

            localizedDescriptionAsset = new LocalizedString
            {
                TableReference = "MachineUpgradeUIData",
                TableEntryReference = key
            };

            localizedDescriptionAsset.Arguments = new object[] { value };
            localizedDescriptionAsset.StringChanged += UpdateDescriptionText;
        }

        private void OnDisable()
        {
            if (localizedNameAsset != null)
            {
                localizedNameAsset.StringChanged -= UpdateNameText;
            }

            if (localizedDescriptionAsset != null)
            {
                localizedDescriptionAsset.StringChanged -= UpdateDescriptionText;
            }
        }

        private void UpdateNameText(string value)
        {
            itemLevel.text = value;
        }

        private void UpdateDescriptionText(string value)
        {
            itemDescription.text = value;
        }

        public void Init(Sprite icon, int value, int level)
        {
            iconResult.sprite = icon;

            InitializeName("OpenNewGameMachine", level);
            InitializeDescription("BringIncome", value);
        }

        public void Close()
        {
            Service<AudioManager>.Get().PlaySound("Click");
            gameObject.SetActive(false);
        }
    }
}