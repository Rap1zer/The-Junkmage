using UnityEngine;
using System.Collections.Generic;

namespace JunkMage.Stats
{
    [CreateAssetMenu(fileName = "StatDefinitionDatabase", menuName = "Stats/Stat Database")]
    public class StatDefinitionDatabase : ScriptableObject
    {
        // default to empty array so we never get a null reference when iterating
        public StatDefinition[] definitions = new StatDefinition[0];

        private Dictionary<Stat, StatDefinition> cache;

        private static StatDefinitionDatabase instance;
        public static StatDefinitionDatabase Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<StatDefinitionDatabase>("StatDefinitionDatabase");
                    if (instance == null)
                    {
                        Debug.LogError("[StatDefinitionDatabase] Could not load 'StatDefinitionDatabase' from Resources. " +
                            "Make sure the asset exists at Assets/Resources/StatDefinitionDatabase.asset");
                        return null;
                    }

                    // ensure the cache is ready
                    instance.BuildCache();
                }
                return instance;
            }
        }

        private void OnEnable()
        {
            // If another instance is already set, warn
            if (instance != null && instance != this)
            {
                Debug.LogWarning("[StatDefinitionDatabase] Another StatDefinitionDatabase instance is already active.");
                // still build cache for this asset so inspector shows values correctly
                BuildCache();
                return;
            }

            instance = this;
            
            // Build or rebuild runtime cache whenever the asset is enabled/loaded
            BuildCache();
        }

        public void BuildCache()
        {
            cache = new Dictionary<Stat, StatDefinition>();

            if (definitions == null || definitions.Length == 0)
                return;

            for (int i = 0; i < definitions.Length; ++i)
            {
                var def = definitions[i];
                if (def == null)
                    continue;

                if (cache.ContainsKey(def.stat))
                {
                    Debug.LogWarning($"[StatDefinitionDatabase] Duplicate StatDefinition for {def.stat} (index {i}). Ignoring later entry.");
                    continue;
                }

                cache[def.stat] = def;
            }
        }

        public StatDefinition GetDefinition(Stat stat)
        {
            if (cache == null) BuildCache();
            if (cache != null && cache.TryGetValue(stat, out var def))
                return def;
            return null;
        }
    }
}
