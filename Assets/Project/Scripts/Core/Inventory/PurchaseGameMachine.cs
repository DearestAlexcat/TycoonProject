using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;

namespace IdleTycoon
{
    public class PurchaseGameMachine : MonoBehaviour
    {
        [SerializeField] LocalizedString localizedCountText;
        //[SerializeField] LocalizedString localizedNameAsset;
        //[SerializeField] LocalizedString localizedDescriptionAsset;

        //LocalizedStringHelper localizedCountText;
        LocalizedStringHelper localizedNameAsset;
        LocalizedStringHelper localizedDescriptionAsset;

        [Space]
        [HideInInspector] public GameMachineType type;
        public Image icon;

        public TMP_Text nameText;
        public TMP_Text description;

        public TMP_Text purchaseCostText;
        public TMP_Text countText;
        [HideInInspector] public int purchaseCost;

        [Space]
        [SerializeField] Button buyButton;
        [SerializeField] GameObject ñategoryBlocker;
        [SerializeField] TMP_Text ñategoryBlockerText;

        public void Initialize()
        {
            //localizedCountText = new LocalizedStringHelper();
            //localizedCountText.OnUpdateText = UpdateCountText;
            //localizedCountText.LocalizeText("MenuScreenTable", "InStock");
            //localizedCountText.localizedAsset.Arguments = new object[] { count };

            if (localizedCountText != null)
            {
                localizedCountText.StringChanged -= UpdateCountText;
            }

            localizedCountText.Arguments = new object[] { count };
            localizedCountText.StringChanged += UpdateCountText;
        }

        public void InitializeName(string key)
        {
            localizedNameAsset = new LocalizedStringHelper();
            localizedNameAsset.OnUpdateText = UpdateNameText;
            localizedNameAsset.LocalizeText("GameMachineUIData", key, 1);
        }

        public void InitializeDescription(string key)
        {
            localizedDescriptionAsset = new LocalizedStringHelper();
            localizedDescriptionAsset.OnUpdateText = UpdateDescriptionText;
            localizedDescriptionAsset.LocalizeText("GameMachineUIData", key);
        }

        private void OnDisable()
        {
            //localizedCountText.OnDisable();
            localizedNameAsset.OnDisable();
            localizedDescriptionAsset.OnDisable();
        }

        private void UpdateCountText(string value)
        {
            countText.text = value;
        }

        private void UpdateNameText(string value)
        {
            nameText.text = value;
        }

        private void UpdateDescriptionText(string value)
        {
            description.text = value;
        }

        int count;
        public int Count
        {
            get => count;
            set
            {
                count = value;

                localizedCountText.Arguments[0] = count;
                localizedCountText.RefreshString();
            }
        }

        public void OnBuyButton() // Button event
        {
            Service<AudioManager>.Get().PlaySound("Click");

            MoneyController money = Service<MoneyController>.Get();

            if (purchaseCost > money.MoneyCount)
            {
                Service<PopUpMessage>.Get().Display();
                return;
            }

            money.SpendMoney(purchaseCost);

            Count++;

            Service<GameMachineCategoryController>.Get().SetCount(1, type);
        }

        public void SetActivePurchaseCategory(bool value)
        {
            buyButton.enabled = value;
            ñategoryBlocker.SetActive(!value);
        }
    }
}