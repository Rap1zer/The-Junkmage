using UnityEngine;

namespace JunkMage.Environment
{
    public class Room : MonoBehaviour
    {
        public int Index { get; private set; }
    
        public float MinX { get; private set; }
        public float MaxX { get; private set; }
        public float MinY { get; private set; }
        public float MaxY { get; private set; }

        private int enemyCount;
        public int EnemyCount
        {
            get => enemyCount;
            set
            {
                enemyCount = value;
                if (EnemyCount <= 0) Cleared = true;
            }
        }

        private bool cleared = false;
        public bool Cleared
        {
            get => cleared;
            private set
            {
                cleared = value;
                if (value)
                {
                    if (exitDoor) exitDoor.OpenDoor();
                    GameObject.Find("Player").GetComponent<EntityEventDispatcher>().DispatchRoomCleared();
                }
            }
        }

        private Door exitDoor = null;

        // Set room boundaries
        void Awake()
        {
            Transform floor = null;

            foreach (Transform t in GetComponentsInChildren<Transform>())
            {
                if (t.CompareTag("Floor"))
                {
                    floor = t;
                    break;
                }
            }
            
            MinX = floor.position.x - floor.localScale.x / 2;;
            MaxX = floor.position.x + floor.localScale.x / 2;
            MinY = floor.position.y - floor.localScale.y / 2;;
            MaxY = floor.position.y + floor.localScale.y / 2;
        }

        public void Initialise(int index, int enemyCount, Room nextRoom, GameObject exitDoor)
        {
            if (exitDoor)
            {
                this.exitDoor = exitDoor.GetComponent<Door>();
                this.exitDoor.Initialize(this, nextRoom, exitDoor);
            }
            Index = index;
            EnemyCount = enemyCount;
        }
    }
}
