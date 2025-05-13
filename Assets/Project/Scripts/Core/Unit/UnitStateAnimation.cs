using System.Collections.Generic;
using UnityEngine;

namespace IdleTycoon
{
    public enum AnimationFlags
    {
        BreathingIdle = 1,
        Walking = 2,
    }

    public class UnitStateAnimation : MonoBehaviour
    {
        [SerializeField] protected Animator animator;

        //private void Awake()
        //{
        //    Debug.Log("Walking: " + Animator.StringToHash("Walking"));
        //    Debug.Log("BreathingIdle: " + Animator.StringToHash("BreathingIdle"));
        //}

        Dictionary<AnimationFlags, int> animationState = new()
        {
            { AnimationFlags.Walking, 1744665739 },
            { AnimationFlags.BreathingIdle, -240973668 }
        };

        int tempHash = -1;

        public void PlayAnimation(System.ValueType flag)
        {
            AnimationFlags value = (AnimationFlags)flag;

            if (animationState.ContainsKey(value) && tempHash == animationState[value]) return;
            animator.ResetTrigger(tempHash);

            tempHash = animationState[value];
            animator.SetTrigger(tempHash);
        }
    }
}