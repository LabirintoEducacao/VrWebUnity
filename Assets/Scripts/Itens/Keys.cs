using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keys : ItemBase
{
    

    public override void ActionItem(){
        currentRoom.PositionNextRoom("DoorAnswer", Inventory.instance.AnswerSelected.correct);
        foreach (DoorStructure room in currentRoom.portDatas)
        {
            if (room.name == "DoorAnswer")
            {
                room.anim.SetTrigger("openning");
            }
        }
    }
}
