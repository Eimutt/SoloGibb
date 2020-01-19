using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayGrid : MonoBehaviour
{
    public Vector2Int size;
    public Tilemap tileMap;
    public TileBase[] tilebase;
    public bool moving;
    public Unit currentUnit;
    public Vector2Int curCell;
    public GridCell[,] gridCells;
    private Vector2Int[,] prev;
    // Start is called before the first frame update
    public void Start()
    {
    }

    public void Awake()
    {
        tileMap = GameObject.Find("Tilemap").GetComponent<Tilemap>();

        gridCells = new GridCell[size.x, size.y];
        prev = new Vector2Int[size.x, size.y];

        InitGround();
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public void ResetTile(Vector3Int cell)
    {
        gridCells[cell.x, cell.y].MoveUnitFrom();
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

    public void MoveUnit(Vector3Int from, Vector3Int to, bool enemy)
    {
        gridCells[from.x, from.y].MoveUnitFrom();
        gridCells[to.x, to.y].MoveUnitTo(enemy);
    }



    public Vector3 GetWorldPos(Vector3Int matrixPos)
    {
        return tileMap.CellToWorld(matrixPos);
    }

    public Vector3Int getCell()
    {
        return tileMap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    public void LightDown()
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                tileMap.SetColor(new Vector3Int(x, y, 0), Color.white);
            }
        }
    }

    public bool GetReachable(Vector3Int cell)
    {
        return gridCells[cell.x, cell.y].GetReachable();
    }

    public void SetReachable(Vector3Int cell, bool value)
    {
        gridCells[cell.x, cell.y].SetReachable(value);
    }

    public bool GetAttackable(Vector3Int cell)
    {
        return gridCells[cell.x, cell.y].GetAttackable();
    }

    /*
    public Stack<Vector3> GetPath(Vector2Int start, Vector2Int end)
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
    }*/

    public Stack<Vector3> GetPath(Vector3Int start, Vector3Int end)
    {
        Stack<Vector3> stack = new Stack<Vector3>();
        if(start == end)
        {
            return stack;
        }
        Vector3Int currGCell = end;
        stack.Push(tileMap.CellToWorld(new Vector3Int(currGCell.x, currGCell.y, 1)));
        while (prev[currGCell.x, currGCell.y] != new Vector2Int(start.x, start.y))
        {
            currGCell = new Vector3Int(prev[currGCell.x, currGCell.y].x, prev[currGCell.x, currGCell.y].y, 1);
            stack.Push(tileMap.CellToWorld(currGCell));
        }

        return stack;
    }

    public void Djikstra(int steps, Vector3Int pos)
    {

        HashSet<Vector2Int> Q = new HashSet<Vector2Int>();

        int[,] dist = new int[size.x, size.y];
        for (int i = 0; i < size.x; i++)
        {
            for(int j = 0; j < size.y; j++)
            {
                gridCells[i, j].SetReachable(false);
                gridCells[i, j].SetAttackable(false);
                prev[i, j] = new Vector2Int();
                //dist[i,j] = 1000;
                gridCells[i, j].SetDistance(1000);
                Q.Add(new Vector2Int(i, j));
            }
        }
        
        //dist[pos.x, pos.y] = 0;
        gridCells[pos.x, pos.y].SetDistance(0);
        while(Q.Count > 0)
        {
            int min = 1000;
            Vector2Int minV = new Vector2Int(0, 0);
            foreach (Vector2Int cell in Q)
            {
                if (gridCells[cell.x, cell.y].GetDistance() < min)
                {
                    min = gridCells[cell.x, cell.y].GetDistance();
                    minV = cell;
                }
            }
            Q.Remove(minV);
            if(gridCells[minV.x, minV.y].GetState() == GridCell.State.Enemy)
            {
                continue;
            }
            if (minV.x > 0)
            {
                int alt = gridCells[minV.x, minV.y].GetDistance() + gridCells[minV.x - 1, minV.y].GetMoveCost();
                if (alt < gridCells[minV.x - 1, minV.y].GetDistance())
                {
                    //dist[minV.x - 1, minV.y] = alt;
                    gridCells[minV.x - 1, minV.y].SetDistance(alt);
                    prev[minV.x - 1, minV.y] = minV;
                }
            }
            if (minV.x < size.x - 1)
            {
                int alt = gridCells[minV.x, minV.y].GetDistance() + gridCells[minV.x + 1, minV.y].GetMoveCost();
                if (alt < gridCells[minV.x + 1, minV.y].GetDistance())
                {
                    //dist[minV.x + 1, minV.y] = alt;
                    gridCells[minV.x + 1, minV.y].SetDistance(alt);
                    prev[minV.x + 1, minV.y] = minV;
                }
            }
            if (minV.y > 0)
            {
                int alt = gridCells[minV.x, minV.y].GetDistance() + gridCells[minV.x, minV.y - 1].GetMoveCost();
                if (alt < gridCells[minV.x, minV.y - 1].GetDistance())
                {
                    //dist[minV.x, minV.y - 1] = alt;
                    gridCells[minV.x, minV.y - 1].SetDistance(alt);
                    prev[minV.x, minV.y - 1] = minV;
                }
            }
            if (minV.y < size.y - 1)
            {
                int alt = gridCells[minV.x, minV.y].GetDistance() + gridCells[minV.x, minV.y + 1].GetMoveCost();
                if (alt < gridCells[minV.x, minV.y + 1].GetDistance())
                {
                    //dist[minV.x, minV.y + 1] = alt;
                    gridCells[minV.x, minV.y + 1].SetDistance(alt);
                    prev[minV.x, minV.y + 1] = minV;
                }
            }
        }

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                tileMap.SetColor(new Vector3Int(i, j, 0), new Vector4(0, 1 - (gridCells[i, j].GetDistance() * 0.1f), 0, 1));
                if (gridCells[i, j].GetDistance() < steps && gridCells[i, j].GetState() == GridCell.State.Empty)
                {
                    tileMap.SetColor(new Vector3Int(i, j, 0), new Vector4(0.7f, 0.9f, 0.9f, 1));
                    gridCells[i, j].SetReachable(true);
                }
            }
        }

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                if (gridCells[i, j].GetDistance() <= steps && gridCells[i, j].GetState() == GridCell.State.Enemy)
                {
                    if (FreeSpace(i, j, steps))
                    {
                        tileMap.SetColor(new Vector3Int(i, j, 0), Color.red);
                        gridCells[i, j].SetAttackable(true);
                    }
                }
            }
        }
    }

    public float DjikstraInfluence(Vector2Int pos)
    {
        HashSet<Vector2Int> Q = new HashSet<Vector2Int>();
        
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                gridCells[i, j].SetReachable(false);
                gridCells[i, j].SetAttackable(false);
                prev[i, j] = new Vector2Int();
                gridCells[i, j].SetDistance(1000);
                Q.Add(new Vector2Int(i, j));
            }
        }
        
        gridCells[pos.x, pos.y].SetDistance(0);
        while (Q.Count > 0)
        {
            int min = 1000;
            Vector2Int minV = new Vector2Int(0, 0);
            foreach (Vector2Int cell in Q)
            {
                if (gridCells[cell.x, cell.y].GetDistance() < min)
                {
                    min = gridCells[cell.x, cell.y].GetDistance();
                    minV = cell;
                }
            }
            Q.Remove(minV);
            
            if (minV.x > 0)
            {
                int alt = gridCells[minV.x, minV.y].GetDistance() + gridCells[minV.x - 1, minV.y].GetMoveCost();
                if (alt < gridCells[minV.x - 1, minV.y].GetDistance())
                {
                    gridCells[minV.x - 1, minV.y].SetDistance(alt);
                    prev[minV.x - 1, minV.y] = minV;
                }
            }
            if (minV.x < size.x - 1)
            {
                int alt = gridCells[minV.x, minV.y].GetDistance() + gridCells[minV.x + 1, minV.y].GetMoveCost();
                if (alt < gridCells[minV.x + 1, minV.y].GetDistance())
                {
                    gridCells[minV.x + 1, minV.y].SetDistance(alt);
                    prev[minV.x + 1, minV.y] = minV;
                }
            }
            if (minV.y > 0)
            {
                int alt = gridCells[minV.x, minV.y].GetDistance() + gridCells[minV.x, minV.y - 1].GetMoveCost();
                if (alt < gridCells[minV.x, minV.y - 1].GetDistance())
                {
                    gridCells[minV.x, minV.y - 1].SetDistance(alt);
                    prev[minV.x, minV.y - 1] = minV;
                }
            }
            if (minV.y < size.y - 1)
            {
                int alt = gridCells[minV.x, minV.y].GetDistance() + gridCells[minV.x, minV.y + 1].GetMoveCost();
                if (alt < gridCells[minV.x, minV.y + 1].GetDistance())
                {
                    gridCells[minV.x, minV.y + 1].SetDistance(alt);
                    prev[minV.x, minV.y + 1] = minV;
                }
            }
        }

        int sum = 0;
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                if ( gridCells[i, j].GetState() == GridCell.State.Friendly)
                {
                    sum += gridCells[i, j].GetDistance() * gridCells[i, j].GetDistance();
                }
            }
        }
        tileMap.SetColor(new Vector3Int(pos.x, pos.y, 0), new Vector4(0, 1 - (sum * 0.007f), 0, 1));

        print("i,j = " + sum);
        return sum;
    }

    bool FreeSpace(int i, int j, int steps)
    {
        if (gridCells[i + 1, j].GetReachable() || gridCells[i - 1, j].GetReachable() || gridCells[i, j + 1].GetReachable() || gridCells[i, j - 1].GetReachable())
        {
            return true;
        }
        return false;
    }

    Vector3Int GetNeighbour(Vector3Int cell)
    {
        if (cell.x > 0)
        {
            if (gridCells[cell.x - 1, cell.y].GetReachable())
            {
                cell.x--;
                return cell;
            } 
        }
        if (cell.x < size.x - 1)
        {
            if (gridCells[cell.x + 1, cell.y].GetReachable())
            {
                cell.x++;
                return cell;
            }
        }
        if (cell.y > 0)
        {
            if (gridCells[cell.x, cell.y - 1].GetReachable())
            {
                cell.y--;
                return cell;
            }
        }
        if (cell.y < size.y - 1)
        {
            if (gridCells[cell.x, cell.y + 1].GetReachable())
            {
                cell.y++;
                return cell;
            }
        }
        return cell;
    }

    bool CheckAdjacent(Vector3Int cell)
    {
        if (((Mathf.Abs(curCell.x - cell.x) == 1) && (Mathf.Abs(curCell.y - cell.y) == 0)) || ((Mathf.Abs(curCell.x - cell.x) == 0) && (Mathf.Abs(curCell.y - cell.y) == 1)))
            return true;
        return false;
    }

    public void setState(Vector3Int cell, GridCell.State state)
    {
        print("Setting cell " + cell + " to " + state);
        gridCells[cell.x, cell.y].SetState(state);
    }

    public Vector3Int GetBestNeighbour(Vector3Int cell)
    {
        int min = 1000;
        Vector3Int nearestCell = cell;
        if (cell.x > 0)
        {
            if (gridCells[cell.x - 1, cell.y].GetDistance() < min && gridCells[cell.x - 1, cell.y].GetReachable())
            {
                nearestCell = new Vector3Int(cell.x - 1, cell.y, cell.z);
                min = gridCells[cell.x - 1, cell.y].GetDistance();
            }
        }
        if (cell.x < size.x - 1)
        {
            if (gridCells[cell.x + 1, cell.y].GetDistance() < min && gridCells[cell.x + 1, cell.y].GetReachable())
            {
                nearestCell = new Vector3Int(cell.x + 1, cell.y, cell.z);
                min = gridCells[cell.x + 1, cell.y].GetDistance();
            }
        }
        if (cell.y > 0)
        {
            if (gridCells[cell.x, cell.y - 1].GetDistance() < min && gridCells[cell.x, cell.y - 1].GetReachable())
            {
                nearestCell = new Vector3Int(cell.x, cell.y - 1, cell.z);
                min = gridCells[cell.x, cell.y + 1].GetDistance();
            }
        }
        if (cell.y < size.y - 1)
        {
            if (gridCells[cell.x, cell.y + 1].GetDistance() < min && gridCells[cell.x, cell.y + 1].GetReachable())
            {
                nearestCell = new Vector3Int(cell.x, cell.y + 1, cell.z);
                min = gridCells[cell.x, cell.y - 1].GetDistance();
            }
        }

        return nearestCell;
    }
}
