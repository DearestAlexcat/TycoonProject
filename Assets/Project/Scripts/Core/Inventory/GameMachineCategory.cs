using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace IdleTycoon
{
    public class GameMachineCategory : MonoBehaviour
    {
        [HideInInspector] public GameMachineType type;
        public Image icon;
        public TMP_Text countText;

        public System.Action<GameMachineType> callback;

        [Space]
        [SerializeField] Image frame;
        [SerializeField] Color activeFrame;
        [SerializeField] Color notActiveFrame;

        [Space]
        [SerializeField] Button selectGroupButton;
        [SerializeField] GameObject ñategoryBlocker;

        int count;
        public int Count
        {
            get
            {
                return count;
            }
            set
            {
                count = value;
                countText.text = value.ToString();
            }
        }

        public void SetActiveFrame(bool value)
        {
            frame.color = value ? activeFrame : notActiveFrame;
        }

        public void SelectGroup() // Button Event
        {
            callback?.Invoke(type);
            Service<AudioManager>.Get().PlaySound("Click");
        }

        public void SetActiveMachineCategory(bool value)
        {
            selectGroupButton.enabled = value;
            ñategoryBlocker.SetActive(!value);
        }
    }
}