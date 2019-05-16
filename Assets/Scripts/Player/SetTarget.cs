using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTarget : MonoBehaviour
{
    public Transform Target;

    private void Start()
    {
        GameController controller = GameObject.Find("GameController").GetComponent<GameController>();
        controller.set.Add(this);
    }
}
