using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class linkDoor : MonoBehaviour
{
    public Answer answerLinked;
    public Door thisDoor;

    private void Start() {
        thisDoor.answer = answerLinked;
    }
    
}
