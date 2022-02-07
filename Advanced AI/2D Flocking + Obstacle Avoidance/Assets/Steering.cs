using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steering : MonoBehaviour
{

    public virtual Vector2 GetSteering(Vector2 position, Vector2 velocity)
    {
        Debug.Log("Base steering class");
        return Vector2.zero;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
