using UnityEngine;

public class Room
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

    public Room(int index, int enemyCount, Room nextRoom, GameObject exitDoor)
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
