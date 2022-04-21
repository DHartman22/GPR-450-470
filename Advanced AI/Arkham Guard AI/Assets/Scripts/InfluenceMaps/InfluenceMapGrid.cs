using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfluenceMapGrid : MonoBehaviour
{
    public int gridWidth;
    public int gridHeight;
    public float cellSize;
    public List<List<GridCell>> cells;
    public List<GridCell> openList;
    public List<GridCell> closedList;
    public GameObject gridCell;
    public Transform startingPos;
    public GridCell targetCell;
    public GameObject targetIndicator;
    public float cellExtents;
    public int impassibleCost = 255;
    public int roughCost = 3;
    public LayerMask impassible;
    public LayerMask rough;
    public float timeBetweenRefreshes;
    public float timeSinceLastRefresh;
    public Vector2 clickPos;
    public bool initted;

    public List<InfluenceActor> influences;
    public enum Directions
    {
        North,
        East,
        South,
        West,
        NorthEast,
        NorthWest,
        SouthEast,
        SouthWest,
        None
    }

    private Vector2[] directions =
    {
        new Vector2(0, 1),
        new Vector2(1, 0),
        new Vector2(0, -1),
        new Vector2(-1, 0),
        new Vector2(1, 1),
        new Vector2(-1, 1),
        new Vector2(1, -1),
        new Vector2(-1, -1),
        new Vector2(0, 0)
    };

    public void GenerateGrid()
    {
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                GameObject newCell = Instantiate(gridCell, startingPos.position + new Vector3(cellSize * i, cellSize * j, 0), Quaternion.identity);
                newCell.name = "Cell " + i + ", " + j;
                cells[i][j] = newCell.GetComponent<GridCell>();
                cells[i][j].cellCoords = new Vector2Int(i, j);
                cells[i][j].transform.parent = transform;

            }
        }
    }

    void CheckInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            initted = true;
            //RefreshFlowField();
        }
    }

    void InfluenceMapRefreshTimer()
    {
        timeSinceLastRefresh += Time.deltaTime;
        if (timeSinceLastRefresh > timeBetweenRefreshes)
        {
            RefreshInfluenceMap();
            timeSinceLastRefresh -= timeBetweenRefreshes;
        }
    }

    public void RefreshInfluenceMap()
    {
        Vector2 closestCellCoords = new Vector2(-1, -1);
        //refresh influences list to ensure new influences are accounted for
        influences.Clear();
        influences.AddRange(GameObject.FindObjectsOfType<InfluenceActor>());
        //Recreate map
        GenerateInfluenceMap();
    }

    public GridCell WorldSpaceToCell(Vector3 worldPos)
    {
        Vector2 closestCellCoords = new Vector2(-1, -1);
        float shortestDistance = 100000f;
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                if (Vector3.Distance(worldPos, cells[i][j].transform.position) < shortestDistance)
                {
                    shortestDistance = Vector3.Distance(worldPos, cells[i][j].transform.position);
                    closestCellCoords = cells[i][j].cellCoords;
                }
            }
        }
        return cells[(int)closestCellCoords.x][(int)closestCellCoords.y];
    }

    void GenerateInfluenceMap()
    {
        //For every grid cell, calculate the influence value based on adding each 
        //influence actor's influence value at the center of the given cell
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                Collider[] colliders = Physics.OverlapBox(cells[i][j].transform.position, Vector3.one * (cellSize / 2f));

                foreach (Collider c in colliders)
                {
                    if (c.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
                    {
                        cells[i][j].cost = impassibleCost;
                        cells[i][j].influence = int.MinValue;

                        cells[i][j].impassable = true;
                    }
                }
                cells[i][j].influence = 0;
                for(int k = 0; k < influences.Count; k++)
                {
                    float newInfluence =
                        1 - Mathf.Pow((1 * (Vector3.Distance(cells[i][j].transform.position, influences[k].transform.position) / influences[k].influenceRadius)), 2);

                    if (Vector3.Distance(cells[i][j].transform.position, influences[k].transform.position) < influences[k].minRange)
                        continue;

                    //reverse influence for enemies
                    if (influences[k].influenceType == InfluenceActor.InfluenceType.Negative)
                        newInfluence *= -1;

                    if(influences[k].influenceType == InfluenceActor.InfluenceType.Positive && newInfluence < 0)
                        newInfluence = 0;
                    if (influences[k].influenceType == InfluenceActor.InfluenceType.Negative && newInfluence > 0)
                        newInfluence = 0;

                    //changes how agressive the influence falloff is
                    cells[i][j].influence += newInfluence * influences[k].influenceStrength;

                    //ensure it doesn't go past 1 either way
                    if (cells[i][j].influence > 1)
                        cells[i][j].influence = 1;
                    if (cells[i][j].influence < -1)
                        cells[i][j].influence = -1;
                }

                if (colliders.Length == 0)
                {
                    cells[i][j].cost = 1;

                    cells[i][j].impassable = false;
                }
            }
        }
    }

    List<GridCell> GetNeighborCells(Vector2 cellCoords)
    {
        List<GridCell> neighborCells = new List<GridCell>();
        if (cellCoords.x - 1 >= 0)
        {
            neighborCells.Add(cells[(int)cellCoords.x + (int)directions[(int)Directions.West].x][(int)cellCoords.y + (int)directions[(int)Directions.West].y]);
            if (cellCoords.y - 1 >= 0)
                neighborCells.Add(cells[(int)cellCoords.x + (int)directions[(int)Directions.SouthWest].x][(int)cellCoords.y + (int)directions[(int)Directions.SouthWest].y]);
            if (cellCoords.y + 1 < gridHeight)
                neighborCells.Add(cells[(int)cellCoords.x + (int)directions[(int)Directions.NorthWest].x][(int)cellCoords.y + (int)directions[(int)Directions.NorthWest].y]);
        }

        if (cellCoords.x + 1 < gridWidth)
        {
            neighborCells.Add(cells[(int)cellCoords.x + (int)directions[(int)Directions.East].x][(int)cellCoords.y + (int)directions[(int)Directions.East].y]);
            if (cellCoords.y - 1 >= 0)
                neighborCells.Add(cells[(int)cellCoords.x + (int)directions[(int)Directions.SouthEast].x][(int)cellCoords.y + (int)directions[(int)Directions.SouthEast].y]);

            if (cellCoords.y + 1 < gridHeight)
                neighborCells.Add(cells[(int)cellCoords.x + (int)directions[(int)Directions.NorthEast].x][(int)cellCoords.y + (int)directions[(int)Directions.NorthEast].y]);
        }

        if (cellCoords.y + 1 < gridHeight)
            neighborCells.Add(cells[(int)cellCoords.x + (int)directions[(int)Directions.North].x][(int)cellCoords.y + (int)directions[(int)Directions.North].y]);
        if (cellCoords.y - 1 >= 0)
            neighborCells.Add(cells[(int)cellCoords.x + (int)directions[(int)Directions.South].x][(int)cellCoords.y + (int)directions[(int)Directions.South].y]);

        return neighborCells;
    }

    GridCell GetCellInRelativeDirection(GridCell startingCell, Directions direction)
    {
        Vector2 dir = directions[(int)direction];
        Vector2 targetCoords = new Vector2(startingCell.cellCoords.x + dir.x, startingCell.cellCoords.y + dir.y);
        if (targetCoords.x >= 0 && targetCoords.x < gridWidth && targetCoords.y >= 0 && targetCoords.y < gridHeight)
        {
            return cells[(int)targetCoords.x][(int)startingCell.cellCoords.y];
        }
        else
        {
            Debug.Log("Cell in direction " + direction.ToString() + " does not exist");
            return null;
        }
    }

    public GridCell GetHighestInfluenceCellInRange(FlockingAgent agent)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(agent.gameObject.transform.position, agent.influenceDetectionRange);
        float highestInfluence = -1;
        GridCell highestInfluenceCell = null;

        foreach(Collider2D collider in colliders) 
        {
            if (collider.gameObject.layer != LayerMask.NameToLayer("GridCell"))
                continue;
            GridCell cell = collider.gameObject.GetComponent<GridCell>();
            if (cell.influence > highestInfluence && Vector3.Distance(agent.transform.position, cell.transform.position) < agent.influenceDetectionRange)
            {
                highestInfluenceCell = cell;
                highestInfluence = cell.influence;
            }
        }

        return highestInfluenceCell;
    }

    // Start is called before the first frame update
    void Start()
    {
        cells = new List<List<GridCell>>(gridWidth);
        for (int i = 0; i < gridWidth; i++)
        {
            cells.Add(new List<GridCell>(gridHeight));
            cells[i].AddRange(new GridCell[gridHeight]);
        }

        GenerateGrid();
        GenerateInfluenceMap();

    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        InfluenceMapRefreshTimer();
    }
}
