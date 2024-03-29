﻿using System;
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
    private InfluenceMap influenceMap;
    // Start is called before the first frame update
    public void Start()
    {
        tileMap = GameObject.Find("Tilemap").GetComponent<Tilemap>();

        gridCells = new GridCell[size.x, size.y];
        prev = new Vector2Int[size.x, size.y];

        InitGround();

        influenceMap = new InfluenceMap(size.x, size.y);
    }

    public void Awake()
    {
        /*
        tileMap = GameObject.Find("Tilemap").GetComponent<Tilemap>();

        gridCells = new GridCell[size.x, size.y];
        prev = new Vector2Int[size.x, size.y];

        InitGround();

        influenceMap = new InfluenceMap(size.x, size.y);
        */
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            debugState();
        }
    }

    void InitGround()
    {
        for (int i = 0; i < size.x; i++)
        {
            for(int j = 0; j < size.y; j++)
            {
                float value = UnityEngine.Random.Range(0, 100.0f);
                if(value < 70)
                {
                    tileMap.SetTile(new Vector3Int(i, j, 0), tilebase[0]);
                    GridCell gC = new GridCell();
                    gC.SetOrgMoveCost(1);
                    gridCells[i, j] = gC;
                } else if ( value < 90)
                {
                    tileMap.SetTile(new Vector3Int(i, j, 0), tilebase[1]);
                    GridCell gC = new GridCell();
                    gC.SetOrgMoveCost(2);
                    gridCells[i, j] = gC;
                } else
                {
                    tileMap.SetTile(new Vector3Int(i, j, 0), tilebase[2]);
                    GridCell gC = new GridCell();
                    gC.SetOrgMoveCost(3);
                    gridCells[i, j] = gC;
                }

            }
        }
    }

    public void ResetTile(Vector3Int cell)
    {
        gridCells[cell.x, cell.y].MoveUnitFrom();
    }


    public void MoveUnit(Vector3Int from, Vector3Int to, bool enemy)
    {
        gridCells[from.x, from.y].MoveUnitFrom();
        gridCells[to.x, to.y].MoveUnitTo(enemy);
    }

    public GridCell.State getCellState(Vector3Int cell)
    {
        if(cell.x < 0 || cell.x >= size.x || cell.y < 0 || cell.y >= size.y)
        {
            return GridCell.State.OutOfBounds;
        }
        return gridCells[cell.x, cell.y].GetState();
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
        if (cell.x >= 0 && cell.x < size.x && cell.y >= 0 && cell.y < size.y)
        {
            return gridCells[cell.x, cell.y].GetReachable();
        }
        return false;
    }

    public void SetReachable(Vector3Int cell, bool value)
    {
        gridCells[cell.x, cell.y].SetReachable(value);
    }

    public bool GetAttackable(Vector3Int cell)
    {
        return gridCells[cell.x, cell.y].GetAttackable();
    }

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
            if (stack.Count >= 5)
            {
                Console.WriteLine("Pathfinding stack overflow");
                return new Stack<Vector3>();
            }
        }

        return stack;
    }

    public void Djikstra(Unit unit, List<Unit> targets)
    {
        int steps = unit.GetMovement();
        Vector3Int pos = unit.GetCellPos();
        int range = unit.GetRange();

        HashSet<Vector2Int> Q = new HashSet<Vector2Int>();
        
        for (int i = 0; i < size.x; i++)
        {
            for(int j = 0; j < size.y; j++)
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
            foreach (Unit target in targets)
            {
                if(target.enemy != unit.enemy)
                {
                    Vector3Int cellpos = target.GetCellPos();
                    if (new Vector2Int(cellpos.x, cellpos.y) == minV)
                    {
                        //print(target.GetCellPos() + "removed");
                        goto END;
                    }
                }
            }
            
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
        END:;
        }

        gridCells[pos.x, pos.y].SetReachable(true);

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                tileMap.SetColor(new Vector3Int(i, j, 0), new Vector4(0, 1 - (gridCells[i, j].GetDistance() * 0.1f), 0, 1));
                if (gridCells[i, j].GetDistance() <= steps && gridCells[i, j].GetState() == GridCell.State.Empty)
                {
                    tileMap.SetColor(new Vector3Int(i, j, 0), new Vector4(0.0f, 0.9f, 0.9f, 1));
                    gridCells[i, j].SetReachable(true);
                }
            }
        }
        
        foreach(Unit target in targets)
        {
            if (target.enemy != unit.enemy)
                isAttackableWithMove(target.GetCellPos(), range);
        }
    }

    //Needs to choose better square
    public void isAttackableWithMove(Vector3Int cell, int range)
    {
        int startx = cell.x - range;
        int starty = cell.y - range;
        int endx = cell.x + range;
        int endy = cell.y + range;
        if (startx < 0)
        {
            startx = 0;
        }
        if (starty < 0)
        {
            starty = 0;
        }
        if (endx >= size.x)
        {
            endx = size.x - 1;
        }
        if (endy >= size.y)
        {
            endy = size.y - 1;
        }

        int minrange = 1000;
        for (int i = startx; i <= endx; i++)
        {
            for (int j = starty; j <= endy; j++)
            {
                //print("checking tile: " + i + ", " + j);
                if (gridCells[i, j].GetReachable() && gridDistance(cell.x, cell.y, i, j) <= range && gridCells[i,j].GetDistance() < minrange)
                {
                    minrange = gridCells[i, j].GetDistance();
                    tileMap.SetColor(new Vector3Int(cell.x, cell.y, 0), Color.red);
                    gridCells[cell.x, cell.y].SetAttackableFrom(new Vector3Int(i, j, 0));
                    //print("enemy on tile: " + cell.x + ", " + cell.y + " attackable from: " + i + ", " + j);
                }
            }
        }
    }

    public bool isAttackableWithoutMove(Vector3Int targetCell, Vector3Int attackerCell, int range)
    {
        if (gridDistance(targetCell.x, targetCell.y, attackerCell.x, attackerCell.y) <= range)
        {
            tileMap.SetColor(new Vector3Int(targetCell.x, targetCell.y, 0), Color.red);
            gridCells[targetCell.x, targetCell.y].SetAttackableFrom(attackerCell);
            return true;
        }

        gridCells[targetCell.x, targetCell.y].SetAttackable(false);
        return false;
    }

    public bool InAttackRange(Unit attacker, Unit target)
    {
        var targetPos = target.GetCellPos();
        var attackerPos = attacker.GetCellPos();
        if (gridDistance(targetPos.x, targetPos.y, attackerPos.x, attackerPos.y) <= attacker.GetRange())
        {
            return true;
        } else
        {
            return false;
        }
    }

    public Vector3Int GetAttackCell(Vector3Int cell)
    {
        return gridCells[cell.x, cell.y].GetAttackableFrom();
    }


    public void SelectTile(Vector3Int pos)
    {
        tileMap.SetColor(pos, Color.green);
    }
    
    public void DeselectTile(Vector3Int pos)
    {
        tileMap.SetColor(pos, Color.white);
    }

    public void GrayOutBoard(Vector3Int pos)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                tileMap.SetColor(new Vector3Int(x, y, 0), Color.grey);
            }
        }
        tileMap.SetColor(pos, Color.green);
    }

    //Used to create influence map
    public float DjikstraInfluence(Vector2Int cell, List<Unit> Units)
    {
        
        int sum = 0;
        foreach(Unit unit in Units)
        {
            Vector3Int enemyPos = unit.GetCellPos();
            sum += gridDistance(cell.x, cell.y, enemyPos.x, enemyPos.y);
        }
        
        //tileMap.SetColor(cell, new Vector4(0, 1 - (sum * 0.007f), 0, 1));

        //print("i: " + pos.x + ", j: " + pos.y + ", sum: "+ sum);
        return sum;
    }


    public void setState(Vector3Int cell, GridCell.State state)
    {
        //print("Setting cell " + cell + " to " + state);
        gridCells[cell.x, cell.y].SetState(state);
    }
    
    public int gridDistance(int x1, int y1, int x2, int y2)
    {
        return Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2);
    }
    
    public Vector3Int GetBestInfluenceMove(List<Unit> Units)
    {
        Vector3Int bestMove = new Vector3Int(0, 0, 0);
        float bestScore = 1000;
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                if (gridCells[i, j].GetReachable())
                {
                    float score = DjikstraInfluence(new Vector2Int(i, j), Units);
                    //print("score of " + i + ", " + j + " : " + score);
                    if (score < bestScore)
                    {
                        bestScore = score;
                        bestMove = new Vector3Int(i, j, 0);
                    }
                }
            }
        }
        return bestMove;
    }

    public void debugState()
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                //print(x + " , " + y + " " + gridCells[x, y].GetState());
                if(gridCells[x,y].GetState() == GridCell.State.Friendly)
                {
                    tileMap.SetColor(new Vector3Int(x, y, 0), Color.green);

                } else if (gridCells[x, y].GetState() == GridCell.State.Enemy)
                {
                    tileMap.SetColor(new Vector3Int(x, y, 0), Color.red);

                } else
                {
                    tileMap.SetColor(new Vector3Int(x, y, 0), Color.yellow);
                }
            }
        }
    }
}
