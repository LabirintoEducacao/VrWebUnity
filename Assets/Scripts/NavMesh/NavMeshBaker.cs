using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshBaker : MonoBehaviour
{
    [SerializeField] private NavMeshSurface[] navMeshSurfaces;
    [SerializeField] private GameObject mapRoot;

    [Header("Player")]
    [SerializeField] private string TagPlayer = "PlayerAgent";

    void Start() {
        CreateBake();
    }

    private void getComponents(){
        if (mapRoot == null)
            mapRoot = GameObject.Find("MapRoot");
        if (mapRoot == null) {
            Debug.LogError("Cannot deal with NavMesh if there is object named MapRoot on the scene");
            return;
        }

        // getNavMesh();
    }

    private void getNavMesh(){
        navMeshSurfaces = mapRoot.GetComponentsInChildren<NavMeshSurface>(false);
        if(navMeshSurfaces != null){
            CreateBake();
        }
    }

    public void CreateBake(){
        getComponents();
        GameObject playerObj = GameObject.FindGameObjectWithTag(TagPlayer);
        playerObj.SetActive(false);
        mapRoot.GetComponent<NavMeshSurface>().BuildNavMesh();
        playerObj.SetActive(true);
        // for(int i = 0 ; i < navMeshSurfaces.Length; i++){
        //     // navMeshSurfaces[i].RemoveData();
        //     navMeshSurfaces[i].BuildNavMesh();
        //     if(i == (navMeshSurfaces.Length - 1))
        //         playerObj.SetActive(true);
        // }
    }
}
