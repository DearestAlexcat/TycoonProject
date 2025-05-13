using System.Collections;
using UnityEngine;

namespace IdleTycoon
{
    class Boot : MonoBehaviour
    {
        [SerializeField] StaticData staticData;

        IEnumerator Start()
        {
            Service<StaticData>.Set(staticData);
       
            Service<SaveDataVerifier>.Get().VerifyData(Service<StaticData>.Get().resetData);

            Service<StaticData>.Get().resetData = false;
            yield return null;

            ScenesController.LoadSceneAsync(Scenes.Level.ToString());
        }
    }
}