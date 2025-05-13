using System;
using UnityEngine;

namespace IdleTycoon
{
    public class SaveDataVerifier : MonoBehaviour
    {
        private void Awake()
        {
            SaveDataVerifier instance = Service<SaveDataVerifier>.Get();

            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Service<SaveDataVerifier>.Set(this);
            DontDestroyOnLoad(gameObject);
        }


        public void VerifyData(bool resetData = false)
        {
            VerifyUISettingsData(resetData);
            VerifyRoomsData(resetData);
            VerifyMoneyData(resetData);
            VerifyGameMachinesData(resetData);
        }

        public void VerifyUISettingsData(bool resetData)
        {
            if (SaveLoadManager.Load<UISettingsStorageData>(StorageKeys.UISettings) == null || resetData)
            {
                UISettingsStorageData storageData = new();

                storageData.language = 0;
                storageData.sounds = true;
                storageData.music = true;

                SaveLoadManager.Save(StorageKeys.UISettings, storageData);
            }
        }

        public void VerifyRoomsData(bool resetData)
        {
            if (SaveLoadManager.Load<RoomsStorageData>(StorageKeys.Rooms) == null || resetData)
            {
                RoomsStorageData storageData = new();
                storageData.lockedRoomIndex = 0;    // The first room is available for purchase at the start
                SaveLoadManager.Save(StorageKeys.Rooms, storageData);
            }
        }

        public void VerifyMoneyData(bool resetData)
        {
            if (SaveLoadManager.Load<MoneyStorageData>(StorageKeys.Money) == null || resetData)
            {
                MoneyStorageData storageData = new();
                storageData.money = 500000;   // At the start we give n dollars
                SaveLoadManager.Save(StorageKeys.Money, storageData);
            }
        }

        public void VerifyGameMachinesData(bool resetData)
        {
            if (SaveLoadManager.Load<GameMachinesStorageData>(StorageKeys.GameMachines) == null || resetData)
            {
                GameMachinesStorageData storageData = new();

                int group = -1;

                int totalCount = (Enum.GetNames(typeof(GameMachineType)).Length - 2) * 10;

                for (int i = 0; i < totalCount; i++) // 10 - grid cells
                {
                    if (i % 10 == 0) group++;

                    storageData.items.Add(new GameMachineSlotInfo());
                    storageData.items[i].gameMachine = new GameMachineItem();

                    switch (group)
                    {
                        case 0: storageData.items[i].gameMachine.type = GameMachineType.ArcadeMachine; break;
                        case 1: storageData.items[i].gameMachine.type = GameMachineType.DanceMachine; break;
                        case 2: storageData.items[i].gameMachine.type = GameMachineType.AirHockey; break;
                        case 3: storageData.items[i].gameMachine.type = GameMachineType.BasketballGame; break;
                        case 4: storageData.items[i].gameMachine.type = GameMachineType.ClawMachine; break;
                        case 5: storageData.items[i].gameMachine.type = GameMachineType.GamblingMachine; break;
                        case 6: storageData.items[i].gameMachine.type = GameMachineType.Pinball; break;
                    }

                    storageData.items[i].gameMachine.level = (i % 10) + 1;
                    storageData.items[i].count = 0;
                }

                SaveLoadManager.Save(StorageKeys.GameMachines, storageData);
            }
        }
    }
}