using UnityEngine;
using System.Collections.Generic;

namespace JunkMage.Player
{
    [CreateAssetMenu(fileName = "StatRegistry", menuName = "Stats/Stat Registry", order = 1)]
    public class StatRegistry : ScriptableObject
    {
        public List<StatDefinition> definitions = new();

        // runtime cache for fast lookups (not serialized)
        private Dictionary<Stat, StatDefinition> cache;

        private void OnEnable()
        {
            // Build a fresh cache on domain reload/enable (fast)
            RebuildCache_Internal();
        }

#if UNITY_EDITOR
        // Keep OnValidate minimal — don't modify other assets here.
        private void OnValidate()
        {
            // Only rebuild runtime cache; don't call AssetDatabase or SetDirty here
            RebuildCache_Internal();
        }
#endif

        // Internal rebuild: create a new dictionary and swap reference atomically.
        public void RebuildCache_Internal()
        {
            var newCache = new Dictionary<Stat, StatDefinition>(definitions == null ? 0 : definitions.Count);
            if (definitions != null)
            {
                for (int i = 0; i < definitions.Count; i++)
                {
                    var d = definitions[i];
                    if (d != null)
                        newCache[d.stat] = d;
                }
            }
            cache = newCache; // atomic swap; avoids mutating a dictionary in place during reads
        }
        
        public void RebuildCache()
        {
            cache = new Dictionary<Stat, StatDefinition>(definitions.Count);
            foreach (var d in definitions)
                if (d != null)
                    cache[d.stat] = d;
        }
        
        public bool TryGetDefinition(Stat stat, out StatDefinition def)
        {
            if (cache == null)
                RebuildCache();

            // declare def first
            if (cache != null && cache.TryGetValue(stat, out def))
                return true;

            def = null;
            return false;
        }
    }
}