using UnityEngine;
using UnityEngine.EventSystems;

namespace IdleTycoon
{
    public class RoomClickHandler : MonoBehaviour
    {
        [SerializeField] CanvasGroup inventoryScreen;
        [SerializeField] CanvasGroup unlockRoomScreen;

        [Space]
        [SerializeField] float duration = 0.1f;

        float time;

        void Update()
        {
            time += Time.deltaTime;

            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject() || EventSystem.current.IsPointerOverGameObject(0))
                {
                    return;
                }

                time = 0;
            }

            if (Input.GetMouseButtonUp(0) && time < duration)
            {
                if (EventSystem.current.IsPointerOverGameObject() || EventSystem.current.IsPointerOverGameObject(0))
                {
                    return;
                }

                Service<AudioManager>.Get().PlaySound("Click");

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider.TryGetComponent(out Room room))
                    {
                        inventoryScreen.alpha = 1f;
                        inventoryScreen.blocksRaycasts = true;
                    }
                    else if (hit.collider.TryGetComponent(out RoomStateSwitcher stateSwitcher))
                    {
                        unlockRoomScreen.alpha = 1f;
                        unlockRoomScreen.blocksRaycasts = true;
                        stateSwitcher.InitUnlockText();
                    }
                }
            }
        }
    }
}