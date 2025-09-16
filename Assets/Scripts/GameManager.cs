using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Room[] rooms = new Room[6];
    public GameObject[] doorObjs = new GameObject[5];
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialise the room classes and their doors.
        for (int i = rooms.Length - 1; i >= 0; i--)
        {
            if (i == rooms.Length - 1)
            {
                rooms[i] = new Room(false, i, null, null);
            }
            else if (i == 0)
            {
                rooms[0] = new Room(true, i, rooms[1], doorObjs[0]);
            }
            else
            {
                rooms[i] = new Room(false, i, rooms[i + 1], doorObjs[i]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
