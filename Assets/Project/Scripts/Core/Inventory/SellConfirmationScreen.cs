using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace IdleTycoon
{
    public class SellConfirmationScreen : MonoBehaviour
    {
        [SerializeField] TMP_Text itemDescription;
        public Image iconSellItem;
        public TMP_Text sellPrice;

        [SerializeField] LocalizedString localizedDescriptionAsset;

        public void InitializeDescription(string key)
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

            localizedDescriptionAsset.StringChanged += UpdateDescriptionText;
        }

        private void OnDisable()
        {
            if (localizedDescriptionAsset != null)
            {
                localizedDescriptionAsset.StringChanged -= UpdateDescriptionText;
            }
        }

        public void OnSellItem()
        {
            Service<GameMachineCategoryController>.Get().OnSellItem();
        }

        private void UpdateDescriptionText(string value)
        {
            itemDescription.text = value;
        }

        public void Show(bool inWarehouse)
        {
            if (inWarehouse)
            {
                InitializeDescription("SellFromWarehouse");
            }
            else
            {
                InitializeDescription("SellFromGameRoom");
            }

            gameObject.SetActive(true);
        }

        public void Close()
        {
            Service<AudioManager>.Get().PlaySound("Click");
            gameObject.SetActive(false);
        }
    }
}