using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.New
{
    public class CombatHandlerNew : MonoBehaviour
    {

        public List<Unit> Units = new List<Unit>();
        private PlayGrid playGrid;
        private int turn;
        private Unit selectedUnit;
        private EventManager eventManager;
        void Start()
        {
            turn = 1;
            playGrid = GetComponent<PlayGrid>();
            eventManager = GameObject.Find("EventManager").GetComponent<EventManager>();

        }

        private void Update()
        {
            if(selectedUnit == null)
            {
                NextUnit();
            }
            if (selectedUnit.enemy)
            {

            }
        }

        public void NextUnit()
        {
            int maxSpeed = 0;
            int index = -1;
            foreach(Unit unit in Units)
            {
                index++;
                if (unit.HasMoveLeft() && unit.GetCurrentSpeed() > maxSpeed)
                {
                    maxSpeed = unit.GetCurrentSpeed(); 
                } 
            }

            if (index != -1)
                SelectUnit(Units[index]);
            else
                NextTurn();
        }

        public void SelectUnit(Unit selectUnit)
        {
            selectedUnit = selectUnit;

            eventManager.SelectUnitEvent(selectedUnit);


        }

        public void NextTurn()
        {
            //Update Turn Counter in UI
            foreach(Unit unit in Units)
            {
                unit.NewTurn();
            }
        }
    }
}
