using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayGrid : MonoBehaviour
{
    public Vector2Int size;
    private Tilemap tileMap;
    public TileBase[] tilebase;
    public bool moving;
    public Unit currentUnit;
    public Vector2Int curCell;
    public GridCell[,] gridCells;
    private Vector2Int[,] prev;
    // Start is called before the first frame update
    void Start()
    {
        tileMap = gameObject.GetComponentInChildren<Tilemap>();
        print(tileMap.cellBounds);

        gridCells = new GridCell[size.x, size.y];
        InitGround();

        prev = new Vector2Int[size.x, size.y];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!moving)
            {
                Vector3 clickpos = (Camera.main.ScreenToWorldPoint(Input.mousePosition));
                clickpos.z = 1;
                Vector3Int cell = tileMap.WorldToCell(clickpos);
                if (gridCells[cell.x, cell.y].unit != null && !gridCells[cell.x, cell.y].unit.enemy)
                {
                    currentUnit = gridCells[cell.x, cell.y].unit;
                    curCell = new Vector2Int(cell.x, cell.y);
                    moving = true;
                    //LightUp();
                    Djikstra(gridCells[cell.x, cell.y].unit.movement, new Vector2Int(cell.x, cell.y));
                }
            } else
            {
                Vector3 clickpos = (Camera.main.ScreenToWorldPoint(Input.mousePosition));
                clickpos.z = 1;
                Vector3Int cell = tileMap.WorldToCell(clickpos);
                if (gridCells[cell.x, cell.y].GetReachable())
                {
                    gridCells[curCell.x, curCell.y].occupied = false;
                    //currentUnit.Move(MoveToCell(currentUnit, cell));
                    MoveToCell(currentUnit, cell);
                    currentUnit.StartMoving(GetPath(curCell, new Vector2Int(cell.x, cell.y)), tileMap.CellToWorld(new Vector3Int(curCell.x, curCell.y, 1)));
                    moving = false;
                    currentUnit = null;
                    LightDown();
                }
            }
            
            
        }
    }

    void InitGround()
    {
        for (int i = 0; i < size.x; i++)
        {
            for(int j = 0; j < size.y; j++)
            {
                tileMap.SetTile(new Vector3Int(i, j, 0), tilebase[1]);
                GridCell gC = new GridCell();
                gC.SetOrgMoveCost(1);
                gridCells[i, j] = gC;
            }
        }

        tileMap.SetTile(new Vector3Int(3, 3, 0), tilebase[0]);
        tileMap.SetTile(new Vector3Int(3, 4, 0), tilebase[0]);
        tileMap.SetTile(new Vector3Int(3, 2, 0), tilebase[0]);
        GridCell gC1 = new GridCell();
        GridCell gC2 = new GridCell();
        GridCell gC3 = new GridCell();
        gC1.SetOrgMoveCost(2);
        gC2.SetOrgMoveCost(2);
        gC3.SetOrgMoveCost(2);
        gridCells[3, 3] = gC1;
        gridCells[3, 4] = gC2;
        gridCells[3, 2] = gC3;
    }

    public Vector3 MoveToCell(Unit unit, Vector3Int matrixPos)
    {
        gridCells[curCell.x, curCell.y].MoveUnitFrom();
        gridCells[matrixPos.x, matrixPos.y].MoveUnitTo(unit);
        //gridCells[curCell.x, curCell.y].unit = null;
        //gridCells[curCell.x, curCell.y].occupied = false;
        //gridCells[matrixPos.x, matrixPos.y].occupied = true;
        //gridCells[matrixPos.x, matrixPos.y].unit = unit;
        return tileMap.CellToWorld(matrixPos);
        
    }

    void LightUp()
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                if ((Mathf.Abs(curCell.x - x) + Mathf.Abs(curCell.y - y)) < 3 && !gridCells[x,y].occupied)
                {
                    tileMap.SetColor(new Vector3Int(x, y, 0), Color.green);
                } else if ((Mathf.Abs(curCell.x - x) + Mathf.Abs(curCell.y - y)) < 4 && gridCells[x, y].unit != null && gridCells[x, y].unit.enemy)
                {
                    tileMap.SetColor(new Vector3Int(x, y, 0), Color.red);
                }
            }
        }
    }

    void LightDown()
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                tileMap.SetColor(new Vector3Int(x, y, 0), Color.white);
            }
        }
    }

    Stack<Vector3> GetPath(Vector2Int start, Vector2Int end)
    {
        Stack<Vector3> stack = new Stack<Vector3>();
        Vector2Int currGCell = end;
        stack.Push(tileMap.CellToWorld(new Vector3Int(currGCell.x, currGCell.y, 1)));
        while (prev[currGCell.x, currGCell.y] != start)
        {
            currGCell = prev[currGCell.x, currGCell.y];
            stack.Push(tileMap.CellToWorld(new Vector3Int(currGCell.x, currGCell.y, 1)));
        }
        
        return stack;
    }

    void Djikstra(int steps, Vector2Int pos)
    {

        HashSet<Vector2Int> Q = new HashSet<Vector2Int>();

        int[,] dist = new int[size.x, size.y];
        for (int i = 0; i < size.x; i++)
        {
            for(int j = 0; j < size.y; j++)
            {
                gridCells[i, j].SetReachable(false);
                dist[i,j] = 1000;
                Q.Add(new Vector2Int(i, j));
            }
        }
        
        dist[pos.x, pos.y] = 0;
        while(Q.Count > 0)
        {
            int min = 1000;
            Vector2Int minV = new Vector2Int(0, 0);
            foreach (Vector2Int cell in Q)
            {
                if (dist[cell.x, cell.y] < min)
                {
                    min = dist[cell.x, cell.y];
                    minV = cell;
                }
            }
            Q.Remove(minV);
            if (minV.x > 0)
            {
                int alt = dist[minV.x, minV.y] + gridCells[minV.x - 1, minV.y].GetMoveCost();
                if (alt < dist[minV.x - 1, minV.y])
                {
                    dist[minV.x - 1, minV.y] = alt;
                    prev[minV.x - 1, minV.y] = minV;
                }
            }
            if (minV.x < size.x - 1)
            {
                int alt = dist[minV.x, minV.y] + gridCells[minV.x + 1, minV.y].GetMoveCost();
                if (alt < dist[minV.x + 1, minV.y])
                {
                    dist[minV.x + 1, minV.y] = alt;
                    prev[minV.x + 1, minV.y] = minV;
                }
            }
            if (minV.y > 0)
            {
                int alt = dist[minV.x, minV.y] + gridCells[minV.x, minV.y - 1].GetMoveCost();
                if (alt < dist[minV.x, minV.y - 1])
                {
                    dist[minV.x, minV.y - 1] = alt;
                    prev[minV.x, minV.y - 1] = minV;
                }
            }
            if (minV.y < size.y - 1)
            {
                int alt = dist[minV.x, minV.y] + gridCells[minV.x, minV.y + 1].GetMoveCost();
                if (alt < dist[minV.x, minV.y + 1])
                {
                    dist[minV.x, minV.y + 1] = alt;
                    prev[minV.x, minV.y + 1] = minV;
                }
            }
        }

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                if(dist[i,j] < steps && !gridCells[i,j].occupied)
                {
                    tileMap.SetColor(new Vector3Int(i, j, 0), Color.green);
                    gridCells[i, j].SetReachable(true);
                }
            }
        }
    }
}
