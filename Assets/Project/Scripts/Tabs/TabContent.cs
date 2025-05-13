using System.Collections.Generic;
using UnityEngine;

namespace IdleTycoon
{
    [RequireComponent(typeof(CanvasGroup))]
    public class TabContent : MonoBehaviour
    {
        public List<int> tabIds;
        CanvasGroup canvasGroup;

        public void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            Display(false);
        }

        public void Display(bool value)
        {
            canvasGroup.alpha = value ? 1 : 0;
            canvasGroup.blocksRaycasts = value;
        }
    }
}