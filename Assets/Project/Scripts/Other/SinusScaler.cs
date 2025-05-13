using UnityEngine;

namespace IdleTycoon
{
    public class SinusScaler : MonoBehaviour
    {
        [SerializeField] float scaleAmplitude = 0.2f;       // Насколько увеличивается/уменьшается масштаб
        [SerializeField] float scaleFrequency = 1f;         // Частота колебаний
        [SerializeField] Vector3 scaleAxis = Vector3.one;   // Ось масштабирования

        [Space]
        [SerializeField] MeshRenderer mr;


        Vector3 originalScale;
        Color originalColor;

        void Start()
        {
            originalScale = transform.localScale;
            originalColor = mr.material.color;
        }

        void Update()
        {
            float scaleOffset = Mathf.Sin(Time.time * scaleFrequency) * scaleAmplitude;
            transform.localScale = originalScale + scaleAxis * scaleOffset;

            Color color = mr.material.color;
            color.a = originalColor.a + scaleOffset;
            mr.material.color = color;
        }
    }
}
