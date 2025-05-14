using IdleTycoon;
using System.Collections.Generic;
using UnityEngine;

public interface IUpdatable
{
    bool IsRemovable { get; }
    void UpdateBehaviour();
}

public class UpdateManager : MonoBehaviour
{
    LinkedList<IUpdatable> updatableList = new LinkedList<IUpdatable>();

    bool isPaused;

    void Awake()
    {
        Service<UpdateManager>.Set(this);
    }

    public void Add(IUpdatable behaviour)
    {
        updatableList.AddLast(behaviour);
    }

    public void Remove(IUpdatable behaviour)
    {
        if (updatableList.Contains(behaviour))
        {
            updatableList.Remove(behaviour);
        }
    }

    public void Add(params IUpdatable[] behaviours)
    {
        for (int i = 0; i < behaviours.Length; i++)
        {
            updatableList.AddLast(behaviours[i]);
        }
    }

    public void Clear()
    {
        updatableList.Clear();
    }

    public static void TogglePauseState(bool paused)
    {
        if (Service<UpdateManager>.Get() != null)
        {
            Service<UpdateManager>.Get().isPaused = paused;
        }
    }

    void Update()
    {
        if (isPaused) return;

        var node = updatableList.First;

        while (node != null)
        {
            var next = node.Next;

            if (node == null || node.Value.IsRemovable)
            {
                updatableList.Remove(node);
            }
            else
            {
                node.Value.UpdateBehaviour();
            }

            node = next;
        }
    }
}
