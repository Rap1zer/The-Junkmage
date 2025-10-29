using UnityEngine;

namespace JunkMage.Systems
{
    public struct DamageInfo
    {
        public float Dmg { get; set; }
        public bool IsCrit { get; set; }
        public GameObject Attacker { get; set; }
        public GameObject Target { get; set; }
    }
}