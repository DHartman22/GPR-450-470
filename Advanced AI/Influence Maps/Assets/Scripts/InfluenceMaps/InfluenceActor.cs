using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfluenceActor : MonoBehaviour
{
    [SerializeField] public float influenceStrength; //Can be positive for friendly influence or negative for enemy influence
    [SerializeField] public float influenceRadius;
    public enum InfluenceType
    {
        Positive,
        Negative
    }
    public InfluenceType influenceType;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
