using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdleTycoon
{
    public class PurchaseGameMachineController : MonoBehaviour
    {
        [SerializeField] GameMachineUIData machineUIData;
        [SerializeField] Transform itemParent;
        [SerializeField] GameMachineCategoryController categoryController;

        [Space]
        [SerializeField] PurchaseGameMachine purchaseGameMachinePrefab;
        [SerializeField] PurchaseGameMachine comingSoonPrefab;

        List<PurchaseGameMachine> purchaseGameMachineInfos;

        private void Awake()
        {
            Service<PurchaseGameMachineController>.Set(this);
        }

        public void Initialize()
        {
            CategorySectionInit();
        }

        public void SetActivePurchaseCategoryUI(GameMachineType type, bool value)
        {
            if (purchaseGameMachineInfos == null) return;

            foreach (var item in purchaseGameMachineInfos)
            {
                if (item.type == type)
                {
                    item.SetActivePurchaseCategory(value);
                    return;
                }
            }
        }

        void CategorySectionInit()
        {
            PurchaseGameMachine item;
            GameMachineDisplayData data;

            purchaseGameMachineInfos = new List<PurchaseGameMachine>();

            foreach (GameMachineType type in Enum.GetValues(typeof(GameMachineType)))
            {
                if (type != GameMachineType.None)
                {
                    data = machineUIData.GetData(type);

                    if (type == GameMachineType.ComingSoon)
                    {
                        item = Instantiate(comingSoonPrefab, itemParent);
                    }
                    else
                    {
                        item = Instantiate(purchaseGameMachinePrefab, itemParent);
                        item.Initialize();

                        item.purchaseCostText.text = data.purchaseCost + " <sprite name=\"Money\">";
                        item.purchaseCost = data.purchaseCost;

                        purchaseGameMachineInfos.Add(item);
                    }

                    item.type = type;

                    item.name = type.ToString();
                    item.icon.sprite = data.sprite;

                    item.InitializeName(data.name);
                    item.InitializeDescription(data.description);
                }
            }
        }

        public void SetItemQuantity(int count, GameMachineType type)
        {
            foreach (var item in purchaseGameMachineInfos)
            {
                if (item.type == type)
                {
                    item.Count = count;
                    return;
                }
            }
        }
    }
}