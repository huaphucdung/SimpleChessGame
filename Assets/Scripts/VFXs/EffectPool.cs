using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;
using UnityEngine.Pool;

public class EffectPool<T> where T : EffectEvent
{
    private IObjectPool<T> pool;
    private T prefab;
    private Transform parentTransform;
    public EffectPool(T prefab, Transform parentTransform)
    {
        this.prefab = prefab;
        this.parentTransform = parentTransform;

        pool = new ObjectPool<T>(CreateEffect, OnGetEffect, OnReturnEffect, OnDestroyEffect);
    }

    private T CreateEffect()
    {
        T newEffect = GameObject.Instantiate(prefab, parentTransform);
        return newEffect;
    }

    private void OnGetEffect(T effect)
    {
        effect.gameObject.SetActive(true);
    }

    private void OnReturnEffect(T effect)
    {
        effect.gameObject.SetActive(false);
    }

    private void OnDestroyEffect(T effect)
    {
        GameObject.Destroy(effect.gameObject);
    }

    public T GetEffect(Vector3 position, Quaternion rotation)
    {
        T effect = pool.Get();
        effect.transform.position = position;
        effect.transform.rotation = rotation;
        return effect;
    }

    public void ReleaveEffect(T effect)
    {
        pool.Release(effect);
    }
}
