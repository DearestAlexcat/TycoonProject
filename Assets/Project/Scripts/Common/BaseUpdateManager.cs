using IdleTycoon;
using UnityEngine;

public abstract class BaseUpdateManager : MonoBehaviour, IUpdatable
{
    bool isRegistered;
    bool isRemoveFromUpdate;

    virtual public bool IsRemovable => gameObject == null || isRemoveFromUpdate;
    abstract public void UpdateBehaviour();

    public void RegisterForUpdate()
    {
        if (!isRegistered)
        {
            isRegistered = true;
            isRemoveFromUpdate = false;
            Service<UpdateManager>.Get().Add(this);
        }
    }

    public void UnregisterFromUpdate(bool isRemoveInstantly = false)
    {
        if (isRegistered)
        {
            isRegistered = false;
            isRemoveFromUpdate = true;
           
            if (isRemoveInstantly)
            {
                Service<UpdateManager>.Get().Remove(this);
            }
        }
    }

   
}
