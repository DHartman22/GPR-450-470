using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    public Vector2 trajectory;
    public float speed;
    public float killTime = 0.8f;
    public float activeTime;
    
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = (trajectory * speed);
    }

    // Update is called once per frame
    void Update()
    {
        activeTime += Time.deltaTime;
        if (activeTime > killTime)
            Destroy(gameObject);
    }
}
