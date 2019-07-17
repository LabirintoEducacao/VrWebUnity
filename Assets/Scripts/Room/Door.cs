﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Door : MonoBehaviour
{
    /* Quando acontecer uma interação com a porta, o script deve verificar se a reposta(item)
     * que o jogador carrega com ele é o que deve ser utilizado para abrir a porta em que este
     * script se encontra.
     * 
     * Caso seja a resposta errada, o jogador perderá uma estrela.
     */

    public Answer AnswerCorrect;
    public Animator thisAnimator;

    public linkDoor ld;
    public bool seted;

    public bool openDoor;
    
    // private void Start() {
    //     ld.thisDoor = this;
    // }

    private void Update() {
        if(ld != null && AnswerCorrect != null && !seted){
            AnswerCorrect = ld.answerLinked;
            seted = true;
        }
    }

    
}