using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckBase : MonoBehaviour
{

    public Answer answer;
    public Animator anim;

    public bool seted;
    public bool openDoor;

    public DoorStructure door;

    // Start is called before the first frame update
    public virtual bool checkAnswer(Answer ans){
        return false;
    }

    // void Update() {
    //     if(ld != null && answer == null && !seted){
    //         answer = ld.answerLinked;
    //         seted = true;
    //     }
    // }

}
