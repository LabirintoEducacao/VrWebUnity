using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkShapes : CheckBase
{
    
    [Tooltip("currentAnswer é a answer que o jogador adicionou a este script.")]
    public Answer currentAnswer;

    public override bool checkAnswer(Answer ans){
        if(ans != null){
            if(ans == answer){
                anim.SetBool("Right", true);
                anim.SetBool("Wrong", false);
                door.anim.SetTrigger("openning");
                return true;
            }
            else
            {
                anim.SetBool("Right", false);
                anim.SetBool("Wrong", true);
                door.anim.SetTrigger("closing");
                return false;
            }
        } else
        {
            return false;
        }
    }

}
