using System.Collections;
using UnityEngine;

namespace IdleTycoon
{
    public class PopUpMessage : MonoBehaviour
    {
        [SerializeField] CanvasGroup cg;
        [SerializeField] float lifeTime = 1f;

        Coroutine currentCoroutine;

        void Awake()
        {
            Service<PopUpMessage>.Set(this);
            SetActive(false);
        }

        public void Display()
        {
            SetActive(true);

            if (currentCoroutine != null)
                StopCoroutine(currentCoroutine);
            currentCoroutine = StartCoroutine(AutoDisable());
        }

        public void SetActive(bool value)
        {
            cg.alpha = value ? 1f : 0f;
            cg.blocksRaycasts = value;
        }

        private IEnumerator AutoDisable()
        {
            yield return new WaitForSeconds(lifeTime);
            SetActive(false);
            currentCoroutine = null;
        }
    }
}
