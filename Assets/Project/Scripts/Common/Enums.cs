namespace IdleTycoon
{
    public enum GameMachineType
    {
        None = 0,
        ArcadeMachine = 1,
        BasketballGame = 2,
        DanceMachine = 3,
        AirHockey = 4,
        ClawMachine = 5,
        GamblingMachine = 6,
        Pinball = 7,
        ComingSoon = 8
    }

    public enum EmojiType
    {
        None = 0,
        Cool = 1,
        TearyEyes = 2,
    }

    public enum UnitGoal
    {
        None = 0,
        ReachEnterPoint = 1,
        ReachExitPoint = 2,
        ReachQueuePoint = 3,
        ReachMahinePoint = 4,
        ReachExitRoom = 5
    }

    public enum StorageKeys
    {
        None = 0,
        GameMachines = 1,
        Money = 2,
        Rooms = 3,
        UISettings = 4
    }

    public enum RoomState
    {
        None = 0,
        Open = 1,
        Close = 2
    }

    public enum Scenes
    {
        None = 0,
        Boot = 1,
        Level = 2
    }
}