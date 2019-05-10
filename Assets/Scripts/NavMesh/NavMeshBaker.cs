using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshBaker : MonoBehaviour
{
    [SerializeField] private NavMeshSurface[] navMeshSurfaces;
    [SerializeField] private GameObject mapRoot;

    [Header("Player")]
    [SerializeField] private GameObject playerObj;
    [SerializeField] private string TagPlayer = "PlayerAgent";

    public bool mapBaked = false;

    private void Awake() {
        playerObj = GameObject.FindWithTag(TagPlayer);
        playerObj.SetActive(false);
    }

    private void Start() {
        getComponents();
    }

    private void getComponents(){
        mapRoot = GameObject.Find("MapRoot");
        getNavMesh();
    }

    private void getNavMesh(){
        navMeshSurfaces = mapRoot.GetComponentsInChildren<NavMeshSurface>();
        if(navMeshSurfaces != null){
            CreateBake();
        }
    }

    private void CreateBake(){
        for(int i = 0 ; i < navMeshSurfaces.Length; i++){
            navMeshSurfaces[i].BuildNavMesh();
            if(i == (navMeshSurfaces.Length - 1))
                playerObj.SetActive(true);
        }
    }
}
