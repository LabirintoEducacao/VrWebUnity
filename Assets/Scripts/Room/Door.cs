using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Door : CheckBase
{
    /* Quando acontecer uma interação com a porta, o script deve verificar se a reposta(item)
     * que o jogador carrega com ele é o que deve ser utilizado para abrir a porta em que este
     * script se encontra.
     * 
     * Caso seja a resposta errada, o jogador perderá uma estrela.
     */

    public Answer AnswerCorrect;
    public linkDoor ld;
    

    public override bool checkAnswer(Answer ans){
        if(ans == answer){
            anim.SetTrigger("openning");
            return true;
        }
        return false;
    }

    /*
    Animator anim = door.thisAnimator;
                        anim.SetTrigger("openning");
 */

    
}
