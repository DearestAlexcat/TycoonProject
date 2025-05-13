using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdleTycoon
{
    public class RoomUpgradeService : MonoBehaviour
    {
        [SerializeField] List<RoomUpgradeManager> roomUpgradeManagers;

        int lockedRoomIndex; // »ндекс указывает на комнату которую можно купить

        private void Awake()
        {
            Service<RoomUpgradeService>.Set(this);
        }

        public void Initialize()
        {
            RoomsStorageData data = GetRoomsStorageData();
            lockedRoomIndex = data.lockedRoomIndex;

            SetRoomsState();
            EnableRoomForPurchase();
        }

        void SetupTotalIncomePerMinute()
        {
            float incomePerMinute = 0f;

            foreach (var roomUpgradeManager in roomUpgradeManagers)
            {
                incomePerMinute += roomUpgradeManager.GetTotalIncomePerMinute();
            }

            Service<MoneyController>.Get().SetIncomePerMinute(incomePerMinute);
        }

        public void InitGameMachineCategory(GameMachineCategory category)
        {
            foreach (var item in roomUpgradeManagers)
            {
                if (item.MachineType == category.type)
                {
                    category.SetActiveMachineCategory(item.IsRoomOpen());
                    Service<PurchaseGameMachineController>.Get().SetActivePurchaseCategoryUI(category.type, item.IsRoomOpen()); //!
                }
            }
        }

        void SetRoomsState()
        {
            for (int i = 0; i < roomUpgradeManagers.Count; i++)
            {
                if (i <= lockedRoomIndex - 1)
                    roomUpgradeManagers[i].SetRoomState(RoomState.Open);
                else
                    roomUpgradeManagers[i].SetRoomState(RoomState.Close);
            }
        }

        public void EnableRoomForPurchase()
        {
            foreach (RoomUpgradeManager manager in roomUpgradeManagers)
            {
                if (!manager.IsRoomOpen())
                    manager.AllowRoomPurchase(false);
            }

            if (lockedRoomIndex < roomUpgradeManagers.Count)
            {
                roomUpgradeManagers[lockedRoomIndex].AllowRoomPurchase(true);
            }
        }

        public void UpdateRoomsStorageData()
        {
            RoomsStorageData storageData = new RoomsStorageData();
            storageData.lockedRoomIndex = lockedRoomIndex;

            SaveLoadManager.Save<RoomsStorageData>(StorageKeys.Rooms, storageData);
        }

        RoomsStorageData GetRoomsStorageData()
        {
            return SaveLoadManager.Load<RoomsStorageData>(StorageKeys.Rooms);
        }

        public void IncrementUnlockedRoom()
        {
            lockedRoomIndex++;
            EnableRoomForPurchase();

            UpdateRoomsStorageData();
        }

        public void PlaceMachinesInRooms()
        {
            // »нициализаци€ игровых комнат машинами соответствующего уровн€
            foreach (GameMachineType type in Enum.GetValues(typeof(GameMachineType)))
            {
                if (type == GameMachineType.None || type == GameMachineType.ComingSoon)
                    continue;

                ApplyStateActiveMachines(type, Service<GameMachineCategoryController>.Get().GetSlotInfosByType(type));
            }

            SetupTotalIncomePerMinute();
        }

        public void PlaceMachineInRoom(GameMachineType type)
        {
            // »нициализаци€ игровой комнаты машинами соответствующего уровн€
            if (type == GameMachineType.None || type == GameMachineType.ComingSoon)
            {
                Debug.LogError("Invalid GameMachineType");
            }
            else
            {
                PlaceGameMachines(type, Service<GameMachineCategoryController>.Get().GetSlotInfosByType(type));
            }

            SetupTotalIncomePerMinute();
        }

        public void RemoveMachinesInRoom(GameMachineType machineType, int removeLevel, int removeCount)
        {
            foreach (var manager in roomUpgradeManagers)
            {
                if (manager.MachineType != machineType) continue;

                if (manager.IsRoomOpen())
                {
                    manager.RemoveMachinesInRoom(removeLevel, removeCount);
                    return;
                }
            }
        }

        public void RePlaceGameMachines(GameMachineType machineType, int removeLevel, List<GameMachineSlotInfo> gameMachineSlots)
        {
            foreach (var manager in roomUpgradeManagers)
            {
                if (manager.MachineType != machineType) continue;

                if (manager.IsRoomOpen())
                {
                    int placeLevel = 0;

                    for (int i = gameMachineSlots.Count - 1; i >= 0; i--)
                    {
                        if (gameMachineSlots[i].count - gameMachineSlots[i].activeMachines > 0) // Ѕерем машину из склада
                        {
                            gameMachineSlots[i].activeMachines++;
                            placeLevel = gameMachineSlots[i].gameMachine.level;
                            break;
                        }
                    }

                    manager.RePlaceGameMachine(removeLevel, placeLevel);
                }

                SetupTotalIncomePerMinute();

                return;
            }
        }

        void PlaceGameMachines(GameMachineType machineType, List<GameMachineSlotInfo> gameMachineSlots)
        {
            foreach (var room in roomUpgradeManagers)
            {
                if (room.MachineType != machineType) continue;

                if (room.IsRoomOpen())
                {
                    int count = room.GetCountSpawnedSlotMachines();

                    for (int i = gameMachineSlots.Count - 1; i >= 0; i--) // ¬ приоритете установка машины высшего уровн€
                    {
                        for (int j = 0; j < gameMachineSlots[i].count; j++)
                        {
                            if (count == room.GetCountPlaces) // ≈сли все места зан€ты
                                return;

                            if (gameMachineSlots[i].count - gameMachineSlots[i].activeMachines == 0) // ≈сли на складе пусто
                                break;

                            if (room.PlaceGameMachine(gameMachineSlots[i].gameMachine.level))
                            {
                                gameMachineSlots[i].activeMachines++;
                                count++;
                            }
                            else return;
                        }
                    }
                }

                return;
            }
        }

        // ѕосле перезахода в работу производитс€ установка машин в соответствии со значением activeMachines
        public void ApplyStateActiveMachines(GameMachineType machineType, List<GameMachineSlotInfo> gameMachineSlots)
        {
            foreach (var room in roomUpgradeManagers)
            {
                if (room.MachineType != machineType) continue;

                if (room.IsRoomOpen())
                {
                    for (int i = gameMachineSlots.Count - 1; i >= 0; i--) // ¬ приоритете установка машины высшего уровн€
                    {
                        for (int j = 0; j < gameMachineSlots[i].activeMachines; j++)
                        {
                            room.PlaceGameMachine(gameMachineSlots[i].gameMachine.level);
                        }
                    }
                }

                return;
            }
        }
    }
}