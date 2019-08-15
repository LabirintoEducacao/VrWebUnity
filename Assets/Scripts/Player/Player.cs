using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;

public class Player : PlayerBase
{
    [Header("Components")]
    public Image GUIReticleLoad;
    public ExitButton exit;
    public RoomManager currentRoom;

    [Header("Variables")]
    public float currentTimeUnlock;
    public float timeToUnlock;
    public float timeToLoadFillAmount;
    public float currentTimeLoadFillAmount;
    public bool IsSeted;
    public SetTarget _SetTarget;
    private void Start()
    {
        GUIReticleLoad.gameObject.SetActive(false);
    }

    private void Update() {
        Ray ray = new Ray(this.transform.position, transform.forward);
        RaycastHit hit;

        //Metodos para detecção
        if (Physics.Raycast(ray.origin, ray.direction, out hit))
        {
            // Debug.Log(hit.collider.tag);
            if (hit.collider.CompareTag("Button"))
            {
                hitArrow(hit);
            }
            else if (hit.collider.CompareTag("Item"))
            {
                hitItem(hit);
            }
            else if (hit.collider.CompareTag("Door"))
            {
                hitCheckAnswer(hit);
            }
            else
            {
                GUIReticleLoad.fillAmount = 0;
                GUIReticleLoad.gameObject.SetActive(false);

                currentTimeLoadFillAmount = 0;
                currentTimeUnlock = 0;
            }
        }
        else {
                GUIReticleLoad.fillAmount = 0;
                GUIReticleLoad.gameObject.SetActive(false);

                currentTimeLoadFillAmount = 0;
                currentTimeUnlock = 0;
        }
        Debug.DrawRay(ray.origin, ray.direction, Color.red);
    }

    #region Hit Detection
    void hitArrow(RaycastHit hit){
        currentTimeUnlock += Time.deltaTime;
        if (currentTimeUnlock >= timeToUnlock){
            GUIReticleLoad.gameObject.SetActive(true);
            currentTimeLoadFillAmount += Time.deltaTime;
            GUIReticleLoad.fillAmount = (currentTimeLoadFillAmount / timeToLoadFillAmount);

            _SetTarget = hit.collider.gameObject.GetComponent<SetTarget>();
            if(_SetTarget && currentTimeLoadFillAmount >+ timeToLoadFillAmount){
                //foi acionado, mandando o agent se mexer e reiniciando as variáveis.
                SetTarget(_SetTarget.Target.position);
                _SetTarget.select();
                
                
                currentTimeLoadFillAmount = 0;
                currentTimeUnlock = 0;
            }

        }
    }

    void hitItem(RaycastHit hit){
        currentTimeUnlock += Time.deltaTime;
        if (currentTimeUnlock >= timeToUnlock){
            GUIReticleLoad.gameObject.SetActive(true);
            currentTimeLoadFillAmount += Time.deltaTime;
            GUIReticleLoad.fillAmount = (currentTimeLoadFillAmount / timeToLoadFillAmount);

            //Salvando posição do parente do item para que possa devolve-lo no lugar desejado;
            GameObject parentFuture, parentActual;
            parentFuture = hit.collider.gameObject.transform.parent.gameObject;
            Transform pos = parentFuture.transform;

            //Caso o inventário esteja vazio
            if(Inventory.instance.item == null && currentTimeLoadFillAmount >= timeToLoadFillAmount){
                // Passando o item selecionado para o inventário

                Inventory.instance.item = hit.collider.gameObject.GetComponentInParent<ItemBase>();
                Inventory.instance.item.currentRoom = currentRoom;
                Inventory.instance.item.ActionItem();
                Inventory.instance.ItemObject = Inventory.instance.item.gameObject;
                Inventory.instance.ItemObject.gameObject.SetActive(false);

                GUIReticleLoad.fillAmount = 0;
                GUIReticleLoad.gameObject.SetActive(false);
                currentTimeLoadFillAmount = 0;
                currentTimeUnlock = 0;
            }
            //Caso já tenha algo no inventário
            else if(Inventory.instance.item != null && currentTimeLoadFillAmount >= timeToLoadFillAmount){
                parentActual = Inventory.instance.ItemObject.transform.parent.gameObject;
                Inventory.instance.item.ActionItem();
                parentActual.transform.position = pos.position;
                parentActual.transform.rotation = pos.rotation;
                Inventory.instance.ItemObject.SetActive(true);
                Inventory.instance.item.DesactivePanel();

                //Pegando o novo objeto
                Inventory.instance.item = hit.collider.gameObject.GetComponentInParent<ItemBase>();
                Inventory.instance.item.currentRoom = currentRoom;
                Inventory.instance.ItemObject = Inventory.instance.item.gameObject;
                Inventory.instance.ItemObject.gameObject.SetActive(false);

                GUIReticleLoad.fillAmount = 0;
                GUIReticleLoad.gameObject.SetActive(false);
                currentTimeLoadFillAmount = 0;
                currentTimeUnlock = 0;

            }
        }
    }

    void hitCheckAnswer(RaycastHit hit){
        CheckBase checkDoor = hit.collider.GetComponent<CheckBase>();

        currentTimeUnlock += Time.deltaTime;
        if(checkDoor != null){
            if (Inventory.instance.item != null && currentTimeUnlock >= timeToUnlock && !checkDoor.openDoor)
            {
                GUIReticleLoad.gameObject.SetActive(true);
                currentTimeLoadFillAmount += Time.deltaTime;
                GUIReticleLoad.fillAmount = (currentTimeLoadFillAmount / timeToLoadFillAmount);

                CheckBase objectCheck = hit.collider.gameObject.GetComponent<CheckBase>();

                if (currentTimeLoadFillAmount >= timeToLoadFillAmount)
                {
                    bool correct = objectCheck.checkAnswer(Inventory.instance.AnswerSelected);
                    if(correct){
                        Debug.Log("Resposta Certa!");
                        Inventory.instance.item = null;

                        GUIReticleLoad.fillAmount = 0;
                        GUIReticleLoad.gameObject.SetActive(false);

                        currentTimeLoadFillAmount = 0;
                        currentTimeUnlock = 0;
                    } else
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
    }
    #endregion
    internal override void SetTarget(Vector3 target)
    {
        IsSeted = true;
        agent.SetDestination(target);
    }
}
