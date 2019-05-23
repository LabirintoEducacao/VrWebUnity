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

        StartCoroutine(CreateRooms( ));
    }

    public RoomManager getRoom(int id) {
        return this.rooms.Find(x=> x.question.question_id == id);
    }

    public IEnumerator CreateRooms ( ) {
        this.rooms = new List<RoomManager>();
        foreach (Question quest in mazeLD.questions)
        {
            GameObject go;
            if (quest.question_id == mazeLD.starting_question_id) {
                go = startingRoom.gameObject;
            } else {
                go = Instantiate(roomPrefab, new Vector3(this.rooms.Count*this.startingRoom.size.x*2f, -300f, 0f), Quaternion.identity, mapRoot);
            }
            yield return new WaitForSeconds(0.1f);
            go.name = "room_"+quest.question_id;
            RoomManager rm = go.GetComponent<RoomManager>();
            rm.question = quest;
            rm.generateAnswers();

            this.rooms.Add(rm);
        }
    }

    public MazeLDWrapper readData (string json) {
        return JsonUtility.FromJson<MazeLDWrapper> (json);
    }
}
