using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shapes : ItemBase
{
    public override void ActionItem(){
        currentRoom.PositionNextRoom("DoorAnswer", Inventory.instance.AnswerSelected.correct);
    }
}
