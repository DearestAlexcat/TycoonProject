using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace IdleTycoon
{
    public class GameMachineDisplaySlot : MonoBehaviour
    {
        [SerializeField] int level;

        [SerializeField] Image frame;
        [SerializeField] Color activeFrame;
        [SerializeField] Color notActiveFrame;

        [SerializeField] Image icon;
        [SerializeField] TMP_Text count;
        [SerializeField] TMP_Text levelNumber;

        public System.Action<int> callback;

        public int Level => level;

        private void Start()
        {
            levelNumber.text = $"lvl. {level}";
        }

        public void SetActiveFrame(bool value)
        {
            frame.color = value ? activeFrame : notActiveFrame;
        }

        public void SetView(Sprite sprite)
        {
            icon.sprite = sprite;
        }

        public void SetCount(int number)
        {
            //bool value = number != 0;
            //count.gameObject.SetActive(value);
            count.text = number.ToString();
            //count.enabled = value;
        }

        public void OnSlotSelected() // Button Event
        {
            Service<AudioManager>.Get().PlaySound("Click");
            callback?.Invoke(level);
        }
    }
}
