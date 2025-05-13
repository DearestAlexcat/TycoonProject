using System.Collections;
using UnityEngine;

namespace IdleTycoon
{
    public class FenceDoor : MonoBehaviour
    {
        [SerializeField] Transform doorLeft, doorRight;
        [SerializeField] float rotationDuration = 1f; // Время открытия/закрытия
        [SerializeField] float rotationAngle;

        int counter = 0;
        bool isOpen = false;
        Coroutine doorCoroutine;

        void OnTriggerEnter(Collider other)
        {
            if (other.isTrigger) return;

            counter++;
            if (!isOpen)
            {
                isOpen = true;
                StartDoorCoroutine(true);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.isTrigger) return;

            counter = Mathf.Max(0, counter - 1);
            if (counter == 0 && isOpen)
            {
                isOpen = false;
                StartDoorCoroutine(false);
            }
        }

        void StartDoorCoroutine(bool open)
        {
            if (doorCoroutine != null)
                StopCoroutine(doorCoroutine);

            doorCoroutine = StartCoroutine(RotateDoors(open));
        }

        IEnumerator RotateDoors(bool open)
        {
            Quaternion leftStart = doorLeft.localRotation;
            Quaternion rightStart = doorRight.localRotation;

            Quaternion leftTarget = Quaternion.Euler(0f, open ? -rotationAngle : 0f, 0f);
            Quaternion rightTarget = Quaternion.Euler(0f, open ? rotationAngle : 0f, 0f);

            float elapsed = 0f;

            while (elapsed < rotationDuration)
            {
                float t = elapsed / rotationDuration;
                doorLeft.localRotation = Quaternion.Slerp(leftStart, leftTarget, t);
                doorRight.localRotation = Quaternion.Slerp(rightStart, rightTarget, t);

                elapsed += Time.deltaTime;
                yield return null;
            }

            doorLeft.localRotation = leftTarget;
            doorRight.localRotation = rightTarget;
            doorCoroutine = null;
        }
    }
}