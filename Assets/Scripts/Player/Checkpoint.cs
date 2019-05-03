using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private GameObject[] _Buttons;
    [SerializeField] private GameObject[] _Temp;
    [SerializeField] private string _tagButton = "Button";

    [Header("Variables")]
    [SerializeField] private float _RangeLimit;

    private void Start() {
        _Temp = GameObject.FindGameObjectsWithTag(_tagButton);
        _Buttons = GameObject.FindGameObjectsWithTag(_tagButton);
        
        getButtons();
    }

    private void getButtons(){
        int i = 0;
        int j = 0;
        foreach (GameObject item in _Temp)
        {
            if((item.transform.position - transform.position).magnitude < _RangeLimit){
                _Buttons[i] = item;
                i++;
                _Buttons[j] = null;
            }
            j++;
        }

        int Length = _Buttons.Length - 1;
        for (int k = 0; k < _Buttons.Length; k++)
        {
            if(_Buttons[Length] == _Buttons[k] || _Buttons[k] == null){
                _Buttons[Length] = null;
            }
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _RangeLimit);
    }
}