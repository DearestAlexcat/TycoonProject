using System.Collections.Generic;
using UnityEngine;


namespace IdleTycoon
{
    public class TabGroup : MonoBehaviour
    {
        [SerializeField] ContentGroup contentGroup;
        [SerializeField] int defaultActiveTabId = 0;

        [Space]
        public Color activeTabTextColor;
        public Color activeTabColor;

        [Space]
        public Color notActiveTabTextColor;
        public Color notActiveTabColor;

        List<TabButton> buttons;
        TabButton selectedTab;

        public void Start()
        {
            foreach (var item in buttons)
                item.SetActiveTab(false);

            OnTabSelected(defaultActiveTabId);
        }

        public void Subscribe(TabButton button)
        {
            if (buttons == null)
                buttons = new List<TabButton>();

            buttons.Add(button);
        }

        public void OnTabSelected(int id)
        {
            foreach (var item in buttons)
            {
                if (item == selectedTab) return;

                if (item.ID == id)
                {
                    if (selectedTab != null)
                    {
                        selectedTab.SetActiveTab(false);
                    }

                    selectedTab = item;
                    selectedTab.SetActiveTab(true);

                    break;
                }
            }

            contentGroup.OnContentEnable(selectedTab.ID);
        }

        public void OnTabSelected(TabButton button)
        {
            if (button == selectedTab)
                return;
            
            if (selectedTab != null) // Прошлую кнопку делаем Deselect
            {
                selectedTab.SetActiveTab(false);
            }

            // Следующая кнопка Select
            selectedTab = button;
            selectedTab.SetActiveTab(true);

            contentGroup.OnContentEnable(selectedTab.ID);
        }
    }
}