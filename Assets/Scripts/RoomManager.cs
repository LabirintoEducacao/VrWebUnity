﻿using System;
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
    public Door EnterDoor;

    [Header ("room data")]
    public Question question;
    public int id { get => question.question_id; }
    public TypeRoom type;
    public bool doorSpawned;
    public GameManager manager;


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
            setTypeRoom();
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

    /* Quando a sala for de uma saída somente, o RoomManager deve indentificar se há uma proxima sala,
     * Ou se a a sala atual é a ultim. Em caso de haver um proxima sala, ele deve pegar a refenrencia
     * da porta EnterDoor(Porta de entrada da sala seguinte) e passar qual a resposta correta para ela,
     * para que ao jogador chegar na porta ele verifique se a resposta coletada é a correta.
     */
    void RoomTypeSingleDoor(){
        Debug.Log("Entrou no if do tipo " + type);

            GameObject doorEnter = Instantiate(doorPrefab, spawnDoor[0].position, spawnDoor[0].rotation,spawnDoor[0]);
            doorEnter.name = "EnterDoor";
            EnterDoor = doorEnter.GetComponent<Door>();

            if(question.paths[0].end_game == true){
                if(question.paths[0].end_game == true){
                        GameObject finalDoor = Instantiate(doorPrefab, spawnDoor[0].position, spawnDoor[0].rotation,spawnDoor[0]);
                        finalDoor.name = "FinalDoor";
                }
            } else{
                Invoke("setNextDoor", 2f);
            }
            // doorRef = Instantiate(doorPrefab, spawnDoor[1].position, spawnDoor[1].rotation,spawnDoor[1]);
            //door = GameObject.Find("EnterDoor").GetComponent<Door>();
            // if(door != null){
            //     Debug.Log("Pegou Porta");
            // }
    }

    void RoomTypeMultipleDoors(){
        Debug.Log("Entrou no if do tipo " + type);
            
            int i = 0;
            foreach(Transform spawn in spawnDoor){
                GameObject doorRef = null;
                if(i == 0){
                    doorRef = Instantiate(doorPrefab, spawnDoor[i].position, spawnDoor[i].rotation,spawnDoor[i]);
                    doorRef.name = "EnterDoor";
                    EnterDoor = doorRef.GetComponent<Door>();
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

    void setNextDoor(){
        RoomManager ConnectedRoom = null;
        if(ConnectedRoom == null){
            foreach (RoomManager room in manager.roomsObjects)
            {
                if(room.id == question.paths[0].connected_question){
                    ConnectedRoom = room;
                    break;
                }
            }
            if(ConnectedRoom != null){
                    Debug.Log("Funfou!!");
            } else{
                    Debug.Log("Não funfou!");
            }
        }
        door = ConnectedRoom.EnterDoor;
        GetCorrectAnswer();
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
