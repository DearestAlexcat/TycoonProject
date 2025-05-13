using UnityEngine;

namespace IdleTycoon
{
    public class BaseMachineObject : MonoBehaviour
    {
        public GameMachineType type;
        public MeshRenderer mr;

        public void Initialize(Vector2 colorOffset)
        {
            mr.material.SetTextureOffset("_BaseMap", colorOffset);
        }
    }
}
