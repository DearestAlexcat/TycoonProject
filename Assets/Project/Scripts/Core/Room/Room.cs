using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleTycoon
{
    public class Room : MonoBehaviour
    {
        public GameMachineType type;
        [SerializeField] BoxCollider inventoryTrigger;
        public float machineRunDuration;
        [SerializeField] MoneyExplosionCallback moneyFXPrefab;

        [Space]
        public Transform enterPoint;                            // ����� ����� / �������� ������� (��� / ������� / ... / ����)
        public Transform exitPoint;                             // ����� ������ / �������� ������� (������ ��� / ���� / ... / ����)

        [Space]
        public List<Transform> queuePoints;     // ����� �������
        public BitArray occupiedQueuePoint;     // ��������� �����

        LinkedList<Unit> waitingUnits = new LinkedList<Unit>(); // ����� � �������
        List<Unit> activeUnits = new List<Unit>();              // ����� � �������

        public List<BaseMachinePlacement> gmPlacement = new List<BaseMachinePlacement>();

        Pooler<MoneyExplosionCallback> moneyFXPool;

        bool isOpen = false;
        public int occupiedQueuePointCount = 0;

        public bool IsOpen
        {
            get => isOpen;
            set
            {
                isOpen = value;
                inventoryTrigger.enabled = value;

                Service<PurchaseGameMachineController>.Get().SetActivePurchaseCategoryUI(type, value);
                Service<GameMachineCategoryController>.Get().SetActiveMachineCategoryUI(type, value);
            }
        }

        void Start()
        {
            foreach (var place in gmPlacement)
            {
                place.OnPlayMoneyFX = PlayMoneyFX;
                place.machineRunDuration = machineRunDuration;
            }

            occupiedQueuePoint = new BitArray(queuePoints.Count);
        }

        public void PlayMoneyFX(Vector3 position)
        {
            if(moneyFXPool == null)
            {
                moneyFXPool = new Pooler<MoneyExplosionCallback>(moneyFXPrefab, Mathf.RoundToInt(GetCountSpawnedSlotMachines() * 0.4f));
            }

            MoneyExplosionCallback ps = moneyFXPool.Get(position);
            ps.Play();
        }

        public int GetAvailableGameMachineSpotCount()
        {
            int total = 0;
            foreach (var place in gmPlacement)
            {
                if(place.ObjectInstalledMachine != null)
                {
                    total += place.playingPlaces.Count;
                }
            }
            return total;
        }

        public int GetCountGameMachinePlacesOccupied()
        {
            int total = 0;
            foreach (var place in gmPlacement)
            {
                total += place.occupiedPlacePointCount;
            }
            return total;
        }

        public float GetTotalIncomePerMinute()
        {
            float totalIncomePerMinute = 0f;

            foreach (var slot in gmPlacement)
            {
                if (slot.ObjectInstalledMachine != null)
                {
                    float income = slot.GetIncomePerMinute();
                    totalIncomePerMinute += income;
                }
            }
            return totalIncomePerMinute;
        }

        public bool IsRoomOpenToVisitors()
        {
            return IsOpen && GetCountSpawnedSlotMachines() > 0;
        }

        public int GetCountSpawnedSlotMachines()
        {
            int count = 0;

            foreach (var slot in gmPlacement)
                if (slot.ObjectInstalledMachine != null)
                    count++;

            return count;
        }

        public MachineSeat GetAvailableMachinePoint() // ������� ���������� ������ �������
        {
            MachineSeat result = null;

            foreach (var machine in gmPlacement)
            {
                result = machine.GetAvailableMachinePoint();
                if (result != null)
                    return result;
            }

            return result;
        }

        public Transform GetNextQueuePoint() // �������� ��������� ��������� ����� �������
        {
            for (int i = 0; i < queuePoints.Count; i++)
            {
                if (!occupiedQueuePoint[i])
                {
                    occupiedQueuePoint[i] = true;
                    occupiedQueuePointCount++;
                    return queuePoints[i];
                }
            }

            return null;
        }

        public void VacateQueuePoint(int index)
        {
            if (index < 0 || index >= queuePoints.Count) return;
            occupiedQueuePoint[index] = false;
            occupiedQueuePointCount--;
        }

        public void AddToQueue(Unit unit)  // �������� ����� � �������
        {
            if (!waitingUnits.Contains(unit))
                waitingUnits.AddLast(unit);
        }

        public void AddToRoom(Unit unit) // �������� ����� � �������
        {
            if (!activeUnits.Contains(unit))
                activeUnits.Add(unit);
        }

        public void RemoveFromRoom(Unit unit) // ������� ����� �� �������
        {
            activeUnits.Remove(unit);
        }

        public void PlayMachine(Unit unit)
        {
            MachineSeat machineSeat = unit.machineSeat;

            machineSeat.machinePlacement.StartAnimation((bool gameOutcome) => EndPlayMachine(unit, gameOutcome));
        }

        void EndPlayMachine(Unit unit, bool gameOutcome)
        {
            unit.EndPlayMachine(gameOutcome);
            RemoveFromRoom(unit);

            if (waitingUnits.Count > 0)
            {
                waitingUnits.First.Value.GoToSelectedRoom(unit.machineSeat);
                waitingUnits.RemoveFirst();

                // �������� �������
                int indexQueue = 0;
                foreach (var waitingUnit in waitingUnits)
                    waitingUnit.GoToNextPointQueue(queuePoints[indexQueue++]);

                // �������� ��������� ����� � �������
                for (int i = 0; i < queuePoints.Count; i++)
                    if (i >= waitingUnits.Count)
                    {
                        occupiedQueuePoint[i] = false;
                        occupiedQueuePointCount--;
                    }
            }
            else
            {
                unit.machineSeat.VacatePlacePoint(); // ����������� ������ ��� ������ �� ��������� � �������
            }
        }
    }
}