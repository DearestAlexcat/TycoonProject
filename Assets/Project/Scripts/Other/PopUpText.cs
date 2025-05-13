using System.Collections;
using UnityEngine;
using TMPro;

namespace IdleTycoon
{
    public class PopUpText : MonoBehaviour
    {
        public TMP_Text textUP;

        [SerializeField] float durationUP;
        [SerializeField] float startDelay;
        [SerializeField] float positionUP;
        [SerializeField] AnimationCurve moveCurve;
        [SerializeField] float timeToDestruction;

        IEnumerator Start()
        {
            textUP.enabled = false;

            yield return new WaitForSeconds(startDelay);

            textUP.enabled = true;

            StartCoroutine(MoveAndFadeCoroutine());
        }

        IEnumerator MoveAndFadeCoroutine()
        {
            Vector3 startPos = transform.localPosition;
            Vector3 endPos = startPos + Vector3.up * positionUP;

            float elapsed = 0f;
            Color originalColor = textUP.color;

            while (elapsed < durationUP)
            {
                float t = elapsed / durationUP;

                transform.localPosition = Vector3.Lerp(startPos, endPos, moveCurve.Evaluate(t));

                // ���������
                float curvedT = moveCurve.Evaluate(t);
                if (curvedT >= 0.7f)
                {
                    float fadeProgress = Mathf.InverseLerp(0.7f, 1f, curvedT); // ����������� �� 0.5 �� 1
                    Color c = originalColor;
                    c.a = Mathf.Lerp(1f, 0f, fadeProgress);
                    textUP.color = c;
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = endPos;
            Color finalColor = textUP.color;
            finalColor.a = 0f;
            textUP.color = finalColor;

            yield return new WaitForSeconds(timeToDestruction);
            Destroy(gameObject);
        }
    }
}