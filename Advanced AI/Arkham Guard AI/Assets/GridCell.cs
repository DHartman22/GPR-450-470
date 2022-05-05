using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridCell : MonoBehaviour
{
    //What does it need to hold?
    public int cost;
    public float influence;
    public float bestCost;
    public Vector3 direction;
    public bool impassable;
    public float cellSize;
    public Vector2Int cellCoords;
    public Text costText;
    public float gizmoSize;
    public SpriteRenderer influenceIndicator;
    // Start is called before the first frame update
    void Start()
    {
        if (impassable)
            cost = 255;
    }

    // Update is called once per frame
    void Update()
    {
        costText.text = influence.ToString("F2");
        if(Input.GetKey(KeyCode.E))
        {
            costText.text = bestCost.ToString("F2");
        }
        if (influence >= 0)
            influenceIndicator.color = new Color(0f, 1f, 0f, Mathf.Abs(influence));
        else
            influenceIndicator.color = new Color(1f, 0f, 0f, Mathf.Abs(influence));
    }
    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireCube(transform.position, Vector3.one * cellSize);
        if(influence >= 0)
            Gizmos.color = new Color(0f, 1f, 0f, Mathf.Abs(influence)/2f);
        else
            Gizmos.color = new Color(1f, 0f, 0f, Mathf.Abs(influence)/2f);

        //Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y, -1), cellSize/2);
        
        //if(!impassable && influence != 0)
        //{
        //    Gizmos.DrawWireSphere(transform.position + (direction.normalized/4f), gizmoSize);
        //    Gizmos.DrawLine(transform.position - (direction.normalized / 4f), transform.position + (direction.normalized / 4f));
        //}
    }
}
