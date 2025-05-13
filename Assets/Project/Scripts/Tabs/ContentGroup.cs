using System.Collections.Generic;
using UnityEngine;

namespace IdleTycoon
{
    public class ContentGroup : MonoBehaviour
    {
        [SerializeField] List<TabContent> contents;

        List<TabContent> enabledTab = new List<TabContent>();

        public void OnContentEnable(int id)
        {
            // Hide content of the previous tab
            SetActiveEnabledTab(false);
            enabledTab.Clear();

            foreach (var item in contents)
            {
                // One content can be displayed simultaneously with another.
                foreach (var tabId in item.tabIds)
                {
                    if (tabId == id)
                    {
                        enabledTab.Add(item);
                    }
                }
            }

            // Display contents of current tab
            SetActiveEnabledTab(true);
        }

        void SetActiveEnabledTab(bool value)
        {
            foreach(var item in enabledTab)
            {
                item.Display(value);
            }
        }

        //public void Subscribe(TabContent content)
        //{
        //    if (contents == null)
        //        contents = new List<TabContent>();
        //    contents.Add(content);
        //}
    }
}