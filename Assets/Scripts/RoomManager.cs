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


public class RoomManager : MonoBehaviour {
    [Header("Doors, Answer and Wall")]
    public int entranceDirection = Constants.DIRECTION_NONE;
    public Transform[] spawnAnswer;
    public Transform[] spawnDoor;
    public List<AnswerReference> answerReference;
    /* 0 - Key; 1 - Cube; 2 - Prism; 3 - Circle; */
    public GameObject[] answerPrefab;
    public GameObject WallPrefab;

    public GameObject doorPrefab;
    public Door door;
    public Door EnterDoor;

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

    private void Update() {
        if (testing) {
            if (Input.GetKeyDown(KeyCode.H)) {
                int door = UnityEngine.Random.Range(0,4);
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
    }

    void createDoors(){
        /* Quando for uma sala que contém somente uma porta de saida, deve spawnar 2 portas
         * No qual uma é de entrada, por onde o player vem e o outro é de sáida no qual o player
         * deve utilizar a o objeto resposta pra ver se está certo
         *
         * Lembrando que deve verificar onde está o corredor para que posicione a porta no local correto
         *
         * Em caso de sala com uma porta de saída, a porta deve ser posicionada no fim do corredor
         */
        if((int)type == 0){
            RoomTypeSingleDoor();
            
        }
        else if((int)type == 1){
            RoomTypeMultipleDoors();
        }

        else if((int)type == 2)
            RoomTypeObjectsAndShapes();
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

            if(question.paths[0].end_game == true){
                if(question.paths[0].end_game == true){
                        GameObject finalDoor = Instantiate(doorPrefab, spawnDoor[0].position, spawnDoor[0].rotation,spawnDoor[0]);
                        finalDoor.name = "FinalDoor";
                }
            } else{
                Invoke("setNextDoor", 2f);
            }
            int num = UnityEngine.Random.Range(0, 4);
            
            //Add Exit Door
            GameObject temp = Instantiate(doorPrefab, spawnDoor[0].position, spawnDoor[0].rotation,spawnDoor[0]);
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

            // doorRef = 
            //door = GameObject.Find("EnterDoor").GetComponent<Door>();
            // if(door != null){
            //     Debug.Log("Pegou Porta");
            // }
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
}
