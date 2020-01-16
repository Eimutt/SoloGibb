using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatHandler : MonoBehaviour
{
    public List<Unit> FriendliyUnits = new List<Unit>();
    public List<Unit> EnemyUnits = new List<Unit>();
    private PlayGrid playGrid;
    private bool PlayerTurn = true;
    private Unit selected;
    private bool moving = false;
    // Start is called before the first frame update
    void Start()
    {
        playGrid = GetComponent<PlayGrid>();
        InitUnits();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerTurn)
        {
            if (Input.GetMouseButtonDown(0) && !moving)
            {
                //IF NO UNIT SEE IF CLICKING ON UNIT
                if (!selected)
                {
                    Vector3Int clickPos = playGrid.getCell();
                    clickPos.z = 0;
                    foreach (Unit unit in FriendliyUnits)
                    {
                        if (unit.GetCellPos() == clickPos && unit.GetAction())
                        {
                            selected = unit;
                            playGrid.Djikstra(unit.GetMovement(), unit.GetCellPos());
                        }
                    }
                }
                else //Unit Selected
                {
                    bool targeted = false;
                    Vector3Int clickPos = playGrid.getCell();
                    clickPos.z = 0;
                    foreach (Unit unit in EnemyUnits)
                    {
                        if (unit.GetCellPos() == clickPos && playGrid.GetAttackable(clickPos))
                        {
                            playGrid.SetReachable(selected.GetCellPos(), true);
                            Vector3Int nextPos = playGrid.GetBestNeighbour(clickPos);
                            Stack<Vector3> path = playGrid.GetPath(selected.GetCellPos(), nextPos);
                            if(path.Count != 0)
                            {
                                selected.StartMoving(path, playGrid.GetWorldPos(selected.GetCellPos()), 3f);
                                playGrid.MoveUnit(selected.GetCellPos(), nextPos, false);
                                selected.SetCellPos(nextPos);
                                moving = true;
                            }
                            if (selected.Attack(unit))
                            {
                                playGrid.ResetTile(clickPos);
                            }
                        }
                    }
                    foreach (Unit unit in FriendliyUnits)
                    {
                        if (unit.GetCellPos() == clickPos && unit.GetAction())
                        {
                            selected = unit;
                            playGrid.Djikstra(unit.GetMovement(), unit.GetCellPos());
                            targeted = true;
                        }
                    }
                    if (playGrid.GetReachable(clickPos))
                    {
                        Stack<Vector3> path = playGrid.GetPath(selected.GetCellPos(), clickPos);
                        selected.StartMoving(path, playGrid.GetWorldPos(selected.GetCellPos()), 3f);
                        playGrid.MoveUnit(selected.GetCellPos(), clickPos, false);
                        selected.SetCellPos(clickPos);
                        moving = true;
                    }
                    if (!targeted)
                    {
                        selected = null;
                        playGrid.LightDown();
                    }
                }
            }
        }
    }

    public void Resume()
    {
        moving = false;
    }

    public void EndTurn()
    {
        if (PlayerTurn)
        {
            //PlayerTurn = false;
            foreach(Unit unit in FriendliyUnits)
            {
                unit.NewTurn();
            }
        }
        else
        {
            PlayerTurn = true;
            foreach(Unit unit in EnemyUnits)
            {
                unit.NewTurn();
            }
        }
    }

    public void InitUnits()
    {
        foreach (Unit unit in FriendliyUnits)
        {
            unit.Move(playGrid.GetWorldPos(unit.GetCellPos()));
            playGrid.setState(unit.GetCellPos(), GridCell.State.Friendly);
        }
        foreach (Unit unit in EnemyUnits)
        {
            unit.Move(playGrid.GetWorldPos(unit.GetCellPos()));
            playGrid.setState(unit.GetCellPos(), GridCell.State.Enemy);
        }
    }
}
