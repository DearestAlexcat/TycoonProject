using UnityEngine;

namespace IdleTycoon 
{ 
    public class CameraBoundsVisualizer : MonoBehaviour
    {
        [Header("Movement Bounds")]
        [SerializeField] Vector2 minBounds = new Vector2(-50, -50); // Минимальные значения по X и Z
        [SerializeField] Vector2 maxBounds = new Vector2(50, 50);   // Максимальные значения по X и Z

        [SerializeField] StaticData data;

        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;

            data.minBounds = minBounds;
            data.maxBounds = maxBounds;

            Vector3 center = new Vector3(
                (minBounds.x + maxBounds.x) / 2f,
                transform.position.y,
                (minBounds.y + maxBounds.y) / 2f
            );

            Vector3 size = new Vector3(Mathf.Abs(maxBounds.x - minBounds.x), 0.1f, Mathf.Abs(maxBounds.y - minBounds.y));

            Gizmos.DrawWireCube(center, size);
        }
    }
}
