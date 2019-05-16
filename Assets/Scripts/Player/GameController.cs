using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public List<SetTarget> set;

    private void Awake()
    {
        set = new List<SetTarget>();
    }
}
