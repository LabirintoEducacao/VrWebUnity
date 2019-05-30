using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;

    [Header ("Rooms")]
    public TextAsset levelDesign;
    MazeLDWrapper mazeLD;

    public string tagRoom;
    public Transform mapRoot;
    public RoomDescriptor startingRoom;
    public GameObject roomPrefab;

    public List<RoomManager> rooms;
    public List<CorridorManager> corridors;
    public GameObject basicTilePrefab;
    public GameObject hubPrefab;

    private void Awake ( ) {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy (this.gameObject);
        }
    }

    private void Start ( ) {
        if (levelDesign == null) {
            Debug.LogError ("Cannot play without a level.");
        }
        mazeLD = readData (levelDesign.text);

        // StartCoroutine (CreateRooms ( ));
        // StartCoroutine (CreateCorridors ( ));
        CreateRooms ( );
        CreateCorridors();
    }

    public RoomManager getRoom (int id) {
        return this.rooms.Find (x => (x.question.question_id == id));
    }

    public CorridorManager[] getCorridorsByRoom (int question_id) {
        return this.corridors.FindAll (x => (x.question_id == question_id)).ToArray ( );
    }

    public void CreateRooms ( ) {
        this.rooms = new List<RoomManager> ( );
        this.corridors = new List<CorridorManager> ( );
        foreach (Question quest in mazeLD.questions) {
            GameObject go;
            if (quest.question_id == mazeLD.starting_question_id) {
                go = startingRoom.gameObject;
            } else {
                go = Instantiate (roomPrefab, new Vector3 (0f, -100f - (this.rooms.Count * 2f), 0f), Quaternion.identity, mapRoot);
            }
            // yield return new WaitForSeconds (0.1f);
            go.name = "room_" + quest.question_id;
            RoomManager rm = go.GetComponent<RoomManager> ( );
            rm.question = quest;
            rm.generateAnswers ( );

            this.rooms.Add (rm);
        }
        // yield break;
    }

    public void CreateCorridors ( ) {
        foreach (RoomManager room in this.rooms) {
            float sameRoomXPad = 1.5f;
            foreach (MazePath path in room.question.paths) {
                GameObject go = new GameObject ( );
                go.transform.parent = mapRoot;
                go.transform.position = room.transform.position + Vector3.right * sameRoomXPad; //+ Vector3.forward * 1.5f
                sameRoomXPad += path.width * 1.5f;
                go.name = "corridor_" + room.question.question_id + "_" + path.type;

                CorridorManager man = go.AddComponent<CorridorManager> ( );
                man.question_id = room.question.question_id;
                man.roomEntrance = room.GetComponentInChildren<HubCheckpoint> ( ).transform;
                man.pathInfo = path;
                if (path.connected_question != -1) {
                    man.roomExit = getRoom (path.connected_question).GetComponentInChildren<HubCheckpoint> ( ).transform;
                }
                
                man.generateCorridor ( this.basicTilePrefab, this.hubPrefab );
                // yield return new WaitForSeconds (0.1f);
                this.corridors.Add (man);
            }
        }
    }

    public MazeLDWrapper readData (string json) {
        return JsonUtility.FromJson<MazeLDWrapper> (json);
    }
}
