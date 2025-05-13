using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdleTycoon
{
    public class GameMachineCategoryController : MonoBehaviour
    {
        [Space]
        [SerializeField] GameMachineUIData machineUIData;
        [SerializeField] List<GameMachineUpgradeUIData> gmUpgradeUIDatas;
        [SerializeField] Transform itemParent;

        [Space]
        [SerializeField] GameMachineCategory gameMachineCategoryPrefab;
        [SerializeField] GameMachineCategory comingSoonPrefab;

        [Space]
        List<GameMachineCategory> gameMachineCategories;                                    // 7
        [SerializeField] List<GameMachineDisplaySlot> gameMachineButtonGrid;                // 10
        [SerializeField, HideInInspector] List<GameMachineSlotInfo> gameMachineSlotInfos;   // 7 * 10

        [Space]
        [SerializeField] ItemCombinationInfo combinationInfo;
        [SerializeField] ItemSellInfo itemSellInfo;

        int selectedGameMachineLevel;
        GameMachineSlotInfo selectedSlot;
        GameMachineType selectedGameMachineType;

        private void Awake()
        {
            Service<GameMachineCategoryController>.Set(this);
        }

        public void Initialize()
        {
            LoadGameMachineSaves(false);
            Service<RoomUpgradeService>.Get().PlaceMachinesInRooms();

            SetPurchasedItemCount();
            CategorySectionInit();
            UpdateCategorySectionCounter();
            SetDefaultActiveSlot();

            combinationInfo.OnCombinationItemCountChanged = ChangeCombinationItemCount;
            itemSellInfo.OnSellItemCountChanged = ChangeSellItemCount;
        }

        public void SetActiveMachineCategoryUI(GameMachineType type, bool value)
        {
            if (gameMachineCategories == null) return;

            foreach (var item in gameMachineCategories)
            {
                if (item.type == type)
                {
                    item.SetActiveMachineCategory(value);
                    return;
                }
            }
        }

        void SetPurchasedItemCount()
        {
            int i = 0;

            int totalCount = (Enum.GetNames(typeof(GameMachineType)).Length - 2);

            // В магазине покупаются только первые уровни. В цикле идет перебор первых уровней.
            for (int j = 0; j < totalCount; j++)
            {
                Service<PurchaseGameMachineController>.Get().SetItemQuantity(gameMachineSlotInfos[i].count, gameMachineSlotInfos[i].gameMachine.type);
                i += 10;
            }
        }

        void CategorySectionInit()
        {
            gameMachineCategories = new List<GameMachineCategory>();

            GameMachineCategory item;

            foreach (GameMachineType type in Enum.GetValues(typeof(GameMachineType)))
            {
                if (type != GameMachineType.None)
                {
                    if (type == GameMachineType.ComingSoon)
                    {
                        item = Instantiate(comingSoonPrefab, itemParent);
                    }
                    else
                    {
                        item = Instantiate(gameMachineCategoryPrefab, itemParent);
                        item.type = type;
                        item.callback = SelectGroup;

                        gameMachineCategories.Add(item);

                        Service<RoomUpgradeService>.Get().InitGameMachineCategory(item);
                    }

                    item.name = type.ToString();
                    item.icon.sprite = machineUIData.GetData(type).sprite;
                }
            }
        }

        public void SetCount(int value, GameMachineType type)
        {
            foreach (var сategory in gameMachineCategories)
            {
                if (type == сategory.type)
                {
                    сategory.Count += value; // Обновляем кол-во в котегории

                    // В магазине покупаются машины только первого уровня, обновляем соответствующие данные
                    GameMachineSlotInfo slot = GetSlotInfosByTypeAndLevel(type, 1);
                    slot.count += value;

                    // Установит машину если есть свободное место
                    Service<RoomUpgradeService>.Get().PlaceMachineInRoom(type);

                    if (type == selectedGameMachineType) // Если категория не выбрана, то визуал не обновляем
                    {
                        gameMachineButtonGrid[0].SetCount(slot.count);
                        SetCombineInfo();
                        SetSellItemInfo();
                    }

                    UpdateGameMachineStorageData();

                    return;
                }
            }
        }

        // -------------------------------------------------------------------------

        public void LoadGameMachineSaves(bool gen = false)
        {
            if (gen == false)
            {
                gameMachineSlotInfos = GetGameMachineStorageData();
            }
            else
            {
                gameMachineSlotInfos = new List<GameMachineSlotInfo>();

                int group = -1;

                int totalCount = (Enum.GetNames(typeof(GameMachineType)).Length - 2) * 10;

                for (int i = 0; i < totalCount; i++)
                {
                    if (i % 10 == 0) group++;

                    gameMachineSlotInfos.Add(new GameMachineSlotInfo());
                    gameMachineSlotInfos[i].gameMachine = new GameMachineItem();

                    switch (group)
                    {
                        case 0: gameMachineSlotInfos[i].gameMachine.type = GameMachineType.ArcadeMachine; break;
                        case 1: gameMachineSlotInfos[i].gameMachine.type = GameMachineType.DanceMachine; break;
                        case 2: gameMachineSlotInfos[i].gameMachine.type = GameMachineType.AirHockey; break;
                        case 3: gameMachineSlotInfos[i].gameMachine.type = GameMachineType.BasketballGame; break;
                        case 4: gameMachineSlotInfos[i].gameMachine.type = GameMachineType.ClawMachine; break;
                        case 5: gameMachineSlotInfos[i].gameMachine.type = GameMachineType.GamblingMachine; break;
                        case 6: gameMachineSlotInfos[i].gameMachine.type = GameMachineType.Pinball; break;
                    }

                    gameMachineSlotInfos[i].gameMachine.level = (i % 10) + 1;

                    //if (UnityEngine.Random.value < 1)
                    {
                        gameMachineSlotInfos[i].count = 2;//UnityEngine.Random.Range(8, 9);
                    }
                    //else
                    {
                        //gameMachineSlotInfos[i].count = 0;
                    }

                    UpdateGameMachineStorageData();
                }
            }
        }

        void UpdateGameMachineStorageData()
        {
            GameMachinesStorageData storageData = new GameMachinesStorageData();
            storageData.items = gameMachineSlotInfos;

            SaveLoadManager.Save<GameMachinesStorageData>(StorageKeys.GameMachines, storageData);
        }

        List<GameMachineSlotInfo> GetGameMachineStorageData()
        {
            return SaveLoadManager.Load<GameMachinesStorageData>(StorageKeys.GameMachines).items;
        }

        public void UpdateCategorySectionCounter()
        {
            Dictionary<GameMachineType, int> typeCount = new Dictionary<GameMachineType, int>();

            foreach (GameMachineType type in Enum.GetValues(typeof(GameMachineType)))
            {
                if (type == GameMachineType.None)
                    continue;
                typeCount.Add(type, 0);
            }

            foreach (var item in gameMachineSlotInfos)
            {
                typeCount[item.gameMachine.type] += item.count;
            }

            foreach (var category in gameMachineCategories)
            {
                if (category.type == GameMachineType.None)
                    continue;
                category.Count = typeCount[category.type];
            }
        }

        // -----------------------------------------------------------------------------------------------------------------------

        GameMachineUpgradeDisplayData GetGMUpgradeDisplayData(GameMachineType type, int level)
        {
            foreach (var item in gmUpgradeUIDatas)
            {
                if (item.type == type)
                {
                    int index = level - 1;

                    if (index < 0 || index >= item.upgradeDatas.Count)
                        return null;

                    return item.upgradeDatas[index];
                }
            }

            return null;
        }

        void SetDefaultActiveSlot()
        {
            selectedGameMachineLevel = 1;

            SelectGroup(GameMachineType.ArcadeMachine);

            InitCallbackGameMachineSlot();
            ChooseGameMachineSlot(selectedGameMachineLevel);
        }

        public void InitCallbackGameMachineSlot()
        {
            foreach (var item in gameMachineButtonGrid)
            {
                item.callback = ChooseGameMachineSlot;
            }
        }

        public void ChooseGameMachineSlot(int level)
        {
            selectedGameMachineLevel = level;

            foreach (var item in gameMachineButtonGrid)
            {
                item.SetActiveFrame(item.Level == level);
            }

            SetCombineInfo();
            SetSellItemInfo();
        }

        public void SelectGroup(GameMachineType type) // Button event
        {
            selectedGameMachineType = type;

            // Select the desired group and reset the selection of the rest
            foreach (var item in gameMachineCategories)
            {
                item.SetActiveFrame(item.type == selectedGameMachineType);
            }

            // Displays available gaming machines of the selected group
            foreach (var item in gameMachineButtonGrid)
            {
                item.SetView(GetGMUpgradeDisplayData(selectedGameMachineType, item.Level).sprite);
                item.SetCount(GetSlotInfosByTypeAndLevel(selectedGameMachineType, item.Level).count);
            }

            SetCombineInfo();
            SetSellItemInfo();
        }

        GameMachineSlotInfo GetSlotInfosByTypeAndLevel(GameMachineType type, int level)
        {
            foreach (var item in gameMachineSlotInfos)
            {
                if (item.gameMachine.type == type && item.gameMachine.level == level)
                {
                    return item;
                }
            }

            return null;
        }

        public List<GameMachineSlotInfo> GetSlotInfosByType(GameMachineType type)
        {
            List<GameMachineSlotInfo> results = new(10);

            foreach (var item in gameMachineSlotInfos)
            {
                if (item.gameMachine.type == type)
                {
                    results.Add(item);
                }
            }

            return results;
        }

        // -----------------------------------------------------------------------------------------------------------------------------------------------

        public void SetCombineInfo()
        {
            selectedSlot = GetSlotInfosByTypeAndLevel(selectedGameMachineType, selectedGameMachineLevel);
            Sprite icon = null;

            if (selectedSlot.count > 0 && selectedGameMachineLevel < 10)
            {
                icon = GetGMUpgradeDisplayData(selectedGameMachineType, selectedGameMachineLevel).sprite;
            }

            if (selectedSlot.count > 1 && selectedGameMachineLevel < 10) // You can't upgrade above 10
            {
                combinationInfo.SetIcon1(icon, selectedGameMachineLevel);
                combinationInfo.SetIcon2(icon, selectedGameMachineLevel);
                combinationInfo.SetActiveIncDecButtons(true);

                combinationInfo.NumCombinations = 1;

                combinationInfo.SetPrice(GetGMUpgradeDisplayData(selectedGameMachineType, selectedGameMachineLevel).upgradeCost);

                GameMachineUpgradeDisplayData newItemDisplayData = GetGMUpgradeDisplayData(selectedGameMachineType, selectedGameMachineLevel + 1);
                combinationInfo.SetCombinationResultIcon(newItemDisplayData.sprite, selectedGameMachineLevel + 1);

                combinationInfo.SetNewItemName(newItemDisplayData.name, selectedGameMachineLevel + 1);
                combinationInfo.SetNewItemBonus(newItemDisplayData.incomeBonusPercent);
                combinationInfo.InitCombineResultScreen(newItemDisplayData.sprite, newItemDisplayData.incomeBonusPercent, selectedGameMachineLevel + 1);
            }
            else
            {
                combinationInfo.SetIcon1(selectedSlot.count == 1 ? icon : null, selectedGameMachineLevel);
                combinationInfo.SetIcon2(null);
                combinationInfo.SetCombinationResultIcon(null);

                combinationInfo.SetActiveIncDecButtons(false);

                combinationInfo.NumCombinations = 0;
                combinationInfo.SetPrice(0);

                combinationInfo.SetNewItemName("", 0);
                combinationInfo.SetNewItemBonus(0);
            }
        }

        public void OnCombineItems() // Button Event
        {
            Service<AudioManager>.Get().PlaySound("Click");

            GameMachineSlotInfo prevSlot = GetSlotInfosByTypeAndLevel(selectedGameMachineType, selectedGameMachineLevel);
            GameMachineSlotInfo nextSlot = GetSlotInfosByTypeAndLevel(selectedGameMachineType, selectedGameMachineLevel + 1);

            foreach (var item in gameMachineButtonGrid)
            {
                if (item.Level == selectedGameMachineLevel)
                {
                    prevSlot.count -= combinationInfo.NumCombinations * 2;

                    if (selectedGameMachineLevel == 1)
                    {
                        Service<PurchaseGameMachineController>.Get().SetItemQuantity(prevSlot.count, selectedGameMachineType);
                    }

                    //if (prevSlot.count == 0)
                    //    item.SetView(null);
                    item.SetCount(prevSlot.count);
                }

                if (item.Level == selectedGameMachineLevel + 1)
                {
                    if (nextSlot.count == 0)
                        item.SetView(GetGMUpgradeDisplayData(selectedGameMachineType, item.Level).sprite);

                    nextSlot.count += combinationInfo.NumCombinations;
                    item.SetCount(nextSlot.count);

                    break;
                }
            }

            foreach (var group in gameMachineCategories)
            {
                if (group.type == selectedGameMachineType)
                {
                    group.Count -= combinationInfo.NumCombinations;
                    break;
                }
            }

            combinationInfo.SpendMoney();

            // Если есть что удалять и заменить на сцене
            if (prevSlot.activeMachines >= combinationInfo.NumCombinations * 2)
            {
                prevSlot.activeMachines -= combinationInfo.NumCombinations * 2;

                //Service<RoomUpgradeService>.Get().RemoveMachinesInRoom(selectedGameMachineType, selectedGameMachineLevel, combinationInfo.NumCombinations * 2);           
                //Service<RoomUpgradeService>.Get().PlaceMachineInRoom(selectedGameMachineType);
                RePlaceGameMachinesOnScene2();
            }

            SetCombineInfo();
            SetSellItemInfo();

            UpdateGameMachineStorageData();
        }

        public void ChangeCombinationItemCount(int value) // Button event
        {
            combinationInfo.ChangeCombinationItemCount(value, selectedSlot.count);
            combinationInfo.SetPrice(GetGMUpgradeDisplayData(selectedGameMachineType, selectedGameMachineLevel).upgradeCost);
        }


        public void RePlaceGameMachinesOnScene2()
        {
            List<GameMachineSlotInfo> slotInfos = GetSlotInfosByType(selectedGameMachineType);

            for (int i = 0; i < combinationInfo.NumCombinations * 2; i++)
            {
                Service<RoomUpgradeService>.Get().RePlaceGameMachines(selectedGameMachineType, selectedGameMachineLevel, slotInfos);
            }
        }

        // -----------------------------------------------------------------------------------------------------------------------------------------------

        public void SetSellItemInfo()
        {
            selectedSlot = GetSlotInfosByTypeAndLevel(selectedGameMachineType, selectedGameMachineLevel);

            GameMachineUpgradeDisplayData slotData = GetGMUpgradeDisplayData(selectedGameMachineType, selectedGameMachineLevel);

            if (selectedSlot.count > 0)
            {
                itemSellInfo.SetActiveIncDecButtons(true);
                itemSellInfo.SellItemCount = 1;
                itemSellInfo.SetSellPrice(slotData.sellCost);
            }
            else
            {
                itemSellInfo.SetActiveIncDecButtons(false);
                itemSellInfo.SellItemCount = 0;
                itemSellInfo.SetSellPrice(0);
            }

            itemSellInfo.SetIcon(slotData.sprite, selectedGameMachineLevel);
            itemSellInfo.SetItemBonusText(slotData.incomeBonusPercent);

            GameMachineSlotInfo slotInfo = GetSlotInfosByTypeAndLevel(selectedGameMachineType, selectedGameMachineLevel);
            itemSellInfo.SetCountText(slotInfo.count, slotInfo.activeMachines);

            itemSellInfo.SetItemName(slotData.name, selectedGameMachineLevel);
        }

        public void ChangeSellItemCount(int value) // Button event
        {
            itemSellInfo.ChangeSellItemCount(value, selectedSlot.count);
            itemSellInfo.SetSellPrice(GetGMUpgradeDisplayData(selectedGameMachineType, selectedGameMachineLevel).sellCost);
        }

        public void OnSellItem() // Button event
        {
            GameMachineSlotInfo slot = GetSlotInfosByTypeAndLevel(selectedGameMachineType, selectedGameMachineLevel);

            int warehouse = 0;

            foreach (var item in gameMachineButtonGrid)
            {
                if (item.Level == selectedGameMachineLevel)
                {
                    warehouse = slot.count - slot.activeMachines;

                    slot.count -= itemSellInfo.SellItemCount;

                    if (itemSellInfo.SellItemCount > warehouse)
                    {
                        slot.activeMachines -= itemSellInfo.SellItemCount - warehouse;
                    }

                    if (selectedGameMachineLevel == 1)
                    {
                        Service<PurchaseGameMachineController>.Get().SetItemQuantity(slot.count, selectedGameMachineType);
                    }

                    //if (slot.count == 0)
                    //    item.SetView(null);

                    item.SetCount(slot.count);

                    break;
                }
            }

            foreach (var group in gameMachineCategories)
            {
                if (group.type == selectedGameMachineType)
                {
                    group.Count -= itemSellInfo.SellItemCount;
                    break;
                }
            }

            itemSellInfo.AddMoney();

            RePlaceGameMachinesOnScene(warehouse);

            SetCombineInfo();
            SetSellItemInfo();

            UpdateGameMachineStorageData();
        }

        void RePlaceGameMachinesOnScene(int warehouse)
        {
            if (itemSellInfo.SellItemCount > warehouse) // Если машины продаются с игровой комнаты
            {
                int sellCount = itemSellInfo.SellItemCount - warehouse; // Число продаваемых машин: склад + игровая комната

                List<GameMachineSlotInfo> slotInfos = GetSlotInfosByType(selectedGameMachineType);

                for (int i = 0; i < sellCount; i++)
                {
                    Service<RoomUpgradeService>.Get().RePlaceGameMachines(selectedGameMachineType, selectedGameMachineLevel, slotInfos);
                }
            }
        }
    }
}