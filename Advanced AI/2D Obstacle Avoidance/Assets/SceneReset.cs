using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneReset : MonoBehaviour
{
    public int buildIndexToLoad;
    public Transform startingPos;
    public GameObject agent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(buildIndexToLoad);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            agent.transform.position = startingPos.position;
        }
    }
}
