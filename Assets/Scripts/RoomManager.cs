using System;
using System.Collections;
using System.Collections.Generic;
using larcom.MazeGenerator.Support;
using UnityEngine;

public enum TypeRoom
{
    OneDoor,
    MultipleDoors,
    ObjectsAndShapes,
    MultipleChoice
} 


public class RoomManager : MonoBehaviour {
    public int entranceDirection = Constants.DIRECTION_NONE;
    public Transform[] spawnAnswer;
    public Transform[] spawnDoor;
    public List<AnswerReference> answerReference;
    public GameObject answerPrefab;
    public GameObject doorPrefab;
    public Door door;
    [Header ("room data")]
    public Question question;
    public int id { get => question.question_id; }
    public TypeRoom type;
    public bool doorSpawned;
    public bool testing = false;




    public void generateAnswers ( ) {
        Answer[] answers = question.answers;
        GameObject[] answerTemp = new GameObject[answers.Length];
        answerReference = new List<AnswerReference> ( );

        List<Transform> molds = new List<Transform> (spawnAnswer);
        Tools.Shuffle (molds);

        for (int i = 0; i < answers.Length; i++) {
            GameObject go = Instantiate (answerPrefab, molds[i].position, molds[i].rotation, molds[i]);
            AnswerReference ansRef = go.GetComponent<AnswerReference> ( );
            ansRef.properties = answers[i];
            answerReference.Add (ansRef);
        }

        if(!doorSpawned)
            createDoors();
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
            Debug.Log("Entrou no if do tipo " + type);
        else if((int)type == 3)
            Debug.Log("Entrou no if do tipo " + type);
    }

    void RoomTypeSingleDoor(){
        Debug.Log("Entrou no if do tipo " + type);

            GameObject doorEnter = Instantiate(doorPrefab, spawnDoor[0].position, spawnDoor[0].rotation,spawnDoor[0]);
            doorEnter.name = "EnterDoor";

            GameObject doorRef;
            doorRef = Instantiate(doorPrefab, spawnDoor[1].position, spawnDoor[1].rotation,spawnDoor[1]);
            door = doorRef.GetComponent<Door>();
            setAnswerDoor();
    }

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

    void RoomTypeMultipleDoors(){
        Debug.Log("Entrou no if do tipo " + type);
            
            int i = 0;
            foreach(Transform spawn in spawnDoor){
                GameObject doorRef = null;
                if(i == 0){
                    doorRef = Instantiate(doorPrefab, spawnDoor[i].position, spawnDoor[i].rotation,spawnDoor[i]);
                    doorRef.name = "EnterDoor";
                    i++;
                }
                else {
                    string nameDoor = "DoorAnswer_" + i;
                    
                    
                    doorRef = Instantiate(doorPrefab, spawnDoor[i].position, spawnDoor[i].rotation,spawnDoor[i]);
                    doorRef.name = nameDoor;

                    i++;
                }
                door = doorRef.GetComponent<Door>();
            }
            doorSpawned = true;
    }

    void setAnswerDoor(){
        
        
        foreach (AnswerReference item in answerReference)
        {
            if(item.properties.correct)
                door.AnswerCorrect = item.properties;
        }
    }

    void setAnswerForMultipleDoors(){

    }
}
