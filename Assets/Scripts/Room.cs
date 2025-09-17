using UnityEngine;

public class Room
{
    public int Index { get; private set; }

    private int enemyCount;
    public int EnemyCount
    {
        get => enemyCount;
        set
        {
            enemyCount = value;
            if (EnemyCount <= 0)
            {
                Cleared = true;
                if (exitDoor) exitDoor.OpenDoor();
            }
        }
    }

    public bool Cleared { get; private set; } = false;
    private Door exitDoor = null;

    public Room(bool cleared, int index, int enemyCount, Room nextRoom, GameObject exitDoor)
    {
        Cleared = cleared;
        Index = index;
        EnemyCount = enemyCount;
        if (exitDoor)
        {
            this.exitDoor = exitDoor.GetComponent<Door>();
            this.exitDoor.Initialize(this, nextRoom, exitDoor);
            if (Cleared) this.exitDoor.OpenDoor();
        }
    }
}
