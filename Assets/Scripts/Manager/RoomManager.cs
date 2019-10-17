using System;
using System.Collections;
using System.Collections.Generic;
using larcom.MazeGenerator.Support;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Animations;

public enum TypeRoom
{
    right_key,
    hope_door,
    multiple_forms,
    true_or_false,
    end_room // End Room
}

public struct DoorStructure
{
    public string name;
    public linkDoor doorProperties;
    public int doorPosition;
    public Animator anim;
    public Transform doorTransform;
}
public class RoomManager : MonoBehaviour, IRoomSet
{
    [Header("Answer")]
    public int entranceDirection = Constants.DIRECTION_NONE;
    public Transform[] spawnAnswer, spawnDoor;
    public List<ItemBase> answerReference;
    [Tooltip("Prefabs das respostas que serão instanciados no mapa.\n 0 - Key\n 1 - Cube\n 2 - Prism\n 3 - Circle")]
    public GameObject[] answerPrefab;

    [Header("Door")]
    [Tooltip("Prefab da porta que será instanciado no mapa.")]
    public GameObject doorPrefab;
    [SerializeField] public DoorStructure[] portDatas;
    public int currentPositionCorridor;

    [Header("Wall")]
    public GameObject WallPrefab;

    [Header("UI")]
    public TextMeshProUGUI TextQuestion;

    [Header("room data")]
    public Question question;
    public int id { get => question.question_id; }
    public TypeRoom type;
    public GameManager manager;
    public string availbility = "";

    [Header("Extras")]
    public GameObject CheckFormAnswer;
    public Transform spawnCheckForm;
    public GameObject CheckLeverAnswer;
    public Transform spawnCheckLevers;

    //Metodos
    public void generateAnswers()
    {
        SetTypeRoom();

        Answer[] answers = question.answers;
        GameObject[] answerTemp = new GameObject[answers.Length];
        answerReference = new List<ItemBase>();

        List<Transform> molds = new List<Transform>(spawnAnswer);
        Tools.Shuffle(molds);

        if (type == TypeRoom.right_key)
        {
            for (int i = 0; i < answers.Length; i++)
            {
                GameObject go = Instantiate(answerPrefab[0], molds[i].position, molds[i].rotation, molds[i]);
                ItemBase ansRef = go.GetComponent<ItemBase>();
                ansRef.properties = answers[i];
                answerReference.Add(ansRef);
            }
        }
        else if (type == TypeRoom.hope_door)
        {
            for (int i = 0; i < answers.Length; i++)
            {
                GameObject go = Instantiate(answerPrefab[0], molds[i].position, molds[i].rotation, molds[i]);
                ItemBase ansRef = go.GetComponent<ItemBase>();
                ansRef.properties = answers[i];
                answerReference.Add(ansRef);
            }
        }
        else if (type == TypeRoom.multiple_forms)
        {
            List<int> count = new List<int>();
            List<int> shapes = new List<int>();
            int i = 0;
            while (count.Count < 3)
            {
                i = UnityEngine.Random.Range(0, 3);
                if (!count.Contains(i))
                {
                    int j = UnityEngine.Random.Range(1, 4);
                    if (!shapes.Contains(j))
                    {
                        GameObject go = Instantiate(answerPrefab[j], molds[i].position, molds[i].rotation, molds[i]);
                        ItemBase ansRef = go.GetComponent<ItemBase>();
                        ansRef.properties = answers[i];
                        answerReference.Add(ansRef);
                        count.Add(i);
                        shapes.Add(j);
                    }
                }
            }
        }
        else if (type == TypeRoom.true_or_false)
        {
            for (int i = 0; i < answers.Length; i++)
            {
                Vector3 newPosition = molds[i].position;
                newPosition.y += 0.5f; 
                GameObject go = Instantiate(answerPrefab[4], newPosition, molds[i].rotation, molds[i]);
                ItemBase ansRef = go.GetComponent<ItemBase>();
                ansRef.properties = answers[i];
                answerReference.Add(ansRef);
            }

            GameObject objectTemp;

            objectTemp = Instantiate(CheckLeverAnswer, spawnCheckLevers.position, spawnCheckLevers.rotation, spawnCheckLevers);
            CheckLevers checkLevers = objectTemp.transform.GetChild(0).GetComponent<CheckLevers>();
            foreach (ItemBase item in answerReference)
            {
                Lever lever = item.gameObject.GetComponent<Lever>();
                checkLevers.listLevers.Add(lever);
            }

            checkLevers.rmanager = this;

            objectTemp = null;

            checkLevers.door = portDatas[1];
        }

        TextQuestion.text = question.question;
    }

    /// <summary>
    /// Identificação e Criação da sala
    /// </summary>
    public void SetTypeRoom()
    {
        if (question.room_type == "right_key")
        {
            type = TypeRoom.right_key;
            Room_right_key();
        }
        else if (question.room_type == "hope_door")
        {
            type = TypeRoom.hope_door;
            Room_hope_door();
        }
        else if (question.room_type == "multiple_forms")
        {
            type = TypeRoom.multiple_forms;
            RoomTypeObjectsAndShapes();
        }
        else if (question.room_type == "true_or_false")
        {
            type = TypeRoom.true_or_false;
            Room_true_or_false();
        }
        else if (question.room_type == "end_room")
        {
            type = TypeRoom.end_room;
            End_Room();
        }
        else if ((int)type == 3)
            Debug.Log("Entrou no if do tipo " + type + this.gameObject.name);
    }
    #region Types_Of_Room

    
    /// <summary>
    /// Criação da sala Right Key Room
    /// </summary>
    void Room_right_key()
    {
        //Variaveis;
        GameObject objectTemp;

        DoorStructure ds = new DoorStructure();
        //Declarando o tamanho do Array
        portDatas = new DoorStructure[2];

        //Instancia da porta de entrada
        objectTemp = Instantiate(doorPrefab, spawnDoor[2].position, spawnDoor[2].rotation, spawnDoor[2]);
        objectTemp.name = "gateway";

        //Salvando as propriedades da porta de entrada
        ds.name = objectTemp.name;
        ds.doorProperties = objectTemp.GetComponent<linkDoor>();
        ds.doorPosition = 2;
        ds.anim = objectTemp.GetComponent<Animator>();
        portDatas[0] = ds;
        //Caso seja a sala final
        if (question.paths[0].end_game == true)
        {
            objectTemp = Instantiate(doorPrefab, spawnDoor[0].position, spawnDoor[0].rotation, spawnDoor[0]);
            objectTemp.name = "EndDoor";

            ds.name = objectTemp.name;
            ds.doorProperties = objectTemp.GetComponent<linkDoor>();
            ds.doorPosition = 0;
            ds.anim = objectTemp.GetComponent<Animator>();

            portDatas[1] = ds;
        }
        else
        {
            objectTemp = Instantiate(doorPrefab, spawnDoor[0].position, spawnDoor[0].rotation, spawnDoor[0]);
            objectTemp.name = "DoorAnswer";

            ds.name = objectTemp.name;
            ds.doorProperties = objectTemp.GetComponent<linkDoor>();
            ds.doorPosition = 0;
            ds.anim = objectTemp.GetComponent<Animator>();

            portDatas[1] = ds;
        }

        //Adicionando muros no cenario
        List<int> count = new List<int>();
        count.Add(portDatas[0].doorPosition);
        count.Add(portDatas[1].doorPosition);

        while (count.Count < 4)
        {
            int num = UnityEngine.Random.Range(0, 4);
            if (!count.Contains(num))
            {
                Instantiate(WallPrefab, spawnDoor[num].position, spawnDoor[num].rotation, spawnDoor[num]);
                count.Add(num);
            }
        }
    }


    /// <summary>
    /// Criação da Hope Door Room
    /// </summary>
    void Room_hope_door() {
        //Variaveis;
        GameObject objectTemp;

        DoorStructure ds = new DoorStructure();
        //Declarando o tamanho do Array
        portDatas = new DoorStructure[2];


        //Instancia da porta de entrada
        objectTemp = Instantiate(doorPrefab, spawnDoor[2].position, spawnDoor[2].rotation, spawnDoor[2]);
        objectTemp.name = "gateway";

        //Salvando as propriedades da porta de entrada
        ds.name = objectTemp.name;
        ds.doorProperties = objectTemp.GetComponent<linkDoor>();
        ds.doorPosition = 2;
        ds.anim = objectTemp.GetComponent<Animator>();
        portDatas[0] = ds;

        List<int> count = new List<int>();
        count.Add(portDatas[0].doorPosition);
        //Caso seja a sala final
        if (question.paths[0].end_game == true)
        {
            objectTemp = Instantiate(doorPrefab, spawnDoor[0].position, spawnDoor[0].rotation, spawnDoor[0]);
            objectTemp.name = "EndDoor";

            ds.name = objectTemp.name;
            ds.doorProperties = objectTemp.GetComponent<linkDoor>();
            ds.doorPosition = 0;
            ds.anim = objectTemp.GetComponent<Animator>();

            portDatas[1] = ds;
            count.Add(portDatas[1].doorPosition);
        }
        else {
            while (count.Count < 4)
            {
                int num = UnityEngine.Random.Range(0, 4);
                int i = 1;
                if (!count.Contains(num))
                {
                    objectTemp = Instantiate(doorPrefab, spawnDoor[num].position, spawnDoor[num].rotation, spawnDoor[num]);
                    objectTemp.name = "DoorAnswer";

                    ds.name = objectTemp.name;
                    ds.doorProperties = objectTemp.GetComponent<linkDoor>();
                    ds.doorPosition = 0;
                    ds.doorTransform = objectTemp.transform;

                    ds.anim = objectTemp.GetComponent<Animator>();

                    portDatas[i] = ds;
                    count.Add(num);

                    i++;
                }
            }
        }
    }

    void positionDoors()
    {
        GameObject objectTemp;
        DoorStructure ds = new DoorStructure();

        objectTemp = Instantiate(doorPrefab, spawnDoor[0].position, spawnDoor[0].rotation, spawnDoor[0]);
        objectTemp.name = "DoorAnswer";

        ds.name = objectTemp.name;
        ds.doorProperties = objectTemp.GetComponent<linkDoor>();
        ds.doorPosition = 0;
        ds.anim = objectTemp.GetComponent<Animator>();

        portDatas[1] = ds;
    }


    /// <summary>
    /// Criação da Multiple Forms Room
    /// </summary>
    void RoomTypeObjectsAndShapes() {
        //Variaveis;
        GameObject objectTemp;

        objectTemp = Instantiate(CheckFormAnswer, spawnCheckForm.position, spawnCheckForm.rotation, spawnCheckForm);
        checkShapes cs = objectTemp.GetComponent<checkShapes>();
        cs.rmanager = this;

        foreach (Answer item in question.answers)
        {
            if (item.correct)
                cs.answer = item;
        }

        objectTemp = null;

        DoorStructure ds = new DoorStructure();
        //Declarando o tamanho do Array
        portDatas = new DoorStructure[2];

        //Instancia da porta de entrada
        objectTemp = Instantiate(doorPrefab, spawnDoor[2].position, spawnDoor[2].rotation, spawnDoor[2]);
        objectTemp.name = "gateway";


        //Salvando as propriedades da porta de entrada
        ds.name = objectTemp.name;
        ds.doorProperties = objectTemp.GetComponent<linkDoor>();
        ds.doorPosition = 2;
        ds.anim = objectTemp.GetComponent<Animator>();
        portDatas[0] = ds;

        //Caso seja a sala final
        if (question.paths[0].end_game == true)
        {
            objectTemp = Instantiate(doorPrefab, spawnDoor[0].position, spawnDoor[0].rotation, spawnDoor[0]);
            objectTemp.name = "EndDoor";

            ds.name = objectTemp.name;
            ds.doorProperties = objectTemp.GetComponent<linkDoor>();
            ds.doorPosition = 0;
            ds.anim = objectTemp.GetComponent<Animator>();

            portDatas[1] = ds;
            cs.door = portDatas[1];
        }
        else
        {
            objectTemp = Instantiate(doorPrefab, spawnDoor[0].position, spawnDoor[0].rotation, spawnDoor[0]);
            objectTemp.name = "DoorAnswer";

            ds.name = objectTemp.name;
            ds.doorProperties = objectTemp.GetComponent<linkDoor>();
            ds.doorPosition = 0;
            ds.anim = objectTemp.GetComponent<Animator>();

            portDatas[1] = ds;
            cs.door = portDatas[1];
        }

        //Adicionando muros no cenario
        List<int> count = new List<int>();
        count.Add(portDatas[0].doorPosition);
        count.Add(portDatas[1].doorPosition);

        while (count.Count < 4)
        {
            int num = UnityEngine.Random.Range(0, 4);
            if (!count.Contains(num))
            {
                Instantiate(WallPrefab, spawnDoor[num].position, spawnDoor[num].rotation, spawnDoor[num]);
                count.Add(num);
            }
        }
    }


    /// <summary>
    /// Criação da sala verdadeiro e falso
    /// </summary>
    void Room_true_or_false()
    {
        //Variaveis;
        GameObject objectTemp;

        DoorStructure ds = new DoorStructure();
        //Declarando o tamanho do Array
        portDatas = new DoorStructure[2];

        //Instancia da porta de entrada
        objectTemp = Instantiate(doorPrefab, spawnDoor[2].position, spawnDoor[2].rotation, spawnDoor[2]);
        objectTemp.name = "gateway";

        //Salvando as propriedades da porta de entrada
        ds.name = objectTemp.name;
        ds.doorProperties = objectTemp.GetComponent<linkDoor>();
        ds.doorPosition = 2;
        ds.anim = objectTemp.GetComponent<Animator>();
        portDatas[0] = ds;

        objectTemp = Instantiate(doorPrefab, spawnDoor[0].position, spawnDoor[0].rotation, spawnDoor[0]);

        //Caso seja a sala final
        if (question.paths[0].end_game == true)
        {  
            objectTemp.name = "EndDoor";
        }
        else
        {
            objectTemp.name = "DoorAnswer";
        }

        ds.name = objectTemp.name;
        ds.doorProperties = objectTemp.GetComponent<linkDoor>();
        ds.doorPosition = 0;
        ds.anim = objectTemp.GetComponent<Animator>();

        portDatas[1] = ds;

        //Adicionando muros no cenario
        List<int> count = new List<int>();
        count.Add(portDatas[0].doorPosition);
        count.Add(portDatas[1].doorPosition);

        while (count.Count < 4)
        {
            int num = UnityEngine.Random.Range(0, 4);
            if (!count.Contains(num))
            {
                Instantiate(WallPrefab, spawnDoor[num].position, spawnDoor[num].rotation, spawnDoor[num]);
                count.Add(num);
            }
        }
    }

    /// <summary>
    /// Criação da End Room
    /// </summary>
    void End_Room()
    {
        //TODO Items de Recompensa
        GameObject objectTemp;

        objectTemp = Instantiate(CheckFormAnswer, spawnCheckForm.position, spawnCheckForm.rotation, spawnCheckForm);
        checkShapes cs = objectTemp.GetComponent<checkShapes>();
        cs.rmanager = this;

        objectTemp = null;

        DoorStructure ds = new DoorStructure();
        //Declarando o tamanho do Array
        portDatas = new DoorStructure[2];

        //Instancia da porta de entrada
        objectTemp = Instantiate(doorPrefab, spawnDoor[2].position, spawnDoor[2].rotation, spawnDoor[2]);
        objectTemp.name = "gateway";

        //Salvando as propriedades da porta de entrada
        ds.name = objectTemp.name;
        ds.doorProperties = objectTemp.GetComponent<linkDoor>();
        ds.doorPosition = 2;
        ds.anim = objectTemp.GetComponent<Animator>();
        portDatas[0] = ds;


        //Adicionando muros no cenario
        List<int> count = new List<int>();
        count.Add(portDatas[0].doorPosition);

        for (int i = 0; i < 4; i++)
        {
            if (!count.Contains(i))
            {
                Instantiate(WallPrefab, spawnDoor[i].position, spawnDoor[i].rotation, spawnDoor[i]);
                count.Add(i);
            }
        }
           
    }

    #endregion

    /// <summary>
    /// Posiciona a próxima sala junto com o corredor.
    /// </summary>
    /// <param name="nameDoor"></param>
    /// <param name="checkAnswer"></param>
    public void PositionNextRoom(string nameDoor ,bool checkAnswer)
    {
        if(spawnDoor[currentPositionCorridor].gameObject.GetComponentInChildren<Animator>().GetBool("open"))
        {
            spawnDoor[currentPositionCorridor].gameObject.GetComponentInChildren<Animator>().SetTrigger("closing");
		}
		this.GetComponentInChildren<HubCheckpoint>().clearGoals();

		int door = 0;
        foreach (DoorStructure item in portDatas)
        {
            if (item.name == nameDoor)
            {
                door = item.doorPosition;
            }
        }

        int pathLength = question.paths.Length;

        int dir = Constants.DIRECTIONS[door];
        int path = 0;
        if (pathLength > 1)
        {
            Debug.Log("Entrou aqui");

            while ((door == currentPositionCorridor) || (door == 2))
            {
                door = UnityEngine.Random.Range(1,4);
            }
            currentPositionCorridor = door;
            dir = Constants.DIRECTIONS[door];

            path = setNextRoom(checkAnswer);

            Debug.Log("Avaibility chamado é: " + question.paths[path].availability);
            CorridorManager[] corridors = GameManager.Instance.getCorridorsByRoom(this.id);
            StartCoroutine(GameManager.Instance.placeNextCorridor(this.spawnDoor[door].position, this.transform.rotation, dir, corridors[path]));

            spawnDoor[door].gameObject.GetComponentInChildren<Animator>().SetTrigger("openning");

            SetDoorAnswer(path);
        }
        else
        {
            CorridorManager[] corridors = GameManager.Instance.getCorridorsByRoom(this.id);

            StartCoroutine(GameManager.Instance.placeNextCorridor(this.spawnDoor[door].position, this.transform.rotation, dir, corridors[path]));
            SetDoorAnswer(path);
            spawnDoor[door].gameObject.GetComponentInChildren<Animator>().SetTrigger("openning");
        }
    }

    int setNextRoom(bool check)
    {
        for (int i = 0; i < question.paths.Length; i++)
        {
            if (check && question.paths[i].availability == "right")
            {
                return i;
            }
            else if (!check && question.paths[i].availability == "wrong")
            {
                return i;
            }
        }
        return 0;
    }

    /// <summary>
    /// Coloca a resposta da pergunta na porta.
    /// </summary>
    /// <param name="path">index do caminho</param>
    public void SetDoorAnswer(int path)
    {
        int pathLength = question.paths.Length;
        if (question.paths[0].end_game == false)
        {
            if (pathLength > 1)
            {
                foreach (RoomManager room in GameManager.Instance.roomsObjects)
                {
                    if (room.id == question.paths[path].connected_question)
                    {
                        StartCoroutine(setGateway(room));
                        return;
                    }
                }
            }
            else if (question.room_type == "true_or_false")
            {
                foreach (RoomManager room in GameManager.Instance.roomsObjects)
                {
                    if (room.id == question.paths[path].connected_question)
                    {
                        linkDoor linked = room.portDatas[0].doorProperties;

                        linked.answerLinked = question.answers[0];
                    }
                }
            }
            else
            {
                Debug.Log("Entrou no else");
                foreach (RoomManager room in GameManager.Instance.roomsObjects)
                {
                    if (room.id == question.paths[path].connected_question)
                    {
                        linkDoor linked = room.portDatas[0].doorProperties;

                        foreach (Answer ans in question.answers)
                        {
                            if (ans.correct)
                            {
                                linked.answerLinked = ans;
                            }
                        }

                    }
                }
            }
        }
        else
        {
            Debug.Log("Entrou no End Room");

            if (question.room_type == "true_or_false")
            {
                RoomManager room = GameManager.Instance.roomsObjects[GameManager.Instance.roomsObjects.Length - 1]; // End Room

                linkDoor linked = room.portDatas[0].doorProperties;

                linked.answerLinked = question.answers[0];
            }
            else
            {
                RoomManager room = GameManager.Instance.roomsObjects[GameManager.Instance.roomsObjects.Length - 1]; // End Room

                linkDoor linked = room.portDatas[0].doorProperties;

                foreach (Answer ans in question.answers)
                {
                    if (ans.correct)
                    {
                        linked.answerLinked = ans;
                    }
                }

            }
           
        }

    }

    IEnumerator setGateway(RoomManager room)
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("Entrou no if do room id");
        linkDoor linked = room.portDatas[0].doorProperties;
        Debug.Log("A sala que foi pego é a " + room.portDatas[0].name);
        Debug.Log(Inventory.instance.AnswerSelected.answer);
        linked.answerLinked = Inventory.instance.AnswerSelected;
    }
}

public interface IRoomSet
{
    //void setNextDoor();
    void PositionNextRoom(String nameDoor, bool checkAnswer);
    void generateAnswers();
    void SetTypeRoom();
}
