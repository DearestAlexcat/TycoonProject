using UnityEngine;

namespace IdleTycoon
{
    [CreateAssetMenu]
    public class StaticData : ScriptableObject
    {
        [Header("Level Movement")]
        public float followSpeed = 10f;
        public Vector2 moveLimitsX;
        public Vector2 moveLimitsZ;

        [Header("Camera Zoom")]
        public float zoomSpeed = 20f;
        public float minZoom = 20f;
        public float maxZoom = 60f;

        [Header("Camera Orientation")]
        public Vector3 cameraPosition;
        public Vector3 cameraRotation;

        [Header("Camera Movement Bounds")]
        public Vector2 minBounds = new Vector2(-50, -50);   // min xz
        public Vector2 maxBounds = new Vector2(50, 50);     // max xz

        [Header("ResetData")]
        public bool resetData = false;
    }
}