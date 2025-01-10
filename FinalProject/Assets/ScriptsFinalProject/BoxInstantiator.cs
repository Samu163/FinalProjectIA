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

    public void ActivateBoxes()
    {
        foreach (var position in spawnPositions)
        {
            GameObject box = Instantiate(boxPrefab, position.position, Quaternion.identity);
            boxes.Add(box);
        }
    }


}
