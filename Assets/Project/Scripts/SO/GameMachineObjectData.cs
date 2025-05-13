using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

namespace IdleTycoon
{
    [System.Serializable]
    public class ColorOffsetData
    {
        public GameMachineType type;
        public List<Vector2> colorOffset;
    }

    [CreateAssetMenu(fileName = "GameMachineObjectPrefabs", menuName = "Data/GameMachineObjectPrefabs")]
    public class GameMachineObjectData : ScriptableObject
    {
        public List<BaseMachineObject> objectDatas;

        public BaseMachineObject GetMachinePrefab(GameMachineType type) // level start 0
        {
            foreach (BaseMachineObject item in objectDatas)
                if (item.type == type)
                    return item;

            return null;
        }
    }
}