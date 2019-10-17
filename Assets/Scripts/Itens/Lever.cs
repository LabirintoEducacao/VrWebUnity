using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : ItemBase
{
    public Animator anim;
         
    public bool isActivate = false;


    public override void ActionItem()
    {
        // TODO Ativa a alavanca
        isActivate = !isActivate;
        anim.SetBool("isActivate", isActivate);
    }
}
