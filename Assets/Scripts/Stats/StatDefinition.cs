using UnityEngine;

namespace JunkMage.Player
{
    [CreateAssetMenu(fileName = "StatDefinition", menuName = "Stats/Stat Definition", order = 0)]
    public class StatDefinition : ScriptableObject
    {
        // Map to enum for compatibility. Keep this in sync with your Stat enum.
        public Stat stat;

        [Header("Designer metadata")]
        public string displayName;
        [TextArea(2,4)]
        public string description;
        public float defaultValue = 0f;
        public float min = float.NegativeInfinity;
        public float max = float.PositiveInfinity;

        public string Label => string.IsNullOrEmpty(displayName) ? name : displayName;
    }
}