using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    [Header("Names Objects")]
    [SerializeField] private string _PlayerName;

    [Header("Scripts")]
    private Npc_Movement _npc;

    private void Awake() {
        _npc = GameObject.Find(_PlayerName).GetComponent<Npc_Movement>();
    }

    public void PlayAI(){
        
    }
}
