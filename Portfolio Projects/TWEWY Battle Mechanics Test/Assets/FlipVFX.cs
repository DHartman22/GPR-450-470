using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipVFX : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(spriteRenderer.flipX)
        {
            //transform.position = new Vector3(-transform.position.x, -transform.position.y);
        }

    }
}
