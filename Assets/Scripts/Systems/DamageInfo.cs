using UnityEngine;

namespace JunkMage.Systems
{
    public struct DamageInfo
    {
        public float Dmg;
        public bool IsCrit;
        public GameObject Attacker;
        public GameObject Target;
    }
}