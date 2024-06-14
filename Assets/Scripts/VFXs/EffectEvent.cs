using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectEvent : MonoBehaviour
{
    private event Action<EffectEvent> releaseAction;

    private ParticleSystem vfx;

    private void Awake()
    {
        vfx = GetComponent<ParticleSystem>();
    }

    public void PlayEffect(Action<EffectEvent> action)
    {
        releaseAction = action;
        vfx.Play();
    }

    private void OnParticleSystemStopped()
    {
        releaseAction?.Invoke(this);
    }
}
