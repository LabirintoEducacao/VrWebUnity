using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class checkShapes : CheckBase
{
    public override bool checkAnswer(Answer ans){
        if(ans != null){
            if(ans == answer){
                anim.SetBool("Right", true);
                anim.SetBool("Wrong", false);
                door.anim.SetTrigger("openning");
                foreach (ItemBase item in rmanager.answerReference)
                {
                    item.gameObject.SetActive(false);
                }
                Player.instance.currentRoom.PositionNextRoom("DoorAnswer", Inventory.instance.AnswerSelected.correct);
                

                return true;
            }
            else
            {
                anim.SetBool("Right", false);
                anim.SetBool("Wrong", true);

                return false;
            }
        } else
        {
            return false;
        }
    }

}
