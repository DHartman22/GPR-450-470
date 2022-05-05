using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FearRaycast : MonoBehaviour
{
    Ray theRay;
    RaycastHit hit;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);
            Debug.Log(hit.collider.gameObject.name);
            if(hit.collider.gameObject.layer == 8)
            {
                hit.collider.gameObject.GetComponent<Guard>().FearEvent(10);
                
            }
        }
        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);
            Debug.Log(hit.collider.gameObject.name);
            if (hit.collider.gameObject.layer == 8)
            {
                hit.collider.gameObject.GetComponent<Guard>().FearEvent(-10);

            }
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);
            Debug.Log(hit.collider.gameObject.name);
            if (hit.collider.gameObject.layer == 8)
            {
                hit.collider.gameObject.GetComponent<Guard>().UnconsciousEvent();

            }
        }

    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawLine(Camera.main.ScreenPointToRay(Input.mousePosition).GetPoint(0), hit.point);
        if(Input.GetKey(KeyCode.Mouse0))
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.green;
        }
    }
}
