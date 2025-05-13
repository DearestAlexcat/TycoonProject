using UnityEngine;
using UnityEngine.EventSystems;

namespace IdleTycoon
{ 
    public class TycoonCameraController : MonoBehaviour
    {
        [SerializeField] StaticData data;

        [Header("Camera AspectRatio Scaler")]
        public Vector2 ReferenceResolution = new Vector2(1920, 1080);
        public Vector3 ZoomFactor = Vector3.one;

        Vector3 targetPosition;
        Vector3 startDrag;

        void Awake()
        {
            transform.position = data.cameraPosition;
            transform.rotation = Quaternion.Euler(data.cameraRotation);
            ApplyAspectRatioOffset();
        }

        void Update()
        {
            DragCamera();
            UpdateCameraPosition();
            HandleTouchZoom();
        }

        void ApplyAspectRatioOffset()
        {
            if (ReferenceResolution.y == 0 || ReferenceResolution.x == 0)
                return;

            var refRatio = ReferenceResolution.x / ReferenceResolution.y;
            var ratio = (float)Screen.width / (float)Screen.height;

            Camera.main.transform.position = transform.position + transform.forward * (1f - refRatio / ratio) * ZoomFactor.z
                                                + transform.right * (1f - refRatio / ratio) * ZoomFactor.x
                                                + transform.up * (1f - refRatio / ratio) * ZoomFactor.y;
        }

        void DragCamera()
        {
            if (EventSystem.current.IsPointerOverGameObject() || EventSystem.current.IsPointerOverGameObject(0))
                return;

            if (!Input.GetMouseButton(0))
                return;

            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (plane.Raycast(ray, out float distance))
            {
                Vector3 hitPoint = ray.GetPoint(distance);

                if (Input.GetMouseButtonDown(0))
                    startDrag = hitPoint;
                else
                {
                    targetPosition += startDrag - hitPoint;
                }
            }
        }

        void UpdateCameraPosition()
        {
            if (targetPosition.sqrMagnitude > float.Epsilon)
            {
                Vector3 newPosition = transform.position + targetPosition * data.followSpeed * Time.deltaTime;

                newPosition.x = Mathf.Clamp(newPosition.x, data.minBounds.x, data.maxBounds.x);
                newPosition.z = Mathf.Clamp(newPosition.z, data.minBounds.y, data.maxBounds.y);

                transform.position = newPosition;        
            }

            targetPosition = Vector3.zero;
        }

        void HandleTouchZoom()
        {
            if (Input.touchCount < 2)
                return;

            // Позиции на плоскости в текущем кадре
            Vector3 pos1 = GetPlanePoint(Input.GetTouch(0).position);
            Vector3 pos2 = GetPlanePoint(Input.GetTouch(1).position);

            // Позиции на плоскости в предыдущем кадре
            Vector3 pos1Prev = GetPlanePoint(Input.GetTouch(0).position - Input.GetTouch(0).deltaPosition);
            Vector3 pos2Prev = GetPlanePoint(Input.GetTouch(1).position - Input.GetTouch(1).deltaPosition);

            // Вычисляем масштаб
            float prevDistance = Vector3.Distance(pos1Prev, pos2Prev);
            float currDistance = Vector3.Distance(pos1, pos2);
            float zoomFactor = currDistance / prevDistance;

            // Игнорируем слишком резкие значения
            if (zoomFactor == 0 || zoomFactor > 10)
                return;
    
            Vector3 midPoint = (pos1 + pos2) / 2f;     // Центр между пальцами
            Vector3 direction = Camera.main.transform.position - midPoint;    // Направление от точки к камере
            Vector3 targetPos = midPoint + direction / zoomFactor; // Новая потенциальная позиция камеры

            // Проверка ограничения по высоте
            float newHeight = targetPos.y;
            if (newHeight >= data.minZoom && newHeight <= data.maxZoom)
            {
                Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetPos, Time.deltaTime * data.zoomSpeed);
            }
        }
    
        Vector3 GetPlanePoint(Vector2 screenPosition) // Проецирует экранную точку на плоскость XZ на уровне Y = 0
        {
            Plane ground = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(screenPosition);

            if (ground.Raycast(ray, out float enter))
            {
                return ray.GetPoint(enter);
            }

            return Vector3.zero;
        }
    }
}