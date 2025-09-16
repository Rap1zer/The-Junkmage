using UnityEngine;

public class Room
{
    public bool Cleared { get; private set; } = false;
    private Door exitDoor = null;

    public Room(Door entryDoor) { this.exitDoor = entryDoor; }

    public Room(bool cleared, Room nextRoom, GameObject exitDoor)
    {
        Cleared = cleared;
        if (exitDoor) this.exitDoor = new Door(this, nextRoom, exitDoor);

        if (Cleared)
        {
            this.exitDoor.OpenDoor();
        }
    }
}
