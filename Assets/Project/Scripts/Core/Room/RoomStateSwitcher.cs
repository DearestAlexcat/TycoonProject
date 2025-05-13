using TMPro;
using UnityEngine;
using UnityEngine.Localization;

namespace IdleTycoon
{
    public class RoomStateSwitcher : MonoBehaviour
    {
        [SerializeField] LocalizedString localizedNameAsset;

        [Space]
        [SerializeField] Room room;

        [Header("Open State")]
        [SerializeField] GameObject openRoomState;

        [Header("Close State")]
        [SerializeField] UnlockRoomData unlockRoomData;
        [SerializeField] GameObject closeRoomState;
        [SerializeField] GameObject allowRoomPurchaseObject;

        [Space]
        [SerializeField] TMP_Text opening—ostText;

        public void OnEnable()
        {
            if (localizedNameAsset != null)
            {
                localizedNameAsset.StringChanged -= UpdateNameText;
            }

            localizedNameAsset = new LocalizedString
            {
                TableReference = "UnlockRoomData",
                TableEntryReference = "OpenRoomFor"
            };

            localizedNameAsset.Arguments = new object[] { unlockRoomData.GetData(room.type).unlockCost };
            localizedNameAsset.StringChanged += UpdateNameText;
        }

        private void OnDisable()
        {
            if (localizedNameAsset != null)
            {
                localizedNameAsset.StringChanged -= UpdateNameText;
            }
        }

        public void AllowRoomPurchase(bool value)
        {
            allowRoomPurchaseObject.SetActive(value);
        }

        public void SetState(RoomState roomState)
        {
            switch (roomState)
            {
                case RoomState.None: break;
                case RoomState.Open:
                    InitOpenState();
                    break;
                case RoomState.Close:
                    InitCloseState();
                    break;
            }
        }

        public void InitUnlockText()
        {
            var data = unlockRoomData.GetData(room.type);

            Service<UnlockRoomScreen>.Get().InitUnlockScreen(data.description, data.unlockCost, OnInitOpenState);
        }

        void InitOpenState()
        {
            openRoomState.SetActive(true);
            closeRoomState.SetActive(false);
            room.IsOpen = true;
        }


        void OnInitOpenState() // Event Button. ¬ËÒËÚ Ì‡ ÍÌÓÔÍÂ ‚ ÓÍÌÂ ÔÓÍÛÔÍË ÍÓÏÌ‡Ú˚
        {
            openRoomState.SetActive(true);
            closeRoomState.SetActive(false);
            room.IsOpen = true;

            Service<RoomUpgradeService>.Get().IncrementUnlockedRoom();
        }

        private void UpdateNameText(string value)
        {
            opening—ostText.text = value;
        }

        void InitCloseState()
        {
            openRoomState.SetActive(false);
            closeRoomState.SetActive(true);
            room.IsOpen = false;
        }
    }
}