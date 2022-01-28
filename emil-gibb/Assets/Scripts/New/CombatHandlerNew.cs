using Assets.Scripts.Enums;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.New
{
    public class CombatHandlerNew : MonoBehaviour
    {
        public List<Unit> Units = new List<Unit>();
        private PlayGrid playGrid;
        private int turn;
        private Unit selectedUnit;
        private EventManager eventManager;
        private bool playerTurn;
        private bool active;
        private bool readyForInput;
        private Unit selectedTarget;

        void Start()
        {
            turn = 1;
            playGrid = GetComponent<PlayGrid>();
            eventManager = GameObject.Find("EventManager").GetComponent<EventManager>();
            InitUnits();
            GameObject.Find("Finish").GetComponent<Button>().onClick.AddListener(() => EndMovement());
            GameObject.Find("Wait").GetComponent<Button>().onClick.AddListener(() => Wait());

        }

        private void Update()
        {
            //If target is reached attack
            //
            if (selectedTarget != null && selectedUnit.GetUnitState() == UnitStateEnum.Idle)
            {
                Attack(selectedUnit, selectedTarget, false);
                selectedTarget = null;
                if (selectedUnit.enemy)
                {
                    return;
                }
            }
            else if (!active)
            {
                NextUnit();
            }



            if (!selectedUnit.enemy && selectedUnit.GetUnitState() == UnitStateEnum.Idle && Input.GetMouseButtonDown(0) && selectedUnit.HasActionLeft())
            {
                Vector3Int clickPos = playGrid.getCell();
                GridCell.State state = playGrid.getCellState(clickPos);
                clickPos.z = 0;
                if (state == GridCell.State.Enemy)
                {
                    for (int i = Units.Count - 1; i >= 0; i--)
                    {
                        Unit target = Units[i];
                        if (target.GetCellPos() == clickPos && playGrid.GetAttackable(clickPos))
                        {
                            if (playGrid.InAttackRange(selectedUnit, target)) //Attack without moving if possible
                                Attack(selectedUnit, target, false);
                            else
                            {
                                MoveForAttack(selectedUnit, target); //Move within range before attacking
                            }

                        }
                    }
                }
                else if (state == GridCell.State.Empty)
                {
                    if (playGrid.GetReachable(clickPos) && selectedUnit.HasMoveLeft())
                    {
                        Move(selectedUnit, clickPos);
                    }
                }
            }
            else if (selectedUnit.enemy && selectedUnit.GetUnitState() == UnitStateEnum.Idle) //Enemy should move
            {
                var FriendlyUnits = Units.Where(x => !x.enemy).ToList();
                playGrid.Djikstra(selectedUnit, Units); //See where on grid selected unit can walk

                bool canAttack = false;
                float bestValue = 0;
                Unit bestTarget = null;
                //Check if target in range
                for (int i = FriendlyUnits.Count - 1; i >= 0; i--)
                {
                    Unit target = FriendlyUnits[i];
                    if (playGrid.GetAttackable(target.GetCellPos()))
                    {
                        print(target.name + " is attackable");
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
                    if (playGrid.InAttackRange(selectedUnit, bestTarget)) //Attack without moving if possible
                        Attack(selectedUnit, bestTarget, false);
                    else
                    {
                        MoveForAttack(selectedUnit, bestTarget); //Move within range before attacking
                    }
                }
                else
                //Find the best square to move to
                {
                    Vector3Int bestMove = playGrid.GetBestInfluenceMove(Units);
                    Move(selectedUnit, bestMove);
                    active = false;
                    selectedUnit.DoAction();
                }
            }
        }

        public void NextUnit()
        {
            //int maxSpeed = 0;
            //int bestIndex = -1;
            //for (int i = 0; i < Units.Count; i++)
            //{
            //    if (Units[i].HasMoveLeft() && Units[i].GetCurrentSpeed() > maxSpeed)
            //    {
            //        maxSpeed = Units[i].GetCurrentSpeed();
            //        bestIndex = i;
            //    }
            //}

            foreach (Unit unit in Units)
            {
                if (unit.HasMoveLeft())
                {
                    SelectUnit(unit);
                    return;
                }
            }
            NextTurn();
        }

        public void SelectUnit(Unit newUnit)
        {
            //UI
            eventManager.SelectUnitEvent(newUnit);

            selectedUnit = newUnit;
            active = true;
            //playerTurn = !newUnit.enemy;
            if (!newUnit.enemy)
            {
                playGrid.LightDown();
                playGrid.Djikstra(newUnit, Units);
                playGrid.SelectTile(newUnit.GetCellPos());
                playerTurn = true;
                readyForInput = true;
            }
        }

        public void NextTurn()
        {
            //Update Turn Counter in UI
            foreach (Unit unit in Units)
            {
                unit.NewTurn();
            }
            active = false;


            turn++;
            CreateTurnOrder();
            eventManager.NewTurnEvent(turn);
        }
        public void InitUnits()
        {
            int unitId = 0;
            foreach (Unit unit in Units)
            {
                unit.SetUnitId(unitId);
                unit.Move(playGrid.GetWorldPos(unit.GetCellPos()));
                if (unit.enemy)
                {
                    playGrid.setState(unit.GetCellPos(), GridCell.State.Enemy);
                }
                else
                {
                    playGrid.setState(unit.GetCellPos(), GridCell.State.Friendly);
                }
                eventManager.AddUnitEvent(unitId, unit.name, unit.enemy);
                unitId++;
            }
            CreateTurnOrder();
        }

        public void Move(Unit unit, Vector3Int end)
        {
            eventManager.MoveUnitEvent(unit.GetUnitId());
            if (unit.GetCellPos() == end && unit.enemy)
            {
                EndMovement();
            }


            Vector3Int start = selectedUnit.GetCellPos();
            playGrid.SetReachable(start, true);
            Stack<Vector3> path = playGrid.GetPath(start, end);
            if (path.Count != 0)
            {
                unit.StartMoving(path, playGrid.GetWorldPos(start), 3f, false);
                playGrid.MoveUnit(start, end, unit.isEnemy());
                unit.SetCellPos(end);
            }

            playGrid.GrayOutBoard(end);
            GetAttackable();
        }

        public void Attack(Unit attacker, Unit target, bool counterAttack)
        {

            eventManager.ActionUnitEvent(attacker.GetUnitId());


            selectedTarget = null;
            //Target Dies
            if (attacker.Attack(target, true))
            {
                playGrid.ResetTile(target.GetCellPos());
                Units.Remove(target);
                eventManager.RemoveUnitEvent(target.GetUnitId());
            }
            //CounterAttack
            else if (target.GetRange() >= attacker.GetRange())
            {
                //Attacker Dies
                if (target.Attack(attacker, false))
                {
                    playGrid.ResetTile(attacker.GetCellPos());
                    Units.Remove(attacker);
                    eventManager.RemoveUnitEvent(attacker.GetUnitId());
                }
            }
            attacker.DoAction();
            if (attacker.isEnemy())
                active = false;

            if (IsCombatDone())
            {
                GameObject.Find("WorldMap").GetComponent<WorldMap>().ActivateMovement();
            }
        }

        public void Wait()
        {
            var unit = selectedUnit;
            Units.Remove(unit);
            Units.Add(unit);

            eventManager.UpdateTurnOrderEvent(Units.Select(u => u.GetUnitId()).ToList());
            NextUnit();
        }

        public void MoveForAttack(Unit attacker, Unit target)
        {
            eventManager.MoveUnitEvent(attacker.GetUnitId());

            playGrid.SetReachable(attacker.GetCellPos(), true);
            Vector3Int nextPos = playGrid.GetAttackCell(target.GetCellPos());
            Stack<Vector3> path = playGrid.GetPath(attacker.GetCellPos(), nextPos);
            if (path.Count != 0)
            {
                attacker.StartMoving(path, playGrid.GetWorldPos(attacker.GetCellPos()), 3f, true);
                playGrid.MoveUnit(attacker.GetCellPos(), nextPos, attacker.isEnemy());
                attacker.SetCellPos(nextPos);
            }
            selectedTarget = target;
            playGrid.GrayOutBoard(nextPos);
        }

        public void EndMovement()
        {
            selectedUnit.DoAction();
            NextUnit();
        }

        public void GetAttackable()
        {
            foreach (Unit target in Units)
            {
                if (target.enemy)
                {
                    playGrid.isAttackableWithoutMove(target.GetCellPos(), selectedUnit.GetCellPos(), selectedUnit.GetRange());
                }
            }
        }

        public void Resume()
        {
            active = false;
        }

        public void CreateTurnOrder()
        {
            Units.Sort((y, u) => u.GetCurrentSpeed().CompareTo(y.GetCurrentSpeed()));
            eventManager.UpdateTurnOrderEvent(Units.Select(u => u.GetUnitId()).ToList());
        }

        public void UpdateUiForUnit()
        {

        }

        public bool IsCombatDone()
        {
            if (!Units.Any(a => a.isEnemy() == true))
            {
                return true;
            }
            else if (!Units.Any(a => a.isEnemy() == true))
            {
                return true;
            }
            return false;
        }
    }
}
