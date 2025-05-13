using System.Collections.Generic;
using UnityEngine;

namespace IdleTycoon
{
    [System.Serializable]
    public class UnlockRoomDisplayData
    {
        public GameMachineType type;
        public string description;
        public int unlockCost;
    }

    [CreateAssetMenu(fileName = "UnlockRoomData", menuName = "Data/UnlockRoomData")]
    public class UnlockRoomData : ScriptableObject
    {
        public List<UnlockRoomDisplayData> displayDatas;

        public UnlockRoomDisplayData GetData(GameMachineType type)
        {
            foreach (UnlockRoomDisplayData item in displayDatas)
                if (item.type == type)
                    return item;
            return null;
        }
    }
}