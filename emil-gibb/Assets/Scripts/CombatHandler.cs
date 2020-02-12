using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatHandler : MonoBehaviour
{
    public List<Unit> FriendlyUnits = new List<Unit>();
    public List<Unit> EnemyUnits = new List<Unit>();
    private PlayGrid playGrid;
    private bool playerTurn = true;
    public Unit selected;
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
        if (playerTurn)
        {
            if (Input.GetMouseButtonDown(0) && !moving)
            {
                //IF NO UNIT SEE IF CLICKING ON UNIT
                if (!selected)
                {
                    Vector3Int clickPos = playGrid.getCell();
                    clickPos.z = 0;
                    foreach (Unit unit in FriendlyUnits)
                    {
                        if (unit.GetCellPos() == clickPos && unit.HasActionLeft())
                        {
                            selected = unit;
                            playGrid.SelectTile(clickPos);
                            if (selected.HasMoveLeft())
                            {
                                playGrid.Djikstra(selected.GetMovement(), selected.GetCellPos(), selected.GetRange());

                                //playGrid.GetEnemiesInRange(selected.GetRange(), selected.GetCellPos());
                            } else
                            {
                                playGrid.GetEnemiesInRange(selected.GetRange(), selected.GetCellPos());
                            }
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
                            if (selected.HasMoveLeft())
                            {
                                playGrid.SetReachable(selected.GetCellPos(), true);
                                Vector3Int nextPos = playGrid.GetAttackCell(clickPos);
                                Stack<Vector3> path = playGrid.GetPath(selected.GetCellPos(), nextPos);
                                if (path.Count != 0)
                                {
                                    selected.StartMoving(path, playGrid.GetWorldPos(selected.GetCellPos()), 3f);
                                    playGrid.MoveUnit(selected.GetCellPos(), nextPos, false);
                                    selected.SetCellPos(nextPos);
                                    moving = true;
                                }
                            }
                            /*
                            Vector3Int nextPos = playGrid.GetAttackCell(clickPos);
                            Stack<Vector3> path = playGrid.GetPath(selected.GetCellPos(), nextPos);
                            if (path.Count != 0)
                            {
                                selected.StartMoving(path, playGrid.GetWorldPos(selected.GetCellPos()), 3f);
                                playGrid.MoveUnit(selected.GetCellPos(), nextPos, false);
                                selected.SetCellPos(nextPos);
                                moving = true;
                            }*/

                            if (selected.Attack(unit))
                            {
                                playGrid.ResetTile(clickPos);
                            }
                            else if(unit.GetRange() >= selected.GetRange())
                            {
                                if (unit.Attack(selected))
                                {
                                    playGrid.ResetTile(selected.GetCellPos());
                                    FriendlyUnits.Remove(selected);
                                }
                            }


                        }
                    }
                    foreach (Unit unit in FriendlyUnits)
                    {
                        if (unit.GetCellPos() == clickPos && unit.HasActionLeft())
                        {
                            playGrid.DeselectTile(selected.GetCellPos());
                            selected = unit;
                            if (selected.HasMoveLeft())
                            {
                                playGrid.Djikstra(unit.GetMovement(), unit.GetCellPos(), selected.GetRange());
                            }
                            targeted = true;
                            playGrid.SelectTile(clickPos);
                        }
                    }
                    if (!targeted && playGrid.GetReachable(clickPos) && selected.HasMoveLeft())
                    {
                        Stack<Vector3> path = playGrid.GetPath(selected.GetCellPos(), clickPos);
                        selected.StartMoving(path, playGrid.GetWorldPos(selected.GetCellPos()), 3f);
                        playGrid.MoveUnit(selected.GetCellPos(), clickPos, false);
                        selected.SetCellPos(clickPos);
                        moving = true;
                        targeted = true;
                        playGrid.LightDown();
                        playGrid.GetEnemiesInRange(selected.GetRange(), selected.GetCellPos());
                        playGrid.SelectTile(clickPos);
                    }
                    if (!targeted)
                    {
                        selected = null;
                        playGrid.LightDown();
                    }
                }
            }
        }
        else //Enemy Turn
        {
            //playGrid.FillInfluenceMap(EnemyUnits, FriendlyUnits);
            foreach (Unit enemy in EnemyUnits)
            {

                //playGrid.Djikstra(enemy.GetMovement(), enemy.GetCellPos());
            }
            playerTurn = true;
        }
    }

    public void Resume()
    {
        moving = false;
    }

    public void EndTurn()
    {
        if (playerTurn)
        {
            //PlayerTurn = false;
            foreach(Unit unit in FriendlyUnits)
            {
                unit.NewTurn();
                unit.calculateImportance();
            }
            playerTurn = false;
        }
        else
        {
            playerTurn = true;
            foreach(Unit unit in EnemyUnits)
            {
                unit.NewTurn();
            }
        }
    }

    public void InitUnits()
    {
        foreach (Unit unit in FriendlyUnits)
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
