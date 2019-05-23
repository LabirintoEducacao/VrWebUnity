using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;

    [Header ("Rooms")]
    public TextAsset levelDesign;
    MazeLDWrapper mazeLD;

    public RoomManager[ ] rooms;
    public string tagRoom;

    private void Awake ( ) {
        if (Instance == null) {
            Instance = new GameManager ( );
        } else {
            Destroy (this.gameObject);
        }
    }

    private void Start ( ) {
        if (levelDesign == null) {
            Debug.LogError ("Cannot play without a level.");
        }
        mazeLD = readData (levelDesign.text);

        GetRooms ( );

    }

    public void GetRooms ( ) {
        GameObject[ ] roomsObjects = GameObject.FindGameObjectsWithTag (tagRoom);
        rooms = new RoomManager[roomsObjects.Length];
        if (roomsObjects.Length > 0) {
            for (int i = 0; i < roomsObjects.Length; i++) {
                rooms[i] = roomsObjects[i].GetComponent<RoomManager> ( );
            }
            setPropertiesRooms ( );
        }
    }

    public void setPropertiesRooms ( ) {
        for (int i = 0; i < rooms.Length; i++) {
            rooms[i].question = mazeLD.questions[0];
        }
    }

    public MazeLDWrapper readData (string json) {
        return JsonUtility.FromJson<MazeLDWrapper> (json);
    }
}
