using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    [Header("Names Objects")]
    [SerializeField] private string TagPlayer = "Player";

    [Header("Scripts")]
    private PlayerMovement _npc;

    private void Awake() {
        _npc = GameObject.Find(TagPlayer).GetComponent<PlayerMovement>();
    }

    public void PlayAI(){
        
    }
}
