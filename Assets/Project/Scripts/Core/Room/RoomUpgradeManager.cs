using UnityEngine;

namespace IdleTycoon
{
    public class RoomUpgradeManager : MonoBehaviour
    {
        [SerializeField] Room room;

        [Space]
        [SerializeField] GameMachineUpgradeData objectData;
        [SerializeField] GameMachineUpgradeUIData upgradeData;

        [Space]
        [SerializeField] RoomStateSwitcher stateSwitcher;

        public int GetCountPlaces => room.gmPlacement.Count;

        public GameMachineType MachineType => room.type;

        public bool IsRoomOpen()
        {
            return room.IsOpen;
        }

        public float GetTotalIncomePerMinute()
        {
            return room.GetTotalIncomePerMinute();
        }

        public void AllowRoomPurchase(bool value)
        {
            stateSwitcher.AllowRoomPurchase(value);
        }

        public int GetCountSpawnedSlotMachines()
        {
            return room.GetCountSpawnedSlotMachines();
        }

        public void SetRoomState(RoomState state)
        {
            stateSwitcher.SetState(state);
        }

        BaseMachinePlacement GetFreePlace()
        {
            foreach (var place in room.gmPlacement)
                if (place.machineLevel == 0)
                    return place;

            return null;
        }

        BaseMachinePlacement GetPlaceWithLevel(int level)
        {
            foreach (var place in room.gmPlacement)
                if (place.machineLevel == level)
                    return place;

            return null;
        }

        public void RemoveMachinesInRoom(int removeLevel, int removeCount)
        {
            for (int i = 0; i < removeCount; i++)
            {
                BaseMachinePlacement place = GetPlaceWithLevel(removeLevel);
                place.FreeUpSlot();
            }
        }

        public void RePlaceGameMachine(int removeLevel, int placeLevel)
        {
            BaseMachineObject machine = objectData.GetMachinePrefab(room.type);

            if (machine != null)
            {
                BaseMachinePlacement place = GetPlaceWithLevel(removeLevel);

                place.FreeUpSlot();

                if (placeLevel > 0) // Или заменяем машину или место пустует
                {
                    SetupGameMachine(machine, place, placeLevel);
                }
            }
            else
            {
                Debug.LogError("Couldn't find a place for the slot machine");
            }
        }


        public bool PlaceGameMachine(int level)
        {
            BaseMachineObject machine = objectData.GetMachinePrefab(room.type);

            if (machine != null)
            {
                BaseMachinePlacement place = GetFreePlace();

                if (place != null)
                {
                    SetupGameMachine(machine, place, level); // Ставим машину на пустое место
                    return true;
                }
            }
            else
            {
                Debug.LogError("Couldn't find a place for the slot machine");
            }

            return false;
        }

        void SetupGameMachine(BaseMachineObject machine, BaseMachinePlacement place, int level)
        {
            BaseMachineObject gm = Object.Instantiate(machine, place.transform.position, place.transform.rotation, place.transform);

            var data = objectData.GetUpgradeDataByMachineType(room.type);

            gm.Initialize(data.colorOffset[level - 1]);

            place.incomePerCycle = upgradeData.GetIncomeByLevel(level - 1);
            place.machineLevel = level;
            place.ObjectInstalledMachine = gm;
        }
    }
}