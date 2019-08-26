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
    true_or_false
}

public struct DoorStructure
{
    public string name;
    public linkDoor doorProperties;
    public int doorPosition;
    public Animator anim;
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

    //Metodos
    public void generateAnswers()
    {
        SetTypeRoom();
        Answer[] answers = question.answers;
        GameObject[] answerTemp = new GameObject[answers.Length];
        answerReference = new List<ItemBase>();

        List<Transform> molds = new List<Transform>(spawnAnswer);
        Tools.Shuffle(molds);

        if(type == TypeRoom.right_key){
            for (int i = 0; i < answers.Length; i++)
            {
                GameObject go = Instantiate(answerPrefab[0], molds[i].position, molds[i].rotation, molds[i]);
                ItemBase ansRef = go.GetComponent<ItemBase>();
                ansRef.properties = answers[i];
                answerReference.Add(ansRef);
            }
        } else if (type == TypeRoom.hope_door){
            for (int i = 0; i < answers.Length; i++)
            {
                GameObject go = Instantiate(answerPrefab[0], molds[i].position, molds[i].rotation, molds[i]);
                ItemBase ansRef = go.GetComponent<ItemBase>();
                ansRef.properties = answers[i];
                answerReference.Add(ansRef);
            }
        } else if(type == TypeRoom.multiple_forms){
            List<int> count = new List<int>();
            List<int> shapes = new List<int>();
            int i = 0;
            while(count.Count < 3){
                i = UnityEngine.Random.Range(0, 3);
                if(!count.Contains(i)){
                    int j = UnityEngine.Random.Range(1, 4);
                    if(!shapes.Contains(j)){
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

        TextQuestion.text = question.question;
    }

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
        else if ((int)type == 3)
            Debug.Log("Entrou no if do tipo " + type + this.gameObject.name);
    }
#region Types_Of_Room
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

    void Room_hope_door(){
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
        else{
            objectTemp = Instantiate(doorPrefab, spawnDoor[0].position, spawnDoor[0].rotation, spawnDoor[0]);
            objectTemp.name = "DoorAnswer";

            ds.name = objectTemp.name;
            ds.doorProperties = objectTemp.GetComponent<linkDoor>();
            ds.doorPosition = 0;
            ds.anim = objectTemp.GetComponent<Animator>();

            portDatas[1] = ds;
            count.Add(portDatas[1].doorPosition);

            while (count.Count < 4)
            {
                int num = UnityEngine.Random.Range(0, 4);
                int i = 0;
                if (!count.Contains(num))
                {
                    objectTemp = Instantiate(doorPrefab, spawnDoor[num].position, spawnDoor[num].rotation, spawnDoor[num]);
                    objectTemp.name = "DoorAnswer";
                    i++;
                    count.Add(num);
                }
            }
        }
    }

    void RoomTypeObjectsAndShapes(){
        //Variaveis;
        GameObject objectTemp;

        objectTemp = Instantiate(CheckFormAnswer, spawnCheckForm.position, spawnCheckForm.rotation, spawnCheckForm);
        checkShapes cs= objectTemp.GetComponent<checkShapes>();
        cs.rmanager = this;
        
        foreach (Answer item in question.answers)
        {
            if(item.correct)
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
#endregion

    public void PositionNextRoom(string nameDoor ,bool checkAnswer)
    {
        int door = 0; //= UnityEngine.Random.Range(0,4);
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
            foreach(MazePath p in question.paths)
            {
                if (checkAnswer && p.availability == "right")
                {
                    availbility = p.availability;
                    path = 0;
                }
                if(!checkAnswer && p.availability == "wrong")
                {
                    availbility = p.availability;
                    path = 1;
                }
            }

            CorridorManager[] corridors = GameManager.Instance.getCorridorsByRoom(this.id);
            StartCoroutine(GameManager.Instance.placeNextCorridor(this.spawnDoor[door].position, this.transform.rotation, 
                dir, corridors[path]));

            SetDoorAnswer(path);
        }
        else
        {
            CorridorManager[] corridors = GameManager.Instance.getCorridorsByRoom(this.id);

            StartCoroutine(GameManager.Instance.placeNextCorridor(this.spawnDoor[door].position, this.transform.rotation, dir, corridors[0]));
            SetDoorAnswer(0);
        }
    }

    public void SetDoorAnswer(int path)
    {
        int pathLength = question.paths.Length;
        if (question.paths[0].end_game == false)
        {
            if (pathLength > 1)
            {
                Debug.Log("Entrou no if");

                foreach (RoomManager room in GameManager.Instance.roomsObjects)
                {
                    if (room.id == question.paths[path].connected_question)
                    {
                        linkDoor linked = room.portDatas[0].doorProperties;
                        Answer ans = Inventory.instance.AnswerSelected;
                        Debug.Log(ans);
                        linked.answerLinked = ans;
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

    }
}

public interface IRoomSet
{
    //void setNextDoor();
    void PositionNextRoom(String nameDoor, bool checkAnswer);
    void generateAnswers();
    void SetTypeRoom();
}
