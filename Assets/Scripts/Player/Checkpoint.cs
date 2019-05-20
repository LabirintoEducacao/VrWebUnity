using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public GameObject[] buttons;
    GameController controller;
    public Player p;
    public string TagPlayerAgent = "PlayerAgent";
    public string TagPlayer = "Player";
    public string TagButton = "Button";

    public float distanceMax;

    private void Awake()
    {
        buttons = GameObject.FindGameObjectsWithTag(TagButton);
        p = GameObject.FindGameObjectWithTag(TagPlayer).GetComponent<Player>();
    }

    private void Start()
    {
        foreach (GameObject item in buttons)
        {
            item.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TagPlayerAgent))
        {
            p.IsSeted = false;
            Debug.Log("Entrou");
            foreach (GameObject item in buttons)
            {
                if ((item.transform.position - transform.position).magnitude <= distanceMax)
                {
                    item.SetActive(true);
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(TagPlayerAgent))
        {
            if (p.IsSeted)
            {
                Debug.Log("Entrou Stay");
                foreach (GameObject item in buttons)
                {
                    if ((item.transform.position - transform.position).magnitude <= distanceMax)
                    {
                        item.SetActive(false);
                    }
                }
            }
       }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, distanceMax);
    }
}
