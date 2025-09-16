using UnityEngine;

public class Room
{
    public int Index { get; private set; }
    public bool Cleared { get; private set; } = false;
    private Door exitDoor = null;

    public Room(bool cleared, int index, Room nextRoom, GameObject exitDoor)
    {
        Cleared = cleared;
        Index = index;
        if (exitDoor)
        {
            this.exitDoor = exitDoor.GetComponent<Door>();
            this.exitDoor.Initialize(this, nextRoom, exitDoor);
            if (Cleared) this.exitDoor.OpenDoor();
        }
    }
}
