using UnityEngine;
using UnityEngine.UI;

namespace IdleTycoon
{
    [RequireComponent(typeof(GridLayoutGroup))]
    public class ResponsiveGrid : MonoBehaviour
    {
        private float aspectRatio = 1f;  // ����������� ������

        private GridLayoutGroup grid;
        private RectTransform rectTransform;
        [SerializeField] CanvasScaler canvasScaler;
        [SerializeField]
        [Range(0f, 1f)] float match = 0f;
        private float previousWidth = 0f;
        private float previousHeight = 0f;

        void Start()
        {
            grid = GetComponent<GridLayoutGroup>();
            rectTransform = GetComponent<RectTransform>();

            // ��������� aspectRatio, ��������� ������� �����
            aspectRatio = grid.cellSize.x / grid.cellSize.y;

            // ��������� ������ ����� ����� ��� ������
            UpdateCellSize();
        }

        void Update()
        {
            UpdateCellSize();
        }

        void UpdateCellSize()
        {
            // �������� ���������� ������� �� GridLayoutGroup
            int columns = grid.constraint == GridLayoutGroup.Constraint.FixedColumnCount
                ? grid.constraintCount
                : 1; // ���� constraint = Flexible, ����� ��������� 1 ��� ��������� ������ �����������

            // �������� ������� � ������� �� GridLayoutGroup
            float totalHorizontalSpacing = grid.spacing.x * (columns - 1) + grid.padding.left + grid.padding.right;
            float scale = GetScale(Screen.width, Screen.height, canvasScaler.referenceResolution, match);
            Debug.Log(scale);
            float parentWidth = rectTransform.rect.width;

            float cellWidth = (parentWidth - totalHorizontalSpacing) / columns;
            float cellHeight = cellWidth / aspectRatio;

            grid.cellSize = new Vector2(cellWidth * scale, cellHeight * scale);
        }

        void UpdateCellSize1()
        {
            // �������� ���������� ������� �� GridLayoutGroup
            int columns = grid.constraint == GridLayoutGroup.Constraint.FixedColumnCount
                ? grid.constraintCount
                : 1; // ���� constraint = Flexible, ����� ��������� 1 ��� ��������� ������ �����������

            // �������� ������� � ������� �� GridLayoutGroup
            float totalHorizontalSpacing = grid.spacing.x * (columns - 1) + grid.padding.left + grid.padding.right;
            float totalVerticalSpacing = grid.spacing.y * (transform.childCount / columns) + grid.padding.top + grid.padding.bottom;

            // �������� ������� ������������� ���������� (������������ RectTransform)
            float parentWidth = rectTransform.rect.width;
            float parentHeight = rectTransform.rect.height;

            // ���� CanvasScaler ������������, ��������� ��� scaleFactor
            //if (canvasScaler != null)
            {
                float scale = GetScale(Screen.width, Screen.height, canvasScaler.referenceResolution, match);
                Debug.Log(canvasScaler.scaleFactor + " " + scale);
                //parentWidth *= scale;
                //parentHeight *= scale;
            }

            // ������������ ��������� ������������ ��� �����
            float availableWidth = parentWidth - totalHorizontalSpacing;
            float availableHeight = parentHeight - totalVerticalSpacing;

            // ������ �����: �������� ����������� ��������, ����� �������� ������ �� ������� ����������
            float cellWidth = availableWidth / columns;
            float cellHeight = cellWidth / aspectRatio;

            // ��������� ������ �����
            if (cellHeight > availableHeight)
            {
                // ���� ������������ ������ ������ ��������� ������, ���������� ������ ��� ���������� ������
                cellHeight = availableHeight;
                cellWidth = cellHeight * aspectRatio;
            }

            // ���������, ���� ������ ����� ������� ������ ��� ���������� ������������
            if (cellWidth > availableWidth)
            {
                // ���� ������������ ������ ������ ��������� ������, ���������� ������ ��� ���������� ������
                cellWidth = availableWidth;
                cellHeight = cellWidth / aspectRatio;
            }

            grid.cellSize = new Vector2(cellWidth, cellHeight);
        }

        void InitializeCellSize()
        {
            float ScreenWidth = Screen.width / GetScale(Screen.width, Screen.height, new Vector2(1920f, 1080f), 0f);

            grid.cellSize = new Vector2(ScreenWidth / 5f, ScreenWidth / 2f);
        }

        float GetScale(int width, int height, Vector2 scalerReferenceResolution, float scalerMatchWidthOrHeight)
        {
            Vector2 vector2 = new Vector2((float)Screen.width, (float)Screen.height);
            float scaleFactor = Mathf.Pow
            (2f,
                Mathf.Lerp(
                            Mathf.Log(vector2.x / scalerReferenceResolution.x, 2f),
                            Mathf.Log(vector2.y / scalerReferenceResolution.y, 2f), scalerMatchWidthOrHeight
                        )
             );

            return scaleFactor;
        }

    }
}