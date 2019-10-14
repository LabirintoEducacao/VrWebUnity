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

    public static Player instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    private void Start()
    {
        GUIReticleLoad.gameObject.SetActive(false);
    }

    private void Update()
    {
        Ray ray = new Ray(this.transform.position, transform.forward);
        RaycastHit hit;

        //Metodos para detecção
        if (Physics.Raycast(ray.origin, ray.direction, out hit))
        {
            Debug.Log(hit.collider.tag);
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
            else if (hit.collider.CompareTag("BackToMenu"))
            {
                hitBackToMenu(hit);
            }
            else if (hit.collider.CompareTag("ExitGame"))
            {
                hitExitGame(hit);
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
    }

    #region Hit Detection
    void hitArrow(RaycastHit hit)
    {
        currentTimeUnlock += Time.deltaTime;
        if (currentTimeUnlock >= timeToUnlock)
        {
            GUIReticleLoad.gameObject.SetActive(true);
            currentTimeLoadFillAmount += Time.deltaTime;
            GUIReticleLoad.fillAmount = (currentTimeLoadFillAmount / timeToLoadFillAmount);

            _SetTarget = hit.collider.gameObject.GetComponent<SetTarget>();
            if (_SetTarget && currentTimeLoadFillAmount > +timeToLoadFillAmount)
            {
                //foi acionado, mandando o agent se mexer e reiniciando as variáveis.
                SetTarget(_SetTarget.Target.position);
                _SetTarget.select();


                currentTimeLoadFillAmount = 0;
                currentTimeUnlock = 0;
            }

        }
    }

    /// <summary>
    /// Verifica e analisa o objeto com tag "Item" visto pelo o jogador.
    /// A verificação ocorre, quando o jogador fica olhando o objeto por um determinado tempo.
    /// Após o tempo, o objeto é colocado no inventário e caso já esteja com um outro objeto no inventário, eles são trocados.
    /// No final a resposta contida no objeto é salva.
    /// </summary>
    /// <param name="hit">Objeto "Item" visto pelo jogador</param>
    void hitItem(RaycastHit hit){
        currentTimeUnlock += Time.deltaTime;
        if (currentTimeUnlock >= timeToUnlock)
        {
            GUIReticleLoad.gameObject.SetActive(true);
            currentTimeLoadFillAmount += Time.deltaTime;
            GUIReticleLoad.fillAmount = (currentTimeLoadFillAmount / timeToLoadFillAmount);

            //Salvando posição do parente do item para que possa devolve-lo no lugar desejado;
            GameObject parentFuture, parentActual;
            parentFuture = hit.collider.gameObject.transform.parent.gameObject;
            Transform pos = parentFuture.transform;

            //Caso o inventário esteja vazio
            if (currentTimeLoadFillAmount >= timeToLoadFillAmount)
            {
                //Caso já tenha algo no inventário, trocar
                if (Inventory.instance.item != null)
                {
                    parentActual = Inventory.instance.ItemObject.transform.parent.gameObject;
                    parentActual.transform.position = pos.position;
                    parentActual.transform.rotation = pos.rotation;
                    Inventory.instance.ItemObject.SetActive(true);
                    Inventory.instance.item.DesactivePanel();
                }
                //Pegando o novo objeto
                Inventory.instance.item = null;
                Inventory.instance.item = hit.collider.gameObject.GetComponentInParent<ItemBase>();
                Inventory.instance.item.currentRoom = currentRoom;
                ItemBase item = hit.collider.gameObject.GetComponentInParent<ItemBase>();
                item.ActionItem();
                EventPool.sendAnswerInteractionEvent(item.properties.answer_id, item.properties.correct);
                Inventory.instance.ItemObject = Inventory.instance.item.gameObject;
                Inventory.instance.ItemObject.gameObject.SetActive(false);

                GUIReticleLoad.fillAmount = 0;
                GUIReticleLoad.gameObject.SetActive(false);
                currentTimeLoadFillAmount = 0;
                currentTimeUnlock = 0;


                //DataManager.manager.answerStatus(this.currentRoom.question.question_id, Inventory.instance.AnswerSelected.correct);
            }
        }
    }

    /// <summary>
    /// Verifica a porta com a resposta
    /// </summary>
    /// <param name="hit"></param>
    void hitCheckAnswer(RaycastHit hit){
        CheckBase checkDoor = hit.collider.GetComponent<CheckBase>();

        currentTimeUnlock += Time.deltaTime;
        if (checkDoor != null)
        {
            //Debug.Log("Hit no object " + checkDoor.gameObject.name);
            if (Inventory.instance.item != null && currentTimeUnlock >= timeToUnlock && checkDoor.answer.answer != null)
            {
                GUIReticleLoad.gameObject.SetActive(true);
                currentTimeLoadFillAmount += Time.deltaTime;
                GUIReticleLoad.fillAmount = (currentTimeLoadFillAmount / timeToLoadFillAmount);


                if (currentTimeLoadFillAmount >= timeToLoadFillAmount)
                {
                    bool correct = checkDoor.checkAnswer(Inventory.instance.AnswerSelected);
                    //dispara evento para registrar a resposta no analytics
                    DataManager.manager.answerStatus(this.currentRoom.id, correct);
                    if (correct)
                    {
                        Debug.Log("Resposta Certa!");

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
    }

    void hitBackToMenu(RaycastHit hit)
    {
        MenuInGame menu = hit.collider.GetComponent<MenuInGame>();
        if (menu != null)
            Debug.Log("Menu não está vazio!");

        currentTimeUnlock += Time.deltaTime;
        if (currentTimeUnlock >= timeToUnlock)
        {
            GUIReticleLoad.gameObject.SetActive(true);
            currentTimeLoadFillAmount += Time.deltaTime;
            GUIReticleLoad.fillAmount = (currentTimeLoadFillAmount / timeToLoadFillAmount);
            if (currentTimeLoadFillAmount >= timeToLoadFillAmount)
            {
                menu.backToMenu("MainMenu");
            }
        }
    }

    void hitExitGame(RaycastHit hit)
    {
        MenuInGame menu = hit.collider.GetComponent<MenuInGame>();
        if (menu != null)
            Debug.Log("Menu não está vazio!");
        currentTimeUnlock += Time.deltaTime;
        if (currentTimeUnlock >= timeToUnlock)
        {
            GUIReticleLoad.gameObject.SetActive(true);
            currentTimeLoadFillAmount += Time.deltaTime;
            GUIReticleLoad.fillAmount = (currentTimeLoadFillAmount / timeToLoadFillAmount);
            if (currentTimeLoadFillAmount >= timeToLoadFillAmount)
            {
                menu.quitGame();
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
