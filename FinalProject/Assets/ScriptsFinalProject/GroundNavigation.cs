using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class GroundNavigation : MonoBehaviour
{
    public NavMeshSurface navMeshSurface; 

    void Start()
    {
        // Inicializa el bake inicial
        navMeshSurface.BuildNavMesh();
    }

    public void RemoveObjectAndRebake(GameObject obstacle)
    {
        if (obstacle != null)
        {
            Destroy(obstacle);
            navMeshSurface.BuildNavMesh();
        }
    }
}
