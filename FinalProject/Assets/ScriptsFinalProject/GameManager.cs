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
    public BoxInstantiator boxInstantiator;

    public static GameManager Instance;

    private Camera[] allSceneCameras;

    public UiController uiController;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        StartPhase1();
    }

    void Update()
    {

    }

    public void StartPhase1()
    {
        ActivateCamera(cameraPhase1);
    }

    public void StartPhase2()
    {
        player.transform.position = spawnPointPhase2.position;

        zombieSpawner.SpawnZombies();
        ActivateCamera(cameraPhase2);
    }

    public void WinGame()
    {
        uiController.ShowYouWinPopUp();
    }
    public void LoseGame()
    {
        uiController.ShowLosePopUp();
    }

    private void ActivateCamera(CinemachineVirtualCamera activeCamera)
    {
        allSceneCameras = Camera.allCameras;
        DisableNonCinemachineCameras();

        cameraPhase1.Priority = activeCamera == cameraPhase1 ? 10 : 0;
        cameraPhase2.Priority = activeCamera == cameraPhase2 ? 10 : 0;
    }

    private void DisableNonCinemachineCameras()
    {
        foreach (Camera cam in allSceneCameras)
        {
            if (!cam.GetComponent<CinemachineBrain>() && cam != null)
            {
                cam.enabled = false;
            }
        }
    }
    private void EnableSpecificCamera(Camera cam)
    {
        DisableNonCinemachineCameras();
        cam.enabled = true;
    }

}
