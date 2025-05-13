using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace IdleTycoon
{
    public class TabButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] int id;
        [SerializeField] TabGroup tabGroup;
        [SerializeField] Image activeTab;
        [SerializeField] TMP_Text activeTabText;

        public Image ActiveTab => activeTab;
        public int ID => id;

        public void Awake()
        {
            tabGroup.Subscribe(this);
        }

        public void SetActiveTab(bool value)
        {
            activeTab.color = value ? tabGroup.activeTabColor : tabGroup.notActiveTabColor;
            activeTabText.color = value ? tabGroup.activeTabTextColor : tabGroup.notActiveTabTextColor;
        }
       
        public void OnPointerClick(PointerEventData eventData)
        {
            Service<AudioManager>.Get()?.PlaySound("Click");
            tabGroup.OnTabSelected(this);
        }
    }
}