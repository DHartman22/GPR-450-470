using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowFieldGrid : MonoBehaviour
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
        for(int i = 0; i < gridWidth; i++)
        {
            for(int j = 0; j < gridHeight; j++)
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
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 closestCellCoords = new Vector2(-1, -1);
            float shortestDistance = 100000f;
            for (int i = 0; i < gridWidth; i++)
            {
                for (int j = 0; j < gridHeight; j++)
                {
                    if(Vector3.Distance(clickPos, cells[i][j].transform.position) < shortestDistance)
                    {
                        shortestDistance = Vector3.Distance(clickPos, cells[i][j].transform.position);
                        closestCellCoords = cells[i][j].cellCoords;
                    }
                }
            }
            targetCell = cells[(int)closestCellCoords.x][(int)closestCellCoords.y];
            targetIndicator.transform.position = targetCell.transform.position;
            //Recreate cost field
            GenerateCostField();
            GenerateIndicationField(targetCell);
        }
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

    void GenerateCostField()
    {
        //Rule out the impassible areas
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                Collider[] colliders = Physics.OverlapBox(cells[i][j].transform.position, Vector3.one * (cellSize/2f));
                
                foreach(Collider c in colliders)
                {
                    if(c.gameObject.layer == LayerMask.GetMask("Obstacle")) 
                    {
                        cells[i][j].cost = 255;
                        cells[i][j].impassable = true;
                    }
                }

                if(colliders.Length == 0)
                {
                    cells[i][j].cost = 1;
                    cells[i][j].bestCost = int.MaxValue;

                    cells[i][j].impassable = false;
                }
            }
        }
    }

    //This video helped me with this function https://youtu.be/tSe6ZqDKB0Y
    void GenerateIndicationField(GridCell destinationCell)
    {
        targetCell = destinationCell;
        targetCell.cost = 0;
        targetCell.bestCost = 0;

        Queue<GridCell> cellQueue = new Queue<GridCell>();

        cellQueue.Enqueue(targetCell);
        while (cellQueue.Count > 0)
        {
            GridCell currentCell = cellQueue.Dequeue();
            List<GridCell> neighbors = GetNeighborCells(currentCell.cellCoords);
            foreach(GridCell neighborCell in neighbors)
            {
                if(neighborCell.cost != 255)
                {
                    if(neighborCell.cost + currentCell.bestCost < neighborCell.bestCost)
                    {
                        neighborCell.bestCost = neighborCell.cost + currentCell.bestCost;
                        cellQueue.Enqueue(neighborCell);
                    }
                    
                }
            }
        }
        GenerateFlowField();
    }

    void GenerateFlowField()
    {
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                List<GridCell> neighbors = GetNeighborCells(cells[i][j].cellCoords);
                int lowestCost = int.MaxValue;
                GridCell cellToMoveTowards;
                foreach (GridCell neighborCell in neighbors)
                {
                    if (neighborCell.cost != 255)
                    {
                        if (neighborCell.bestCost < lowestCost)
                        {
                            lowestCost = neighborCell.bestCost;
                            cellToMoveTowards = neighborCell;
                            cells[i][j].direction = (cellToMoveTowards.transform.position - cells[i][j].transform.position).normalized;
                        }
                    }
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
        
        if(cellCoords.x + 1 < gridWidth)
        {
            neighborCells.Add(cells[(int)cellCoords.x + (int)directions[(int)Directions.East].x][(int)cellCoords.y + (int)directions[(int)Directions.East].y]);
            if (cellCoords.y - 1 >= 0)
                neighborCells.Add(cells[(int)cellCoords.x + (int)directions[(int)Directions.SouthEast].x][(int)cellCoords.y + (int)directions[(int)Directions.SouthEast].y]);

            if (cellCoords.y + 1 < gridHeight)
                neighborCells.Add(cells[(int)cellCoords.x + (int)directions[(int)Directions.NorthEast].x][(int)cellCoords.y + (int)directions[(int)Directions.NorthEast].y]);
        }
        
        if(cellCoords.y + 1 < gridHeight)
            neighborCells.Add(cells[(int)cellCoords.x + (int)directions[(int)Directions.North].x][(int)cellCoords.y + (int)directions[(int)Directions.North].y]);
        if(cellCoords.y - 1 >= 0)    
            neighborCells.Add(cells[(int)cellCoords.x + (int)directions[(int)Directions.South].x][(int)cellCoords.y + (int)directions[(int)Directions.South].y]);

        return neighborCells;
    }

    GridCell GetCellInRelativeDirection(GridCell startingCell, Directions direction)
    {
        Vector2 dir = directions[(int)direction];
        Vector2 targetCoords = new Vector2(startingCell.cellCoords.x + dir.x, startingCell.cellCoords.y + dir.y);
        if(targetCoords.x >= 0 && targetCoords.x < gridWidth && targetCoords.y >= 0 && targetCoords.y < gridHeight)
        {
            return cells[(int)targetCoords.x][(int)startingCell.cellCoords.y];
        }
        else
        {
            Debug.Log("Cell in direction " + direction.ToString() + " does not exist");
            return null;
        }
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
        GenerateCostField();

    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
    }
}
