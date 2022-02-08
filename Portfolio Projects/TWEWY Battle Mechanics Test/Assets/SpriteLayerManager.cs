using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLayerManager : MonoBehaviour
{
    public List<SpriteRenderer> renderers;
    // Start is called before the first frame update
    void Start()
    {
        renderers = new List<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        renderers.Clear();
        renderers.AddRange(GameObject.FindObjectsOfType<SpriteRenderer>());
        foreach(SpriteRenderer renderer in renderers)
        {
            renderer.sortingOrder = (int)renderer.gameObject.transform.position.y;
        }
    }
}
