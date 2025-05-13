using System;
using UnityEngine;
using UnityEngine.Events;

namespace IdleTycoon
{
    [Serializable]
    public class CustomEvent
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public UnityEvent ThisEvent { get; private set; }
    }
}