using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;

public class Player : PlayerBase
{
    [Header("Components")]
    public GameObject button;
    public Image GUIReticleLoad;
    public ExitButton exit;
    public Inventory inventory;
    public RoomManager currentRoom;

    [Header("Variables")]
    public float currentTimeUnlock;
    public float timeToUnlock;
    public float timeToLoadFillAmount;
    public float currentTimeLoadFillAmount;
    public bool IsSeted;

    private void Start()
    {
        GUIReticleLoad.gameObject.SetActive(false);
    }

    public SetTarget st;

    private void Update()
    {
        Ray ray = new Ray(this.transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray.origin, ray.direction, out hit))
        {
            // Debug.Log(hit.collider.tag);
            if (hit.collider.CompareTag("Button"))
            {
                currentTimeUnlock += Time.deltaTime;
                if (currentTimeUnlock >= timeToUnlock)
                {
                    GUIReticleLoad.gameObject.SetActive(true);
                    currentTimeLoadFillAmount += Time.deltaTime;
                    GUIReticleLoad.fillAmount = (currentTimeLoadFillAmount / timeToLoadFillAmount);
                    button = hit.collider.gameObject;
                    st = button.GetComponent<SetTarget>();
                    if (st != null & currentTimeLoadFillAmount >= timeToLoadFillAmount)
                    {
                        //foi acionado, mandando o agent se mexer e reiniciando as variáveis
                        SetTarget(st.Target.position);
                        st.select();
                        GUIReticleLoad.fillAmount = 0;
                        GUIReticleLoad.gameObject.SetActive(false);

                        currentTimeLoadFillAmount = 0;
                        currentTimeUnlock = 0;
                    }
                }
            }
            else if (hit.collider.CompareTag("ButtonExit"))
            {
                currentTimeUnlock += Time.deltaTime;
                if (currentTimeUnlock >= timeToUnlock)
                {
                    GUIReticleLoad.gameObject.SetActive(true);
                    currentTimeLoadFillAmount += Time.deltaTime;
                    GUIReticleLoad.fillAmount = (currentTimeLoadFillAmount / timeToLoadFillAmount);
                    button = hit.collider.gameObject;
                    ExitButton exit = button.GetComponent<ExitButton>();
                    exit = button.GetComponent<ExitButton>();

                    if (currentTimeLoadFillAmount >= timeToLoadFillAmount)
                    {
                        //foi acionado, mandando o agent se mexer e reiniciando as variáveis
                        exit.desactivePanel();

                        GUIReticleLoad.fillAmount = 0;
                        GUIReticleLoad.gameObject.SetActive(false);

                        currentTimeLoadFillAmount = 0;
                        currentTimeUnlock = 0;
                    }
                }
            }
            else if (hit.collider.CompareTag("Item"))
            {
                currentTimeUnlock += Time.deltaTime;
                if (currentTimeUnlock >= timeToUnlock)
                {
                    GUIReticleLoad.gameObject.SetActive(true);
                    currentTimeLoadFillAmount += Time.deltaTime;
                    GUIReticleLoad.fillAmount = (currentTimeLoadFillAmount / timeToLoadFillAmount);

                    GameObject parentFuture, parentActual;
                    button = hit.collider.gameObject;
                    parentFuture = button.transform.parent.gameObject;
                    Transform pos = parentFuture.transform;
                    

                    if (inventory.item != null && currentTimeLoadFillAmount >= timeToLoadFillAmount)
                    {
                        parentActual = inventory.ItemObject.transform.parent.gameObject;

                        parentActual.transform.position = pos.position;
                        parentActual.transform.rotation = pos.rotation;

                        inventory.ItemObject.SetActive(true);
                         
                        inventory.item.DesactivePanel();

                        inventory.item = button.GetComponentInParent<AnswerReference>();
                        inventory.ItemObject = inventory.item.gameObject;

                        inventory.ItemObject.gameObject.SetActive(false);
                        
                        GUIReticleLoad.fillAmount = 0;
                        GUIReticleLoad.gameObject.SetActive(false);

                        currentTimeLoadFillAmount = 0;
                        currentTimeUnlock = 0;
                        foreach (Animator item in currentRoom.anims)
                        {
                            item.SetTrigger("openning");
                        }
                    }
                    else if(inventory.item == null && currentTimeLoadFillAmount >= timeToLoadFillAmount)
                    {
                        inventory.item = button.GetComponentInParent<AnswerReference>();
                        inventory.ItemObject = inventory.item.gameObject;

                        inventory.ItemObject.gameObject.SetActive(false);

                        GUIReticleLoad.fillAmount = 0;
                        GUIReticleLoad.gameObject.SetActive(false);

                        currentTimeLoadFillAmount = 0;
                        currentTimeUnlock = 0;

                        foreach (Animator item in currentRoom.anims)
                        {
                            item.SetTrigger("openning");
                        }
                    }
                }
            }
            else if (hit.collider.CompareTag("Door"))
            {
                Door checkDoor = hit.collider.GetComponent<Door>();

                currentTimeUnlock += Time.deltaTime;
                if (inventory.item != null && currentTimeUnlock >= timeToUnlock && !checkDoor.openDoor)
                {
                    GUIReticleLoad.gameObject.SetActive(true);
                    currentTimeLoadFillAmount += Time.deltaTime;
                    GUIReticleLoad.fillAmount = (currentTimeLoadFillAmount / timeToLoadFillAmount);

                    button = hit.collider.gameObject;

                    Door door = button.GetComponent<Door>();

                    if (currentTimeLoadFillAmount >= timeToLoadFillAmount)
                    {
                        if (door.AnswerCorrect == inventory.item.properties)
                        {
                            Debug.Log("Resposta Certa!");
                            door.openDoor = true;
                            inventory.item = null;

                            GUIReticleLoad.fillAmount = 0;
                            GUIReticleLoad.gameObject.SetActive(false);

                            currentTimeLoadFillAmount = 0;
                            currentTimeUnlock = 0;
                        }
                        else
                        {
                            Debug.Log("Resposta errada!");

                            GUIReticleLoad.fillAmount = 0;
                            GUIReticleLoad.gameObject.SetActive(false);

                            currentTimeLoadFillAmount = 0;
                            currentTimeUnlock = 0;
                        }
                    }
                }
            }
            else
            {
                GUIReticleLoad.fillAmount = 0;
                GUIReticleLoad.gameObject.SetActive(false);

                currentTimeLoadFillAmount = 0;
                currentTimeUnlock = 0;
            }
        }
        else
        {
            GUIReticleLoad.fillAmount = 0;
            GUIReticleLoad.gameObject.SetActive(false);

            currentTimeLoadFillAmount = 0;
            currentTimeUnlock = 0;
        }
        Debug.DrawRay(ray.origin, ray.direction, Color.red);
        // if(hit.collider != null)
        //     Debug.Log(hit.collider.name);
    }


internal override void SetTarget(Vector3 target)
    {
        IsSeted = true;
        agent.SetDestination(target);
    }
}
