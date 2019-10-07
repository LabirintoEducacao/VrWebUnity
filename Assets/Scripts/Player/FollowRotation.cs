using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class FollowRotation : MonoBehaviour
{
    public Transform Target;
    public bool OpenMenu = false;
    public Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        Vector3 rotation = new Vector3(transform.eulerAngles.x, Target.eulerAngles.y, transform.eulerAngles.z);

        if(Target.eulerAngles.x >= 56f && !OpenMenu)
        {
            Debug.Log("Entrou no Target Rotation >= 56f");
            //transform.rotation = Quaternion.Euler(rotation);
            OpenMenu = true;
            anim.SetBool("OpenMenu", OpenMenu);
        }
        else if(Target.eulerAngles.x < 56f && OpenMenu)
        {
            OpenMenu = false;
            anim.SetBool("OpenMenu", OpenMenu);
        }
        else if(Target.eulerAngles.x < 56f && !OpenMenu)
        {
            transform.rotation = Quaternion.Euler(rotation);
        }
    }
}
