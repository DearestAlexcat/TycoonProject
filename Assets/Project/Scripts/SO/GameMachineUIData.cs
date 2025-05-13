using System.Collections.Generic;
using UnityEngine;

namespace IdleTycoon
{
    [System.Serializable]
    public class GameMachineDisplayData
    {
        public string name;
        public GameMachineType type;
        public Sprite sprite;
        public string description;
        public int purchaseCost;
    }

    [CreateAssetMenu(fileName = "GameMachineUIData", menuName = "Data/GameMachineUIData")]
    public class GameMachineUIData : ScriptableObject
    {
        public List<GameMachineDisplayData> displayDatas;

        public GameMachineDisplayData GetData(GameMachineType type)
        {
            foreach (GameMachineDisplayData item in displayDatas)
                if (item.type == type)
                    return item;
            return null;
        }
    }
}