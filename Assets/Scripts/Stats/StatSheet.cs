using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatSheet", menuName = "Scriptable Objects/Stat Sheet", order = 0)]
public class StatSheet : ScriptableObject
{
    public List<StatEntry> entries = new();

    // Runtime cache for quick lookups (not serialized)
    private Dictionary<Stat, float> cache;

    private void OnEnable()
    {
        RebuildCache();
    }

#if UNITY_EDITOR
    // Rebuild in editor when changed
    private void OnValidate()
    {
        RebuildCache();
    }
#endif

    private void RebuildCache()
    {
        if (entries == null)
        {
            cache = new Dictionary<Stat, float>();
            return;
        }

        cache = new Dictionary<Stat, float>(entries.Count);
        foreach (var e in entries)
        {
            cache[e.type] = e.baseValue;
        }
    }

    /// <summary>
    /// Returns the base value for the requested stat, or -1f if not present.
    /// </summary>
    public float GetBaseValue(Stat stat)
    {
        if (cache != null && cache.TryGetValue(stat, out var v))
            return v;

        // Fallback linear search in case cache wasn't built (very rare)
        if (entries != null)
        {
            foreach (var e in entries)
                if (e.type == stat)
                    return e.baseValue;
        }

        return -1f;
    }
}