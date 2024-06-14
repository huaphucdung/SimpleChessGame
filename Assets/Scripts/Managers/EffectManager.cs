using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [SerializedDictionary("Key", "Effect Prefab")]
    [SerializeField] private SerializedDictionary<string, EffectEvent> effectDictionary;
    [SerializeField] private Transform effectParent;
    private static Dictionary<string, EffectPool<EffectEvent>> effectPoolDictionary;
    public void Initialize()
    {
        effectPoolDictionary = new Dictionary<string, EffectPool<EffectEvent>>();
        foreach (KeyValuePair<string, EffectEvent> data in effectDictionary)
        {
            effectPoolDictionary.Add(data.Key, new EffectPool<EffectEvent>(data.Value, effectParent));
        }
    }

    public static void PlayerEffect(string key, Vector3 position, Quaternion rotation)
    {
        if (!effectPoolDictionary.ContainsKey(key)) return;
        EffectPool<EffectEvent> pool = effectPoolDictionary[key];
        EffectEvent effect = pool.GetEffect(position, rotation);
        effect.PlayEffect(pool.ReleaveEffect);
    }
}
