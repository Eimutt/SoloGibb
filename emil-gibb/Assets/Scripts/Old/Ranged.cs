using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranged : Unit
{
    public override bool Attack(Unit target, bool offensive)
    {
        print(this.name + " ranged attacks " + target.name + " for " + this.dmg);
        if (offensive)
        {

        }
        return target.TakeDamage(dmg);
    }
}
