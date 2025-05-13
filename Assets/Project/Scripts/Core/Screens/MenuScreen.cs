using UnityEngine;

namespace IdleTycoon
{
    public class MenuScreen : ScreenBase
    {
        [Space]
        [SerializeField] CustomToggle soundToggle;
        [SerializeField] CustomToggle musicToggle;

        [Space]
        [SerializeField] LanguageSwitcher languageSwitcher;

        void Awake()
        {
            Service<MenuScreen>.Set(this);
        }

        public void ResetProgress()
        {
            Service<AudioManager>.Get().PlaySound("Click");

            MessageScreenYesNo screen = Service<MessageScreenYesNo>.Get();

            screen.InitializeMessage("ResetProgres");
            screen.callbackYes = DoResetProgress;

            screen.OnDisplay(true);

            void DoResetProgress()
            {
                Service<StaticData>.Get().resetData = true;

                ScenesController.LoadSceneAsync(Scenes.Boot.ToString());
            }
        }

        void InitializeUI()
        {
            soundToggle.OnToggleClick = SoundToggle;
            musicToggle.OnToggleClick = MusicToggle;

            languageSwitcher.OnUpdateStorageData = UpdateUISettingsStorageData;
        }

        public void Initialize()
        {
            InitializeUI();

            UISettingsStorageData storageData = GetUISettingsStorageData();

            soundToggle.HardToggleState(storageData.sounds);
            musicToggle.HardToggleState(storageData.music);

            languageSwitcher.SetFlagView(storageData.language);
        }

        void UpdateUISettingsStorageData()
        {
            UISettingsStorageData storageData = new UISettingsStorageData();

            storageData.sounds = soundToggle.IsActive;
            storageData.music = musicToggle.IsActive;
            storageData.language = languageSwitcher.CurrentId;

            SaveLoadManager.Save<UISettingsStorageData>(StorageKeys.UISettings, storageData);
        }

        UISettingsStorageData GetUISettingsStorageData()
        {
            return SaveLoadManager.Load<UISettingsStorageData>(StorageKeys.UISettings);
        }

        void SoundToggle()
        {
            Service<AudioManager>.Get().SetActiveSounds(soundToggle.IsActive);

            UpdateUISettingsStorageData();
        }

        void MusicToggle()
        {
            Service<AudioManager>.Get().SetActiveStartupTrack(musicToggle.IsActive);

            UpdateUISettingsStorageData();
        }
    }
}