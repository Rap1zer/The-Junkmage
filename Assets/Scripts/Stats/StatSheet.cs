using System.Collections.Generic;
using UnityEngine;

namespace JunkMage.Stats
{
    [CreateAssetMenu(fileName = "StatSheet", menuName = "Scriptable Objects/Stat Sheet", order = 0)]
    public class StatSheet : ScriptableObject
    {
        [Header("Preferred: StatDef-backed entries")]
        public List<StatValueEntry> values = new();

        [Header("Legacy (kept for compatibility)")]
        public List<StatEntry> entries = new();

        // runtime cache (Stat -> baseValue). Build via RebuildCache().
        private Dictionary<Stat, float> cache;

        private void OnEnable()
        {
            RebuildCache();
        }

#if UNITY_EDITOR
        // Keep OnValidate cheap: rebuild internal cache only (no asset writes).
        private void OnValidate()
        {
            RebuildCache();
        }
#endif

        /// <summary>
        /// Rebuilds runtime cache. Preference order:
        ///  1) values[] using statDef.stat
        ///  2) legacy entries[]
        /// </summary>
        public void RebuildCache()
        {
            var newCache = new Dictionary<Stat,float>();

            if (values != null)
            {
                for (int i = 0; i < values.Count; i++)
                {
                    var sv = values[i];
                    if (sv == null || sv.statDef == null) continue;
                    // statDef.stat is the enum mapping; use it.
                    newCache[sv.statDef.stat] = sv.baseValue;
                }
            }

            // Fill missing stats from legacy entries if not already present.
            if (entries != null)
            {
                for (int i = 0; i < entries.Count; i++)
                {
                    var e = entries[i];
                    if (e == null) continue;
                    if (!newCache.ContainsKey(e.type))
                        newCache[e.type] = e.baseValue;
                }
            }

            // Atomic swap: assign the new dictionary reference.
            cache = newCache;
        }

        /// <summary>
        /// Same contract as before: returns base value or -1f if missing.
        /// </summary>
        public float GetBaseValue(Stat stat)
        {
            if (cache == null) RebuildCache();
            if (cache != null && cache.TryGetValue(stat, out var v)) return v;
            return -1f;
        }
    }
    
    [System.Serializable]
    public class StatValueEntry
    {
        [Tooltip("Reference to the StatDefinition asset (preferred).")]
        public StatDefinition statDef;

        [Tooltip("Base value for this stat.")]
        public float baseValue = 0f;

        public StatValueEntry() { }

        public StatValueEntry(StatDefinition def, float value)
        {
            statDef = def;
            baseValue = value;
        }
    }

}