using System.Collections.Generic;
using UnityEngine;

namespace JunkMage.Stats
{
    [CreateAssetMenu(fileName = "StatSheet", menuName = "Scriptable Objects/Stat Sheet", order = 0)]
    public class StatSheet : ScriptableObject
    {
        [Header("Stat entries (use StatDefinition assets)")]
        public List<StatValueEntry> values = new();

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
        /// Rebuilds runtime cache. Preference:
        ///  - values[] using statDef.stat
        /// </summary>
        public void RebuildCache()
        {
            var newCache = new Dictionary<Stat, float>();

            if (values != null)
            {
                for (int i = 0; i < values.Count; i++)
                {
                    var sv = values[i];
                    if (sv == null || sv.statDef == null) continue;
                    // Use the enum mapping stored on the referenced StatDefinition
                    newCache[sv.statDef.stat] = sv.baseValue;
                }
            }

            // Atomic swap: assign the new dictionary reference.
            cache = newCache;
        }

        /// <summary>
        /// Returns base value or -1f if missing (same contract as before).
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
