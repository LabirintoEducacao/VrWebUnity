using System.Collections;
using System.Collections.Generic;
using larcom.MazeGenerator.Support;
using UnityEngine;

[RequireComponent(typeof(NavMeshBaker))]
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

    RoomManager currentRoom;
    RoomManager nextRoom;
    CorridorManager currentCorridor;

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
        CreateCorridors ( );
        StartCoroutine(this.placeContinuation());
    }

    public RoomManager getRoom (int id) {
        return this.rooms.Find (x => (x.id == id));
    }

    public CorridorManager[] getCorridorsByRoom (int question_id) {
        return this.corridors.FindAll (x => (x.question_id == question_id)).ToArray ( );
    }

    IEnumerator placeContinuation() {
        Vector3 nextCorrPivot = currentRoom.GetComponent<RoomDescriptor>().topLeft;
        Vector2 roomSize = currentRoom.GetComponent<RoomDescriptor>().size;

        CorridorManager[] corridors = getCorridorsByRoom(currentRoom.id);
        if (this.currentCorridor != null) {
            this.currentCorridor.gameObject.SetActive(false);
            this.currentCorridor = null;
        }
        
        if (corridors.Length > 0) {
            currentCorridor = getCorridorsByRoom(currentRoom.id)[0];
        } else {
            Debug.Log("Must have reached the endgame.");
            yield break;
        }
        currentCorridor.transform.position = nextCorrPivot + Vector3.right * roomSize.x/2f;
        currentCorridor.gameObject.SetActive(true);
        yield return new WaitForEndOfFrame();

        if (!currentCorridor.pathInfo.end_game) {
            nextRoom = getRoom(currentCorridor.pathInfo.connected_question);
            if (nextRoom == null) {
                Debug.LogError(string.Format("Room {0} does not exist on path of type {1} from room {2}.",new object[] {currentCorridor.pathInfo.connected_question, currentCorridor.pathInfo.type, currentRoom.id}));
                yield break;
            }
            nextRoom.transform.position = nextCorrPivot + Vector3.forward * currentCorridor.pathInfo.height + Vector3.forward * nextRoom.GetComponent<RoomDescriptor>().size.y; // currentCorridor.roomExit.position+Vector3.forward*1f;
            //change listener
            currentRoom.GetComponentInChildren<HubCheckpoint>().onPlayerEnter -= onEnteredNextRoom;
            nextRoom.GetComponentInChildren<HubCheckpoint>().onPlayerEnter += onEnteredNextRoom;

            yield return new WaitForEndOfFrame();
            CorridorGenerator ccGen = currentCorridor.GetComponent<CorridorGenerator>();
            currentRoom.GetComponentInChildren<HubCheckpoint>().setGoal(Constants.DIRECTION_UP, ccGen.getEntranceTranform());
            ccGen.setEntranceMotion(currentRoom.GetComponentInChildren<HubCheckpoint>().transform);
            ccGen.setExitMotion(nextRoom.GetComponentInChildren<HubCheckpoint>().transform);
            this.nextRoom.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(0.1f);
        this.GetComponent<NavMeshBaker>().CreateBake();
        this.currentRoom.GetComponentInChildren<HubCheckpoint>().activate();
    }

    void onEnteredNextRoom(HubCheckpoint hub) {
        this.currentRoom.gameObject.SetActive(false);
        this.currentCorridor.gameObject.SetActive(false);
        this.currentRoom = this.nextRoom;
        StartCoroutine(placeContinuation());
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
            if (quest.question_id == mazeLD.starting_question_id) {
                currentRoom = rm;
            } else {
                go.SetActive (false);
            }
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
                go.name = "corridor_" + room.id + "_" + path.type;

                CorridorManager man = go.AddComponent<CorridorManager> ( );
                man.question_id = room.id;
                man.roomEntrance = room.GetComponentInChildren<HubCheckpoint> ( ).transform;
                man.pathInfo = path;
                if (path.connected_question != -1) {
                    man.roomExit = getRoom (path.connected_question).GetComponentInChildren<HubCheckpoint> ( ).transform;
                }

                man.generateCorridor (this.basicTilePrefab, this.hubPrefab);
                // yield return new WaitForSeconds (0.1f);
                man.gameObject.SetActive (false);
                this.corridors.Add (man);
            }
        }
    }

    public MazeLDWrapper readData (string json) {
        return JsonUtility.FromJson<MazeLDWrapper> (json);
    }
}
