using UnityEngine;

namespace IdleTycoon
{
    public class ScreenBase : MonoBehaviour
    {
        [SerializeField] CanvasGroup cg;

        public void OnDisplay(bool value)
        {
            Service<AudioManager>.Get().PlaySound("Click");

            cg.alpha = value ? 1f : 0f;
            cg.blocksRaycasts = value;
        }
    }
}
