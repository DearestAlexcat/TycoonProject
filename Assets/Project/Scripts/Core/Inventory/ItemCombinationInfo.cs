using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace IdleTycoon
{
    public class ItemCombinationInfo : MonoBehaviour
    {
        [SerializeField] Image iconCombine1, iconCombine2, iconCombineResult;
        [SerializeField] TMP_Text numCombinationsTxt, priceCombineTxt, newItemName, newItemDescription;
        [SerializeField] Button incButton, decButton, combineButton;
        [SerializeField] TMP_Text iconCombine1LvlText, iconCombine2LvlText, iconCombineResultLvlText;


        public System.Action<int> OnCombinationItemCountChanged;

        int numCombinations;
        int priceCombine;

        public Sprite IconCombineResult => iconCombineResult.sprite;

        [Header("Combine result")]
        [SerializeField] CombineResultScreen combineResultScreen;

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

        public int NumCombinations
        {
            get
            {
                return numCombinations;
            }
            set
            {
                numCombinations = value < 0 ? 0 : value;
                numCombinationsTxt.text = numCombinations.ToString();
            }
        }

        public int PriceCombine
        {
            get
            {
                return priceCombine;
            }
            set
            {
                priceCombine = value;
                priceCombineTxt.text = priceCombine + " <sprite name=\"Money\">";
            }
        }

        private void UpdateNameText(string value)
        {
            newItemName.text = value;
        }

        private void UpdateDescriptionText(string value)
        {
            newItemDescription.text = value;
        }

        public void OnCombineItems()
        {
            Service<GameMachineCategoryController>.Get().OnCombineItems();
        }

        public void InitCombineResultScreen(Sprite icon, int value, int level)
        {
            combineResultScreen.Init(icon, value, level);
        }

        public void SetNewItemBonus(int value)
        {
            newItemDescription.gameObject.SetActive(value != 0);

            InitializeDescription("BringMore", value);
        }

        public void SetNewItemName(string name, int level)
        {
            newItemName.gameObject.SetActive(!string.IsNullOrEmpty(name));

            InitializeName(name, level);
        }


        public void SetIcon1(Sprite icon, int level = 0)
        {
            iconCombine1.gameObject.SetActive(icon != null);
            iconCombine1.sprite = icon;
            iconCombine1LvlText.text = $"lvl. {level}";
        }

        public void SetIcon2(Sprite icon, int level = 0)
        {
            iconCombine2.gameObject.SetActive(icon != null);
            iconCombine2.sprite = icon;
            iconCombine2LvlText.text = $"lvl. {level}";
        }

        public void SetCombinationResultIcon(Sprite icon, int level = 0)
        {
            iconCombineResult.gameObject.SetActive(icon != null);
            iconCombineResult.sprite = icon;
            iconCombineResultLvlText.text = $"lvl. {level}";
        }

        public void SetActiveIncDecButtons(bool value)
        {
            incButton.enabled = value;
            decButton.enabled = value;
        }

        public void OnChangeCombinationItemCallback(int value)
        {
            Service<AudioManager>.Get().PlaySound("Click");
            OnCombinationItemCountChanged?.Invoke(value);
        }

        public void ChangeCombinationItemCount(int numCombine, int availableItemsCount) // Button event
        {
            if (numCombinations + numCombine < 1 || (numCombinations + numCombine) * 2 > availableItemsCount)
                return;

            NumCombinations += numCombine;
        }

        public void SetPrice(int price)
        {
            bool value = price != 0 && price * numCombinations <= Service<MoneyController>.Get().MoneyCount;
            combineButton.interactable = value;
            PriceCombine = price * numCombinations;
        }

        public void SpendMoney()
        {
            Service<MoneyController>.Get().SpendMoney(priceCombine);
        }
    }
}