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

    private void Start ( ) {
        
    }

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

        if((int)type == 0){
            Debug.Log("Entrou no if do tipo " + type);
            
            GameObject doorRef;
            doorRef = Instantiate(doorPrefab, spawnDoor[0].position, spawnDoor[0].rotation,spawnDoor[0]);
            door = doorRef.GetComponent<Door>();
            setAnswerDoor();
        }
        else if((int)type == 1){
            Debug.Log("Entrou no if do tipo " + type);
            for(int i = 0; i < spawnDoor.Length; i++){
                GameObject doorRef;
            doorRef = Instantiate(doorPrefab, spawnDoor[i].position, spawnDoor[i].rotation,spawnDoor[i]);
            door = doorRef.GetComponent<Door>();
            }
        }

        else if((int)type == 2)
            Debug.Log("Entrou no if do tipo " + type);
        else if((int)type == 3)
            Debug.Log("Entrou no if do tipo " + type);
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
