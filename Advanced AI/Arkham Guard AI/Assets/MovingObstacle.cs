using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    public Transform target;
    public Transform origin;
    public bool movingTowardsTarget = true;
    public float timeSinceLerpStart;
    public float totalLerpTime;

    void LerpStart()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLerpStart += Time.deltaTime;
        if(movingTowardsTarget)
        {
            transform.position = Vector3.Lerp(origin.position, target.position, timeSinceLerpStart/totalLerpTime);
        }
        else
        {
            transform.position = Vector3.Lerp(target.position, origin.position, timeSinceLerpStart / totalLerpTime);
        }
        if(timeSinceLerpStart > totalLerpTime)
        {
            timeSinceLerpStart = 0;
            movingTowardsTarget = !movingTowardsTarget;
        }
    }
}
