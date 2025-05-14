using IdleTycoon;
using UnityEngine;

public class MoneyExplosionCallback : MonoBehaviour, IPoolObject<MoneyExplosionCallback>
{
    public Pooler<MoneyExplosionCallback> Pooler { get; set; }

    [SerializeField] ParticleSystem ps;

    public void Play()
    {
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ps.Play(true);
    }

    public void OnParticleSystemStopped()
    {
        Pooler.Free(this);
    }
}
