using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailCollider : MonoBehaviour
{
    public enum TrailType
    {
        Front,
        Back
    }

    public TrailType type;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "TrailEnd" && gameObject.tag == "TrailStart")
        {
            GameObject.FindObjectOfType<CursorManager>().CircleCheck();
        }
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
