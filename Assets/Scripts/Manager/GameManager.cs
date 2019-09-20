using System.Collections;
using System.Collections.Generic;
using larcom.MazeGenerator.Models;
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

    public RoomManager[] roomsObjects;
    public int cellSize = 1;

    public List<RoomManager> rooms;
    public List<CorridorManager> corridors;
    public GameObject basicTilePrefab;
    public GameObject hubPrefab;

    public RoomManager currentRoom;
    public RoomManager nextRoom;
    CorridorManager currentCorridor;

    private void Awake ( ) {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy (this.gameObject);
        }
    }

    private void Start () {
        if (DataManager.manager.mazeLD != null) {
            Debug.Log("Loaded QR Code level.");
            mazeLD = DataManager.manager.mazeLD;
        } else {
            if (levelDesign == null) {
                Debug.LogError ("Cannot play without a level.");
            }
            mazeLD = readData (levelDesign.text);
			DataManager.manager.setNewLevel(mazeLD);
        }

        // StartCoroutine (CreateRooms ( ));
        // StartCoroutine (CreateCorridors ( ));
        CreateRooms ( );
        CreateCorridors ( );
        // StartCoroutine(this.placeContinuation());
    }

    public RoomManager getRoom (int id) {
        return this.rooms.Find (x => (x.id == id));
    }

    public CorridorManager[] getCorridorsByRoom (int question_id) {
        return this.corridors.FindAll (x => (x.question_id == question_id)).ToArray ( );
    }

    public CorridorManager[] getCorridorsByRoomAndType(int question_id, string room_type) {
        return this.corridors.FindAll (x => ((x.question_id == question_id)&&(x.pathInfo.type.Equals(room_type)))).ToArray ( );
    }

    public CorridorManager[] getCorridorsByRoomAndAvailbility(int question_id, string availbility)
    {
        return this.corridors.FindAll(x => ((x.question_id == question_id) && (x.pathInfo.availability.Equals(availbility)))).ToArray();
    }

    public IEnumerator placeNextCorridor(Vector3 position, Quaternion baseRot, int direction, CorridorManager corridor) {
        Vector3 nextCorrPivot = position;
		int delta = (Mathf.RoundToInt(baseRot.eulerAngles.y / 90) + 4) % 4;
		int true_dir = (Tools.directionToIndex (direction)  + delta) % 4;

        //se já tem corredor, desabilita
        // if (this.currentCorridor != null) {
        //     this.currentCorridor.gameObject.SetActive(false);
        // }
        this.currentCorridor = corridor;

        Debug.Log(currentCorridor.pathInfo.connected_question);

        if (this.currentCorridor == null) {
            Debug.LogError (string.Format ("Cannot allocate inexistent corridor. Room-{0}", new object[] { currentRoom.id }));
            yield break;
        }
        // Debug.Log("generating corridor in direction: "+direction+" index: "+Tools.directionToIndex(direction));
        // coloca o novo corredor em posição e rotação e espera até o final do frame pra continuar devido a problemas de render/update
        float rot = Constants.ROTATIONS[true_dir];
        MapCoord d = Constants.DELTA[true_dir];
        Vector3 fwd = new Vector3 (d.x, 0f, d.y);
        d = Constants.DELTA[(true_dir + 1) % 4];
        Vector3 right = new Vector3 (d.x, 0f, d.y);
        float corrEntx = (corridor.GetComponent<CorridorGenerator> ().entrance.x + 0.5f) * corridor.cellSize;
        this.currentCorridor.transform.rotation = Quaternion.Euler (0f, rot + baseRot.y, 0f);
        this.currentCorridor.transform.position = nextCorrPivot - right * corrEntx;
        this.currentCorridor.gameObject.SetActive (true);
        yield return new WaitForEndOfFrame ();

        if (!currentCorridor.pathInfo.end_game) {
            nextRoom = getRoom (currentCorridor.pathInfo.connected_question);
            if (nextRoom == null) {
                Debug.LogError (string.Format ("Room {0} does not exist on path of type {1} from room {2}.", new object[] { currentCorridor.pathInfo.connected_question, currentCorridor.pathInfo.type, currentRoom.id }));
                yield break;
            }
            //add delta to finishing position in corridor
            Transform exitHub = corridor.GetComponent<CorridorGenerator> ().getExitTranform ();

            float corrEndx = (corridor.GetComponent<CorridorGenerator> ().exit.x + 0.5f) * corridor.cellSize;
            nextCorrPivot += (fwd * (corridor.pathInfo.height * corridor.cellSize) + right * corrEndx);

            nextCorrPivot = exitHub.transform.position + exitHub.forward * cellSize / 2f;
            nextCorrPivot.y -= 1.5f;

            nextRoom.transform.rotation = Quaternion.Euler (0f, rot + baseRot.y, 0f);
            //nextRoom.transform.position = nextCorrPivot + Vector3.forward * currentCorridor.pathInfo.height + Vector3.forward * nextRoom.GetComponent<RoomDescriptor>().size.y;
            nextRoom.transform.position = nextCorrPivot + fwd * nextRoom.GetComponent<RoomDescriptor> ().size.y - right * nextRoom.GetComponent<RoomDescriptor> ().size.x / 2f; // - nextRoom.spawnDoor[0].transform.localPosition;

            //change listener
            currentRoom.GetComponentInChildren<HubCheckpoint> ().onPlayerEnter -= onEnteredNextRoom;
            nextRoom.GetComponentInChildren<HubCheckpoint> ().onPlayerEnter += onEnteredNextRoom;

            yield return new WaitForEndOfFrame ();
            CorridorGenerator ccGen = currentCorridor.GetComponent<CorridorGenerator> ();
            currentRoom.GetComponentInChildren<HubCheckpoint> ().setGoal (direction, ccGen.getEntranceTranform ());
            ccGen.setEntranceMotion (currentRoom.GetComponentInChildren<HubCheckpoint> ().transform);
            //ccGen.setExitMotion (nextRoom.GetComponentInChildren<HubCheckpoint> ().transform);
            this.nextRoom.gameObject.SetActive (true);
        }
        yield return new WaitForSeconds (0.1f);
        this.GetComponent<NavMeshBaker> ().CreateBake ();
        this.currentRoom.GetComponentInChildren<HubCheckpoint> ().activate ();
    }

    IEnumerator placeContinuation() {
        Vector3 nextCorrPivot = currentRoom.GetComponent<RoomDescriptor>().topLeft;
        Vector2 roomSize = currentRoom.GetComponent<RoomDescriptor>().size;

        CorridorManager[] corridors = getCorridorsByRoom (currentRoom.id);
        CorridorManager corr = null;
        if (corridors.Length > 0) {
            corr = corridors[0];
        } else {
            Debug.Log ("Must have reached the endgame.");
            yield break;
        }
        yield return placeNextCorridor (nextCorrPivot + Vector3.right * roomSize.x / 2f, this.transform.rotation, Constants.DIRECTION_UP, corr);
    }

    void onEnteredNextRoom (HubCheckpoint hub) {
        if(this.currentRoom.GetComponentInChildren<HubCheckpoint>() == hub)
            return;
        this.currentRoom.gameObject.SetActive (false);
        this.currentCorridor.gameObject.SetActive (false);
        this.currentRoom = this.nextRoom;
        // this.currentRoom.gameObject.SetActive (true);
    }

    public void CreateRooms () {
        this.rooms = new List<RoomManager> ();
        this.corridors = new List<CorridorManager> ();
        this.roomsObjects = new RoomManager[mazeLD.questions.Length];
        int i = 0;

        foreach (Question quest in mazeLD.questions) {
            GameObject go;
            if (quest.question_id == mazeLD.starting_question_id) {
                go = startingRoom.gameObject;
            } else {
                go = Instantiate (roomPrefab, new Vector3 (0f, -100f - (this.rooms.Count * 2f), 0f), Quaternion.identity, mapRoot);
            }
            // yield return new WaitForSeconds (0.1f);
            go.name = "room_" + quest.question_id;
            RoomManager rm = go.GetComponent<RoomManager> ();
            if (quest.question_id == mazeLD.starting_question_id) {
                currentRoom = rm;
            } else {
                go.SetActive (false);
            }
            rm.question = quest;
            rm.generateAnswers ();
            rm.manager = this;
            roomsObjects[i] = go.GetComponent<RoomManager> ();
            i++;

            this.rooms.Add (rm);
        }
        // yield break;
    }

    public void CreateCorridors () {
        foreach (RoomManager room in this.rooms) {
            float sameRoomXPad = 1.5f;
            foreach (MazePath path in room.question.paths) {
                GameObject go = new GameObject ();
                go.transform.parent = mapRoot;
                go.transform.position = room.transform.position + Vector3.right * sameRoomXPad; //+ Vector3.forward * 1.5f
                sameRoomXPad += path.width * 1.5f;
                go.name = "corridor_" + room.id + "_" + path.type;

                CorridorManager man = go.AddComponent<CorridorManager> ();
                man.cellSize = this.cellSize;
                man.question_id = room.id;
                man.roomEntrance = room.GetComponentInChildren<HubCheckpoint> ().transform;
                man.pathInfo = path;
                if (path.connected_question != -1) {
                    man.roomExit = getRoom (path.connected_question).GetComponentInChildren<HubCheckpoint> ().transform;
                }

                man.generateCorridor (this.basicTilePrefab, this.hubPrefab);
                // yield return new WaitForSeconds (0.1f);
                man.gameObject.SetActive (false);
                this.corridors.Add (man);
            }
        }
    }

    public void setExitMotionInHub()
    {
        currentCorridor.generator.setExitMotion(nextRoom.gameObject.GetComponentInChildren<HubCheckpoint>().transform);
    }

    public MazeLDWrapper readData (string json) {
        return JsonUtility.FromJson<MazeLDWrapper> (json);
    }
}
