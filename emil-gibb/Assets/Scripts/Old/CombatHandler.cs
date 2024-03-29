﻿using System.Collections;
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
    private int tabIndex = 0;
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
                            SelectUnit(unit);
                        }
                    }
                }
                else //Unit Selected
                {

                    playGrid.LightDown();
                    Vector3Int clickPos = playGrid.getCell();
                    GridCell.State state = playGrid.getCellState(clickPos);
                    clickPos.z = 0;
                    if (state == GridCell.State.Enemy)
                    {
                        for (int i = EnemyUnits.Count - 1; i >= 0; i--)
                        {
                            Unit target = EnemyUnits[i];
                            if (target.GetCellPos() == clickPos && playGrid.GetAttackable(clickPos))
                            {
                                Attack(selected, target);
                            }
                        }
                    }
                    else if (state == GridCell.State.Friendly)
                    {
                        foreach (Unit unit in FriendlyUnits)
                        {
                            if (unit.GetCellPos() == clickPos && unit.HasActionLeft())
                            {
                                SelectUnit(unit);
                            }
                        }
                    }
                    else if (state == GridCell.State.Empty)
                    {
                        if (playGrid.GetReachable(clickPos) && selected.HasMoveLeft())
                        {
                            Move(selected.GetCellPos(), clickPos, selected);
                            GetAttackable(selected);
                        }
                        else
                        {
                            selected = null;
                        }

                    }
                }
            }
        }
        else //Enemy Turn
        {
            if (!moving)
            {
                bool enemyLeft = false;
                foreach (Unit enemy in EnemyUnits)
                {
                    if (enemy.HasActionLeft())
                    {
                        enemyLeft = true;
                    }
                    else
                    {
                        continue;
                    }
                    playGrid.Djikstra(enemy, FriendlyUnits);
                    bool canAttack = false;
                    float bestValue = 0;
                    Unit bestTarget = null;
                    //Check if target in range
                    for (int i = FriendlyUnits.Count - 1; i >= 0; i--)
                    {
                        Unit target = FriendlyUnits[i];
                        if (playGrid.GetAttackable(target.GetCellPos()))
                        {
                            canAttack = true;
                            if (target.getImportance() > bestValue)
                            {
                                bestTarget = target;
                                bestValue = target.getImportance();
                            }

                        }
                    }
                    //IF target found attack
                    if (canAttack)
                    {
                        Attack(enemy, bestTarget);
                        break;
                    }
                    else
                    //Find the best square to move to
                    {
                        //Vector3Int bestMove = playGrid.GetBestInfluenceMove(EnemyUnits, FriendlyUnits);
                        //Move(enemy.GetCellPos(), bestMove, enemy);
                        //enemy.DoAction();
                        //break;
                    }
                }
                if (!enemyLeft)
                    EndTurn();
            }
        }
    }

    public void Resume()
    {
        moving = false;
    }


    public void SelectUnit(Unit unit)
    {
        playGrid.LightDown();
        selected = unit;
        if (selected.HasMoveLeft())
        {
            playGrid.Djikstra(selected, EnemyUnits);
        }
        else
        {
            GetAttackable(selected);
        }
        playGrid.SelectTile(unit.GetCellPos());
    }

    public void SelectNextUnit()
    {
        for (int i = tabIndex; i < FriendlyUnits.Count; i++)
        {
            if (FriendlyUnits[i].HasActionLeft())
            {
                SelectUnit(FriendlyUnits[i]);
                break;
            }
        }
        tabIndex++;
        if (tabIndex >= FriendlyUnits.Count)
            tabIndex = 0;
    }

    public void EndTurn()
    {
        if (playerTurn)
        {
            selected = null;
            foreach (Unit unit in EnemyUnits)
            {
                unit.NewTurn();
            }
            playerTurn = false;
        }
        else
        {
            playerTurn = true;
            foreach (Unit unit in FriendlyUnits)
            {

                unit.calculateImportance();
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

    public void Move(Vector3Int start, Vector3Int end, Unit unit)
    {
        playGrid.SelectTile(end);
        playGrid.SetReachable(start, true);
        Stack<Vector3> path = playGrid.GetPath(start, end);
        if (path.Count != 0)
        {
            unit.StartMoving(path, playGrid.GetWorldPos(start), 3f, false);
            playGrid.MoveUnit(start, end, unit.isEnemy());
            unit.SetCellPos(end);
            moving = true;
        }
    }

    public bool ReadyForInput()
    {
        return (!moving && playerTurn);
    }

    public void Attack(Unit attacker, Unit target)
    {
        if (attacker.HasMoveLeft())
        {
            playGrid.SetReachable(attacker.GetCellPos(), true);
            Vector3Int nextPos = playGrid.GetAttackCell(target.GetCellPos());
            Stack<Vector3> path = playGrid.GetPath(attacker.GetCellPos(), nextPos);
            if (path.Count != 0)
            {
                attacker.StartMoving(path, playGrid.GetWorldPos(attacker.GetCellPos()), 3f, true);
                playGrid.MoveUnit(attacker.GetCellPos(), nextPos, attacker.isEnemy());
                attacker.SetCellPos(nextPos);
                moving = true;
            }
        }

        //Target Dies
        if (attacker.Attack(target, true))
        {
            playGrid.ResetTile(target.GetCellPos());
            if (attacker.isEnemy())
            {
                FriendlyUnits.Remove(target);
            }
            else
            {
                EnemyUnits.Remove(target);
            }
        }
        //CounterAttack
        else if (target.GetRange() >= attacker.GetRange())
        {
            //Attacker Dies
            if (target.Attack(attacker, false))
            {
                playGrid.ResetTile(attacker.GetCellPos());
                if (attacker.isEnemy())
                {
                    EnemyUnits.Remove(attacker);
                }
                else
                {
                    FriendlyUnits.Remove(attacker);
                }
            }
        }
    }

    public void GetAttackable(Unit selected)
    {
        foreach (Unit target in EnemyUnits)
        {
            playGrid.isAttackableWithoutMove(target.GetCellPos(), selected.GetCellPos(), selected.GetRange());
        }
    }
}
