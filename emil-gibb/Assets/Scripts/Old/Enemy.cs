using Assets.Scripts.New;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Old
{
    public class Enemy : Unit
    {
        public override bool Attack(Unit target, bool offensive)
        {
            return base.Attack(target, offensive);
        }

        public override void FinishMoving()
        {
            base.FinishMoving();
            if (attacking)
            {
                //AttackAnimation();
            } else
            {
                GameObject.Find("GameHandler").GetComponent<CombatHandlerNew>().Resume();
            }
        }
    }
}
