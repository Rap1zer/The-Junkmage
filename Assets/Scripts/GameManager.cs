using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Room[] rooms = new Room[3];
    public GameObject[] doorObjs = new GameObject[2];
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rooms[2] = new Room(false, null , null);
        rooms[1] = new Room(false, rooms[2], doorObjs[1]);
        rooms[0] = new Room(true, rooms[1], doorObjs[0]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
