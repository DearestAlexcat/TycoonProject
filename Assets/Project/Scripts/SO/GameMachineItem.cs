using System.Collections.Generic;
using UnityEngine;

namespace IdleTycoon
{
    [System.Serializable]
    public class GameMachineItem
    {
        public GameMachineType type;
        [Range(1, 10)] public int level;
    }
}