using System.Collections.Generic;
using UnityEngine;

namespace IdleTycoon
{
    public class SingleRoomChooser : MonoBehaviour
    {
        [SerializeField] Room exitRooms;
        [SerializeField] List<Room> rooms;

        public Room GetAvailableRoom()
        {
            if (rooms.Count == 1)
            {
                return rooms[0];
            }

            return exitRooms;
        }
    }
}
