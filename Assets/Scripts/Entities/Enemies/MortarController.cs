using JunkMage.Environment;
using UnityEngine;
using UnityEngine.Serialization;

namespace JunkMage.Entities.Enemies
{
    public class MortarController : EnemyBase
    {
        [Header("Mortar Settings")]
        [FormerlySerializedAs("dmgIndicator")] [SerializeField] private GameObject dmgZone;
        [SerializeField] private int dmgZoneCount = 2;
        private Room myRoom;

        protected override void Start()
        {
            base.Start();
            GameObject[] rooms = GameObject.FindGameObjectsWithTag("Room");
            foreach (GameObject roomObj in rooms)
            {
                Room room = roomObj.GetComponent<Room>();
                if (room.Index == roomIndex)
                {
                    myRoom = room;
                    break;
                }
            }
        }
        
        protected override void DoAttackBehavior()
        {
            if (AttackCooled() && PlayerInRoom) Attack();
        }

        protected override void Attack()
        {
            base.Attack();
            for (int i = 0; i < dmgZoneCount; i++)
            {
                Vector3 randomPos = new Vector3(Random.Range(myRoom.MinX, myRoom.MaxX), Random.Range(myRoom.MinY, myRoom.MaxY), 0);
                GameObject zone = Instantiate(dmgZone, randomPos, Quaternion.identity);
                zone.GetComponent<DamageZone>().Owner = this;
            }
        }

        public void OnHit()
        {
            playerHealth.TakeDamage(2, gameObject);
        }
    }
}
