using System;
using System.Collections;
using System.Collections.Generic;
using larcom.MazeGenerator.Support;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Animations;


public enum TypeRoom
{
    OneDoor,
    MultipleDoors,
    ObjectsAndShapes,
    MultipleChoice
}

public struct  DoorStructure{
    public string name;
    public Door doorProperties;
    public int doorPosition;
}

public class RoomManager : MonoBehaviour {
    [Header("Answer")]
    public int entranceDirection = Constants.DIRECTION_NONE;
    public Transform[] spawnAnswer;
    public Transform[] spawnDoor;
    public List<AnswerReference> answerReference;
    /* 0 - Key; 1 - Cube; 2 - Prism; 3 - Circle; */
    [Tooltip("Prefabs das respostas que serão instanciados no mapa. 0 - Key; 1 - Cube; 2 - Prism; 3 - Circle; ")]
    public GameObject[] answerPrefab;
    [Header("Wall")]
    public GameObject WallPrefab;

    [Header("Door")]
    [Tooltip("Prefab da porta que será instanciado no mapa.")]
    public GameObject doorPrefab;
    public Door door;
    public Door EnterDoor;
    public DoorStructure[] portDatas;

    [Header("Animations")]
    public List<Animator> anims;

    [Header("UI")]
    public TextMeshProUGUI TextQuestion;

    [Header ("room data")]
    public Question question;
    public int id { get => question.question_id; }
    public TypeRoom type;
    public bool doorSpawned;
    public GameManager manager;
    public bool testing = false;

    private void Start() {
    }

    private void Update() {
        if (testing) {
            if (Input.GetKeyDown(KeyCode.H)) {
                int door = 0; //= UnityEngine.Random.Range(0,4);
                foreach (DoorStructure item in portDatas)
                {
                    if(item.name == "DoorAnswer"){
                        door = item.doorPosition;
                    }
                }

                int dir = Constants.DIRECTIONS[door];

                CorridorManager[] corridors = GameManager.Instance.getCorridorsByRoom(this.id);

                StartCoroutine(GameManager.Instance.placeNextCorridor(this.spawnDoor[door].position, this.transform.rotation, dir, corridors[0]));
            } 
        }
    }

    public void generateAnswers ( ) {
        if(question.room_type != "ObjectsAndShapes"){
            Answer[] answers = question.answers;
            GameObject[] answerTemp = new GameObject[answers.Length];
            answerReference = new List<AnswerReference> ( );

            List<Transform> molds = new List<Transform> (spawnAnswer);
            Tools.Shuffle (molds);

            for (int i = 0; i < answers.Length; i++) {
                GameObject go = Instantiate (answerPrefab[0], molds[i].position, molds[i].rotation, molds[i]);
                AnswerReference ansRef = go.GetComponent<AnswerReference> ( );
                ansRef.properties = answers[i];
                answerReference.Add (ansRef);
            }
        }

        if(!doorSpawned)
            setTypeRoom();

        TextQuestion.text = question.question;
    }

    /* Quando for uma sala que contém somente uma porta de saida, deve spawnar 2 portas
     * No qual uma é de entrada, por onde o player vem e o outro é de sáida no qual o player
     * deve utilizar a o objeto resposta pra ver se está certo
     *
     * Lembrando que deve verificar onde está o corredor para que posicione a porta no local correto
     *
     * Em caso de sala com uma porta de saída, a porta deve ser posicionada no fim do corredor
    */
    void setTypeRoom(){
        if(question.room_type == "key"){
            type = TypeRoom.OneDoor;
            RoomTypeSingleDoor();
        }
        else if(question.room_type == "doors"){
            type = TypeRoom.MultipleDoors;
            RoomTypeMultipleDoors();
        }
        else if(question.room_type == "ObjectsAndShapes"){
            type = TypeRoom.ObjectsAndShapes;
            RoomTypeObjectsAndShapes();
        }
        else if((int)type == 3)
            Debug.Log("Entrou no if do tipo " + type + this.gameObject.name);
    }

    /* Quando a sala for de uma saída somente, o RoomManager deve indentificar se há uma proxima sala,
     * Ou se a a sala atual é a ultim. Em caso de haver um proxima sala, ele deve pegar a refenrencia
     * da porta EnterDoor(Porta de entrada da sala seguinte) e passar qual a resposta correta para ela,
     * para que ao jogador chegar na porta ele verifique se a resposta coletada é a correta.
     */
    void RoomTypeSingleDoor(){
        List<int> count = new List<int>();
        
        
        Debug.Log("Entrou no if do tipo " + type + this.gameObject.name);

        GameObject doorEnter = Instantiate(doorPrefab, spawnDoor[2].position, spawnDoor[2].rotation,spawnDoor[2]);
        doorEnter.name = "EnterDoor";
        EnterDoor = doorEnter.GetComponent<Door>();
        count.Add(2);

        portDatas = new DoorStructure[2];

        DoorStructure ds = new DoorStructure();
        ds.name = doorEnter.name;
        ds.doorProperties = EnterDoor;
        ds.doorPosition = 2;
        
        portDatas[0] = ds;

        Debug.Log(portDatas[0].name + "\n" + "Door Cadastrado \n" + "Posição em que a porta se encontra: " + portDatas[0].doorPosition);

        if(question.paths[0].end_game == true){
            GameObject finalDoor = Instantiate(doorPrefab, spawnDoor[0].position, spawnDoor[0].rotation,spawnDoor[0]);
            finalDoor.name = "FinalDoor";
        } else{
            Invoke("setNextDoor", 2f);
        }
        int num = UnityEngine.Random.Range(0, 4);
        
        //Add Exit Door
        GameObject temp = Instantiate(doorPrefab, spawnDoor[0].position, spawnDoor[0].rotation,spawnDoor[0]);
        string nameDoor = "DoorAnswer";
        temp.name = nameDoor;

        ds.name = temp.name;
        ds.doorProperties = temp.GetComponent<Door>();
        ds.doorPosition = 0;

        count.Add(0);
        anims = new List<Animator>();
        Animator animTemp = temp.GetComponent<Animator>();
        anims.Add(animTemp);

        //Add Walls
        while(count.Count < 4){
            num = UnityEngine.Random.Range(0, 4);

            if(!count.Contains(num)){
                Instantiate(WallPrefab, spawnDoor[num].position, spawnDoor[num].rotation,spawnDoor[num]);
                count.Add(num);
            }
        }
        // PositionNextCorridorAndRoom("DoorAnswer");
    }

    

    void RoomTypeMultipleDoors(){
        Debug.Log("Entrou no if do tipo " + type + this.gameObject.name);
        int i = 0;
        foreach(Transform spawn in spawnDoor){
            GameObject doorRef = null;
            if(i == 0){
                doorRef = Instantiate(doorPrefab, spawnDoor[i].position, spawnDoor[i].rotation,spawnDoor[i]);
                doorRef.name = "EnterDoor";
                EnterDoor = doorRef.GetComponent<Door>();
                
                door = doorRef.GetComponent<Door>();
            }
            else if(i != 0) {
                string nameDoor = "DoorAnswer_" + i;
                
                
                doorRef = Instantiate(doorPrefab, spawnDoor[i].position, spawnDoor[i].rotation,spawnDoor[i]);
                doorRef.name = nameDoor;

                
                door = doorRef.GetComponent<Door>();
            }
            i++;
        }
        doorSpawned = true;
    }

    void RoomTypeObjectsAndShapes(){
        Debug.Log("Entrou no if do tipo " + type + this.gameObject.name);

        int s = 0;
        foreach(Transform spawn in spawnDoor){
            GameObject doorRef = null;
            if(s == 0){
                doorRef = Instantiate(doorPrefab, spawnDoor[s].position, spawnDoor[s].rotation,spawnDoor[s]);
                doorRef.name = "EnterDoor";
                EnterDoor = doorRef.GetComponent<Door>();
                
                door = doorRef.GetComponent<Door>();
            }
            else if(s != 0) {
                string nameDoor = "DoorAnswer_" + s;
                
                
                doorRef = Instantiate(doorPrefab, spawnDoor[s].position, spawnDoor[s].rotation,spawnDoor[s]);
                doorRef.name = nameDoor;

                
                door = doorRef.GetComponent<Door>();
            }
            s++;
        }
        doorSpawned = true;

        // Answer Spawn
        bool cube = false, prism = false, circle = false;

        Answer[] answers = question.answers;
        GameObject[] answerTemp = new GameObject[answers.Length];
        answerReference = new List<AnswerReference> ( );

        List<Transform> molds = new List<Transform> (spawnAnswer);
        Tools.Shuffle (molds);

        List<int> count = new List<int>();
        int i;
        
        while (count.Count < 3)
        { 
            i = UnityEngine.Random.Range(0, 3);
            Debug.Log("Entrou aqui!");
            if(!count.Contains(i)){
                if(!cube){
                    GameObject go = Instantiate (answerPrefab[i + 1], molds[i].position, molds[i].rotation, molds[i]);
                    AnswerReference ansRef = go.GetComponent<AnswerReference> ( );
                    ansRef.properties = answers[i];
                    answerReference.Add (ansRef);
                    count.Add(i);
                    cube = true;
                    
                    Debug.Log("Object: " + go.name + "\n in Spawn: " + i);
                } else if(!prism){
                    GameObject go = Instantiate (answerPrefab[i + 1], molds[i].position, molds[i].rotation, molds[i]);
                    AnswerReference ansRef = go.GetComponent<AnswerReference> ( );
                    ansRef.properties = answers[i];
                    answerReference.Add (ansRef);
                    count.Add(i);
                    prism = true;

                    Debug.Log("Object: " + go.name + "\n in Spawn: " + i);
                } else if(!circle){
                    GameObject go = Instantiate (answerPrefab[i + 1], molds[i].position, molds[i].rotation, molds[i]);
                    AnswerReference ansRef = go.GetComponent<AnswerReference> ( );
                    ansRef.properties = answers[i];
                    answerReference.Add (ansRef);
                    count.Add(i);
                    circle = true;

                    Debug.Log("Object: " + go.name + "\n in Spawn: " + i);
                }

                
            }
        }
        
    }

    void setNextDoor(){
        RoomManager ConnectedRoom = null;
        if(ConnectedRoom == null){
        foreach (RoomManager room in manager.roomsObjects)
        {
            if(room.id == question.paths[0].connected_question){
                ConnectedRoom = room;
                Debug.Log("Name ConnectedRoom: " + ConnectedRoom.name);
            }
            }
            if(ConnectedRoom != null){
                    Debug.Log("ConnectedRoom Não está vazio!!");
                    door = ConnectedRoom.EnterDoor;
                    if(door != null) GetCorrectAnswer();
            } else{
                Debug.Log("ConnectedRoom está vazio!");
            }
        }
    }

    void GetCorrectAnswer(){
        foreach(Answer ans in question.answers){
            if (ans.correct)
            {
                door.AnswerCorrect = ans;
            }
        }
    }

    public void PositionNextCorridorAndRoom(string nameDoor){
        int door = 0; //= UnityEngine.Random.Range(0,4);
        foreach (DoorStructure item in portDatas)
        {
            if(item.name ==nameDoor){
                door = item.doorPosition;
            }
        }

        int dir = Constants.DIRECTIONS[door];

        CorridorManager[] corridors = GameManager.Instance.getCorridorsByRoom(this.id);

        StartCoroutine(GameManager.Instance.placeNextCorridor(this.spawnDoor[door].position, this.transform.rotation, dir, corridors[0]));
    }
}
