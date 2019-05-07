using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private BoxCollider collider;

    [Header("Tags, Names and Layers")]
    [SerializeField] private string _tagButton = "Button";
    [SerializeField] private string _tagPlayer = "Player";

    [Header("Buttons")]
    [SerializeField] private GameObject[] _Buttons;
    [SerializeField] private GameObject[] _Temp;

    [Header("Variables")]
    [SerializeField] private float _RangeLimit;
    [SerializeField] private int size = 0;

    private void Start() {
        collider = GetComponent<BoxCollider>();

        _Temp = GameObject.FindGameObjectsWithTag(_tagButton);
        // _Buttons = GameObject.FindGameObjectsWithTag(_tagButton);
        
        getButtons();

        for (int i = 0; i < _Buttons.Length; i++)
        {
            _Buttons[i].SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag(_tagPlayer)){
            for (int i = 0; i < _Buttons.Length; i++)
            {
                _Buttons[i].SetActive(true);
            }
        }
    }

    public void desactiveButtons(){
        for (int i = 0; i < _Buttons.Length; i++)
            {
                _Buttons[i].SetActive(false);
            }
    }

    private void getButtons(){
        int i = 0;
        int j = 0;

        

        foreach (GameObject item in _Temp)
        {
            if((item.transform.position - transform.position).magnitude < _RangeLimit){
                size++;
            }
        }
        _Buttons = new GameObject[size];

        foreach (GameObject item in _Temp)
        {
            if((item.transform.position - transform.position).magnitude < _RangeLimit){
                _Buttons[i] = item;
                i++;
            }
        }

        size = i;

        _Buttons = new GameObject[size];

        foreach (GameObject item in _Temp)
        {
            if((item.transform.position - transform.position).magnitude < _RangeLimit){
                _Buttons[j] = item;
                j++;
            }
        }

        for (int m = 0; m < _Buttons.Length; m++)
        {
            SetDestination set;
            set = _Buttons[m].GetComponent<SetDestination>();
            set.check = this;
        }

        _Temp = new GameObject[0];
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _RangeLimit);
    }
}