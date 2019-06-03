using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    /* Quando acontecer uma interação com a porta, o script deve verificar se a reposta(item)
     * que o jogador carrega com ele é o que deve ser utilizado para abrir a porta em que este
     * script se encontra.
     * 
     * Caso seja a resposta errada, o jogador perderá uma estrela.
     */

    public Answer AnswerCorrect;
    public RoomManager room;
    public bool openDoor;

    private void Start() {
        Invoke("getAnswer", 1);
    }

    void getAnswer(){
        foreach (AnswerReference item in room.answerReference)
        {
            if(item.properties.correct)
                AnswerCorrect = item.properties;
        }
    }
}
