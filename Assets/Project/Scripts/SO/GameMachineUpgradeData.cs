using System.Collections.Generic;
using UnityEngine;

namespace IdleTycoon
{
    [System.Serializable]
    public class UpgradeDataByMachineLevel
    {
        public GameMachineType type;
        public List<Vector2> colorOffset;
    }

    [CreateAssetMenu(fileName = "GameMachineObjectPrefabs", menuName = "Data/GameMachineObjectPrefabs")]
    public class GameMachineUpgradeData : ScriptableObject
    {
        public List<BaseMachineObject> objectDatas;
        public List<UpgradeDataByMachineLevel> colorOffsetDatas;

        public BaseMachineObject GetMachinePrefab(GameMachineType type)
        {
            foreach (BaseMachineObject item in objectDatas)
                if (item.type == type)
                    return item;

            return null;
        }

        public UpgradeDataByMachineLevel GetUpgradeDataByMachineType(GameMachineType type) // level start 0
        {
            foreach (var item in colorOffsetDatas)
                if (item.type == type)
                    return item;

            return null;
        }
    }
}