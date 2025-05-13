using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleTycoon
{
    public class UnitSpawner : MonoBehaviour
    {
        public Room exitRoom;                       // Выход из здания
        public List<Room> rooms;       // Посещаемые комнаты

        [Space]
        public Unit unitPrefab;
        public Transform spawnPoint;

        List<Unit> spawnedUnits;

        [Range(0f, 1f)]
        public float fillPercent = 0.7f;

        Pooler<Unit> unitPool;

        void Awake()
        {
            Service<UnitSpawner>.Set(this);
        }

        public void Initialize()
        {
            unitPool = new Pooler<Unit>(unitPrefab, 20);
            unitPool.FreeCallback = DecreaseUnitCount;

            spawnedUnits = new List<Unit>();
            StartCoroutine(UpdatePopulation());
        }

        void DecreaseUnitCount(Unit unit)
        {
            spawnedUnits.Remove(unit);
        }

        int GetAllRoomCountQueueSpots()
        {
            int total = 0;
            foreach (var room in rooms)
                if (room.IsRoomOpenToVisitors())
                    total += room.queuePoints.Count;
            return total;
        }

        int GetAllRoomCountOccupiedQueueSpots()
        {
            int total = 0;
            foreach (var room in rooms)
                if (room.IsRoomOpenToVisitors())
                    total += room.occupiedQueuePointCount;
            return total;
        }

        int GetAllRoomCountMachinesSpots()
        {
            int total = 0;
            foreach (var room in rooms)
                if (room.IsRoomOpenToVisitors())
                    total += room.gmPlacement.Count;
            return total;
        }

        int GetAllRoomCountOccupiedMachinesSpots()
        {
            int total = 0;
            foreach (var room in rooms)
                if (room.IsRoomOpenToVisitors())
                    total += room.GetCountGameMachinePlacesOccupied();
            return total;
        }

        IEnumerator UpdatePopulation()
        {
            while (true)
            {
                // Общее кол-во мест должно быть заполнено на 70%
                int desiredQueueFilled = Mathf.RoundToInt(GetAllRoomCountQueueSpots() * fillPercent);
                int desiredMachinesFilled = Mathf.RoundToInt(GetAllRoomCountMachinesSpots() * fillPercent);

                int currentMachinesFilled = GetAllRoomCountOccupiedMachinesSpots();
                int currentQueueFilled = GetAllRoomCountOccupiedQueueSpots();

                int needToSpawn = (desiredMachinesFilled - currentMachinesFilled) +
                                  (desiredQueueFilled - currentQueueFilled);

                for (int i = 0; i < needToSpawn; i++)
                {
                    SpawnRandomUnit();
                    yield return new WaitForSeconds(Random.Range(1f, 2f));
                }

                yield return new WaitForSeconds(2f);
            }
        }

        private void SpawnRandomUnit()
        {
            if (unitPrefab == null) return;

            Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : transform.position;

            Unit unit = unitPool.Get(spawnPos, Quaternion.identity);
            unit.gameObject.name = "Male";

            spawnedUnits.Add(unit);

            unit.exitRoom = exitRoom;

            Room room;

            for (int i = 0; i < rooms.Count; i++)
            {
                if (unit.rooms.Count == 0)
                {
                    room = GetRandomRoom(unit);
                    if (room != null)
                        unit.rooms.Add(room);
                }
                else
                {
                    if (UnityEngine.Random.value < 0.357f)
                    {
                        room = GetRandomRoom(unit);
                        if (room != null)
                            unit.rooms.Add(room);
                    }
                }
            }

            unit.GoToRoom();
        }

        Room GetRandomRoom(Unit unit)
        {
            List<Room> temp = new List<Room>();

            foreach (var room in rooms)
            {
                if (room.IsRoomOpenToVisitors())
                {
                    if (!unit.rooms.Contains(room))
                        temp.Add(room);
                }
            }

            if (temp.Count == 0)
                return null;

            return temp[Random.Range(0, temp.Count)];
        }
    }
}