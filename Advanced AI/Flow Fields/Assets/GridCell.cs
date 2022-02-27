using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridCell : MonoBehaviour
{
    //What does it need to hold?
    public int cost;
    public int bestCost;
    public Vector3 direction;
    public bool impassable;
    public float cellSize;
    public Vector2Int cellCoords;
    public Text costText;
    public float gizmoSize;
    // Start is called before the first frame update
    void Start()
    {
        if (impassable)
            cost = 255;
    }

    // Update is called once per frame
    void Update()
    {
        costText.text = bestCost.ToString();
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, Vector3.one * cellSize);
        Gizmos.color = new Color(1f, 1f, 0f, 0.25f);
        if(!impassable && bestCost != 0)
        {
            Gizmos.DrawWireSphere(transform.position + (direction.normalized/4f), gizmoSize);
            Gizmos.DrawLine(transform.position - (direction.normalized / 4f), transform.position + (direction.normalized / 4f));
        }
    }
}
