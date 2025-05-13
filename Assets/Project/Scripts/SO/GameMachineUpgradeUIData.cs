using System.Collections.Generic;
using UnityEngine;

namespace IdleTycoon
{
    [System.Serializable]
    public class GameMachineUpgradeDisplayData
    {
        public string name;
        public Sprite sprite;
        public int upgradeCost;
        public int sellCost;
        public int incomeBonusPercent;
    }

    [CreateAssetMenu(fileName = "GameMachineUpgradeUIData", menuName = "Data/GameMachineUpgradeUIData")]
    public class GameMachineUpgradeUIData : ScriptableObject
    {
        public GameMachineType type;
        public int baseIncome;
        public List<GameMachineUpgradeDisplayData> upgradeDatas;

        public int GetIncomeByLevel(int machineLevel)
        {
            if (machineLevel < 0 || machineLevel > upgradeDatas.Count)
            {
                Debug.LogError($"Invalid index {nameof(machineLevel)}");
                return baseIncome;
            }

            return Mathf.RoundToInt(baseIncome + baseIncome * upgradeDatas[machineLevel].incomeBonusPercent / 100f);
        }
    }
}
