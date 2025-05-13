using System;
using System.Collections.Generic;

namespace IdleTycoon
{
    public class ItemsStorageData<T>
    {
        public List<T> items = new List<T>();
    }

    [Serializable]
    public class MoneyStorageData
    {
        public int money;
    }

    [Serializable]
    public class RoomsStorageData
    {
        public int lockedRoomIndex;
    }

    [Serializable]
    public class UISettingsStorageData
    {
        public int language;
        public bool sounds;
        public bool music;
    }

    public class GameMachinesStorageData : ItemsStorageData<GameMachineSlotInfo> { }

    // -----------------------------------------------------------------------------------------

    public class SaveDataListStorageData
    {
        public int id;
        public string description;
    }
}

