using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace IdleTycoon
{
    public class ItemSellInfo : MonoBehaviour
    {
        [SerializeField] Image itemSellIcon;
        [SerializeField] TMP_Text itemSellCountTxt, sellPriceTxt, itemName, itemDescription, showroomCountText, warehouseCountText;
        [SerializeField] Button incButton, decButton;
        [SerializeField] Button sellButton;
        [SerializeField] TMP_Text itemSellLvlText;

        [Header("Sell confirmation")]
        [SerializeField] SellConfirmationScreen sellConfirmationScreen;

        [SerializeField] LocalizedString localizedNameAsset;
        [SerializeField] LocalizedString localizedDescriptionAsset;
        [SerializeField] LocalizedString localizedShowroomCountText;
        [SerializeField] LocalizedString localizedWarehouseCountText;

        public System.Action<int> OnSellItemCountChanged;

        int sellItemCount;
        int sellPrice;
        int warehouseCount;

        public int SellItemCount
        {
            get
            {
                return sellItemCount;
            }
            set
            {
                sellItemCount = value < 0 ? 0 : value;
                itemSellCountTxt.text = sellItemCount.ToString();
            }
        }

        public int SellPrice
        {
            get
            {
                return sellPrice;
            }
            set
            {
                sellPrice = value;
                sellPriceTxt.text = sellPrice + " <sprite name=\"Money\">";
                sellConfirmationScreen.sellPrice.text = sellPrice.ToString();
            }
        }

        public void OnDisplayConfirmationScreen()
        {
            Service<AudioManager>.Get().PlaySound("Click");
            sellConfirmationScreen.Show(warehouseCount > 0);
        }

        public void SetCountText(int count, int activeMachines)
        {
            warehouseCount = count - activeMachines;

            InitializeShowroomCountText("InHall", activeMachines);
            InitializeWarehouseCountText("InStok", warehouseCount);
        }
        public void InitializeShowroomCountText(string key, int value)
        {
            if (localizedShowroomCountText != null)
            {
                localizedShowroomCountText.StringChanged -= UpdateShowroomCountText;
            }

            localizedShowroomCountText = new LocalizedString
            {
                TableReference = "MachineUpgradeUIData",
                TableEntryReference = key
            };

            localizedShowroomCountText.Arguments = new object[] { value };
            localizedShowroomCountText.StringChanged += UpdateShowroomCountText;
        }

        public void InitializeWarehouseCountText(string key, int value)
        {
            if (localizedWarehouseCountText != null)
            {
                localizedWarehouseCountText.StringChanged -= UpdateWarehouseCountText;
            }

            localizedWarehouseCountText = new LocalizedString
            {
                TableReference = "MachineUpgradeUIData",
                TableEntryReference = key
            };

            localizedWarehouseCountText.Arguments = new object[] { value };
            localizedWarehouseCountText.StringChanged += UpdateWarehouseCountText;
        }

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

            if (localizedShowroomCountText != null)
            {
                localizedShowroomCountText.StringChanged -= UpdateShowroomCountText;
            }

            if (localizedWarehouseCountText != null)
            {
                localizedWarehouseCountText.StringChanged -= UpdateWarehouseCountText;
            }
        }

        private void UpdateNameText(string value)
        {
            itemName.text = value;
        }

        private void UpdateDescriptionText(string value)
        {
            itemDescription.text = value;
        }

        private void UpdateShowroomCountText(string value)
        {
            showroomCountText.text = value;
        }

        private void UpdateWarehouseCountText(string value)
        {
            warehouseCountText.text = value;
        }

        public void SetItemName(string name, int level)
        {
            //itemName.gameObject.SetActive(!string.IsNullOrEmpty(name));
            itemName.text = name;
            InitializeName(name, level);
        }

        public void SetItemBonusText(int value)
        {
            //itemDescription.gameObject.SetActive(value != 0);
            InitializeDescription("BringMore", value);
        }

        public void SetIcon(Sprite icon, int level)
        {
            itemSellIcon.gameObject.SetActive(icon != null);
            itemSellIcon.sprite = icon;
            sellConfirmationScreen.iconSellItem.sprite = icon;
            itemSellLvlText.text = $"lvl. {level}";
        }

        public void SetActiveIncDecButtons(bool value)
        {
            incButton.enabled = value;
            decButton.enabled = value;
        }

        public void ChangeSellItemCount(int numItems, int availableItemsCount) // Button event
        {
            if (sellItemCount + numItems < 1 || sellItemCount + numItems > availableItemsCount)
                return;

            SellItemCount += numItems;
        }

        public void SetSellPrice(int sellPrice)
        {
            sellButton.interactable = sellPrice != 0;
            SellPrice = sellPrice * sellItemCount;
        }

        public void OnChangeCombinationItemCallback(int value)
        {
            Service<AudioManager>.Get().PlaySound("Click");
            OnSellItemCountChanged?.Invoke(value);
        }

        public void AddMoney()
        {
            Service<MoneyController>.Get().AddMoney(sellPrice);
        }
    }
}