using TMPro;
using UnityEngine;

namespace IdleTycoon
{
    public class MoneyController : MonoBehaviour
    {
        [SerializeField] TMP_Text moneyText;
        [SerializeField] TMP_Text incomePerMinuteText;

        int moneyCount;
        public int MoneyCount
        {
            get
            {
                return moneyCount;
            }
            set
            {
                moneyCount = value;
                moneyText.text = FormatMoney(value);
            }
        }

        public void SetIncomePerMinute(float value)
        {
            incomePerMinuteText.text = $"+{Mathf.RoundToInt(value)}/min";
        }

        string FormatMoney(int value)
        {
            if (value >= 1_000_000_000)
                return (value / 1_000_000_000f).ToString("0.###") + "B";
            if (value >= 1_000_000)
                return (value / 1_000_000f).ToString("0.###") + "M";
            if (value >= 1_000)
                return (value / 1_000f).ToString("0.###") + "K";

            return value.ToString();
        }

        public void AddMoney(int value)
        {
            if (value < 0f) value = 0;
            MoneyCount += value;

            UpdateMoneyStorageData();
        }

        public void SpendMoney(int value)
        {
            if (value < 0f) value = 0;

            if (value > moneyCount)
            {
                Service<PopUpMessage>.Get().Display();
                return;
            }

            MoneyCount -= value;

            UpdateMoneyStorageData();
        }


        private void Awake()
        {
            Service<MoneyController>.Set(this);
        }

        public void Initialize()
        {
            MoneyCount = GetMoneyStorageData();
        }

        void UpdateMoneyStorageData()
        {
            MoneyStorageData storageData = new MoneyStorageData();
            storageData.money = moneyCount;

            SaveLoadManager.Save<MoneyStorageData>(StorageKeys.Money, storageData);
        }

        int GetMoneyStorageData()
        {
            return SaveLoadManager.Load<MoneyStorageData>(StorageKeys.Money).money;
        }
    }
}