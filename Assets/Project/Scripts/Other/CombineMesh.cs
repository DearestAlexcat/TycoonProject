using UnityEngine;

namespace IdleTycoon
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class CombineMesh : MonoBehaviour
    {
        void Start()
        {
            MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[meshFilters.Length - 1];

            int index = 0;
            foreach (MeshFilter mf in meshFilters)
            {
                if (mf == GetComponent<MeshFilter>()) continue;

                CombineInstance ci = new CombineInstance();
                ci.mesh = mf.sharedMesh;
                ci.transform = transform.worldToLocalMatrix * mf.transform.localToWorldMatrix;
                combine[index++] = ci;

                mf.gameObject.SetActive(false);
            }

            Mesh combinedMesh = new Mesh();
            combinedMesh.CombineMeshes(combine, true, true);

            GetComponent<MeshFilter>().sharedMesh = combinedMesh;
            gameObject.SetActive(true);
        }
    }
}
