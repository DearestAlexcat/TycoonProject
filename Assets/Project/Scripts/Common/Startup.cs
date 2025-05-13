using UnityEngine;

namespace IdleTycoon
{
    public class Startup : MonoBehaviour
    {
        void Start()
        {
            Service<AudioManager>.Get().InitSounds();
            Service<MenuScreen>.Get().Initialize();

            Service<MoneyController>.Get().Initialize();

            Service<RoomUpgradeService>.Get().Initialize();

            Service<PurchaseGameMachineController>.Get().Initialize();
            Service<GameMachineCategoryController>.Get().Initialize();

            Service<UnitSpawner>.Get().Initialize();
        }
    }
}
