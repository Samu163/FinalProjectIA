using System.Collections.Generic;
using UnityEngine;

public class BroadcastManager : MonoBehaviour
{
    private List<ZombieBehavior> zombies = new List<ZombieBehavior>();
    private bool playerDetected = false;
    private Transform player;

    void Start()
    {
        GameObject playerObject = GameObject.FindWithTag("robber");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    public void RegisterZombie(ZombieBehavior zombie)
    {
        if (!zombies.Contains(zombie))
        {
            zombies.Add(zombie);
        }
    }

    public void OnPlayerDetected()
    {
        playerDetected = true;

        foreach (ZombieBehavior zombie in zombies)
        {
            zombie.SetPursuing(player);
        }
    }

    public void OnPlayerLost()
    {
        playerDetected = false;

        foreach (ZombieBehavior zombie in zombies)
        {
            zombie.SetWandering();
        }
    }
}
