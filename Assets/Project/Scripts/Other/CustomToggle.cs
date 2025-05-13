using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace IdleTycoon
{
    public class CustomToggle : MonoBehaviour
    {
        [SerializeField] RectTransform handleRect;

        [Space]
        [SerializeField] Image handleBack;
        [SerializeField] Color handleBackOn;
        [SerializeField] Color handleBackOff;

        [Space]
        [SerializeField] float toggleDuration;
        [SerializeField] Vector2 bordersX;

        bool toggleState;

        Coroutine currentCoroutine;

        public Action OnToggleClick;

        public bool IsActive => toggleState;

        //void Start()
        //{
        //    HardToggleState(false);
        //}

        public void HardToggleState(bool value)     // Включает указанное состояние без анимаций
        {
            toggleState = value;

            ToggleEffect(1f); // Брать из сейвов

            Vector3 temp = handleRect.anchoredPosition;
            temp.x = value ? bordersX.y : bordersX.x;
            handleRect.anchoredPosition = temp;

            OnToggleClick?.Invoke();
        }

        public void ToggleState(bool value) // Включает указанное состояние
        {
            if (toggleState == value) return;

            toggleState = value;

            OnToggleClick?.Invoke();
            CancelSetState();
            currentCoroutine = StartCoroutine(SetStateCoroutine());
        }

        // Переключатель состояния
        public void OnToggleState() // Button Event
        {
            Service<AudioManager>.Get().PlaySound("Click");

            toggleState = !toggleState;

            OnToggleClick?.Invoke();
            CancelSetState();
            currentCoroutine = StartCoroutine(SetStateCoroutine());
        }

        public void CancelSetState()
        {
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
                currentCoroutine = null;
            }
        }

        void ToggleEffect(float t)
        {
            handleBack.color = Color.Lerp(handleBackOff, handleBackOn, EaseOutCubic(toggleState ? t : 1f - t));
        }

        float EaseOutCubic(float t)
        {
            return 1f - Mathf.Pow(1f - t, 3f);
        }

        float EaseInOutSine(float t)
        {
            return -(Mathf.Cos(Mathf.PI * t) - 1f) / 2f;
        }

        IEnumerator SetStateCoroutine()
        {
            float t = 0f;
            float start = toggleState ? bordersX.x : bordersX.y;
            float end = toggleState ? bordersX.y : bordersX.x;
            Vector2 temp;

            while (t < 1f)
            {
                t += Time.unscaledDeltaTime / toggleDuration;

                temp = handleRect.anchoredPosition;
                temp.x = Mathf.Lerp(start, end, EaseInOutSine(t));
                handleRect.anchoredPosition = temp;

                ToggleEffect(t);

                yield return null;
            }

            currentCoroutine = null;
        }
    }
}