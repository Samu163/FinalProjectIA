using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CivilianInstantiator : MonoBehaviour
{
    public CivilianController civilianPrefab;
    public List<Transform> positions = new List<Transform>();
    public List<CivilianController> civilians = new List<CivilianController>();

    public void InstantiateCivilians(int number)
    {
        List<Transform> tempList = new List<Transform>(GameManager.Instance.player.civilians);
        for (int i = 0; i < number; i++)
        {
            if (positions.Count == 0) break;

            int randomIndex = Random.Range(0, positions.Count);
            Transform randomPosition = positions[randomIndex];

            CivilianController newCivilian = Instantiate(civilianPrefab, randomPosition.position, randomPosition.rotation);
            newCivilian.Init(randomPosition);

            civilians.Add(newCivilian); 
            tempList.Add(newCivilian.transform);
            

        }
        GameManager.Instance.player.civilians = tempList.ToArray();
        tempList.Clear();

    }
}
