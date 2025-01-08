using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class FlockManager : MonoBehaviour
{
    
    public static FlockManager FM;

    public LeaderMovement leader;


    public GameObject birdPrefab; 
    
   
    public int numBird = 20; 
    
   
    public GameObject[] allBird; 
    
    
    public Vector3 flyLimits = new Vector3(); 
    
    
    public Vector3 goalPos = Vector3.zero;

    [Header("Bird Settings")]
    
   
    [Range(0.0f, 5.0f)]
    public float minSpeed;
    
   
    [Range(0.0f, 5.0f)]
    public float maxSpeed;
    
    
    [Range(1.0f, 10.0f)]
    public float neighbourDistance;
    
    
    [Range(1.0f, 5.0f)]
    public float rotationSpeed;

    
    void Start()
    {
       
        allBird = new GameObject[numBird];

       
        for (int i = 0; i < numBird; i++)
        {
            
            Vector3 pos = this.transform.position + new Vector3(
                Random.Range(-flyLimits.x, flyLimits.x),
                Random.Range(-flyLimits.y, flyLimits.y),  
                Random.Range(-flyLimits.z, flyLimits.z));

           
            allBird[i] = Instantiate(birdPrefab, pos, Quaternion.identity);
           
        }
      
        FM = this;
        
        
        goalPos = this.transform.position;
    }

    
    void Update()
    {
        
        if (Random.Range(0, 100) < 10)
        {
            
            goalPos = this.transform.position + new Vector3(
                Random.Range(-flyLimits.x, flyLimits.x),
                Random.Range(-flyLimits.y, flyLimits.y),  
                Random.Range(-flyLimits.z, flyLimits.z));
        }
    }
}
