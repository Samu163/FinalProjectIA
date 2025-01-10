using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    public Transform spawnPointPhase2;
    public GameObject phase2Gate;

    public FSM player;
    public CinemachineVirtualCamera cameraPhase1;
    public CinemachineVirtualCamera cameraPhase2;
    public ZombieSpawner zombieSpawner;
    public GroundNavigation groundNavigation;
    public BoxInstantiator boxInstantiator;
    public CivilianInstantiator civilianInstantiator;


    public static GameManager Instance;
    //public GameObject cameraPhase1;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartPhase1()
    {
    }


    public void StartPhase2()
    {
        player.transform.position = spawnPointPhase2.position;
        civilianInstantiator.InstantiateCivilians(5);
        zombieSpawner.SpawnZombies();
    }

    public void GoMainMenu()
    {

    }

    public void StartGame()
    {

    }

}
