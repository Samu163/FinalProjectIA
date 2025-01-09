using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxInstantiator : MonoBehaviour
{
    public GameObject boxPrefab;
    public List<Transform> spawnPositions;
    public List<GameObject> boxes;

    public void InstantiateBoxes()
    {
        foreach (var box in boxes)
        {
            if (!box.activeSelf)
            {
                box.SetActive(true);
            }
        }

    }


}
