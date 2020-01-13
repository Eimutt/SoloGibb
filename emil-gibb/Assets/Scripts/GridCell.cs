using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    public enum State { Empty, Friendly, Enemy}
    private State state;
    public bool occupied;
    public Unit unit;
    private bool reachable;
    private bool Attackable;
    private int OrgMoveCost;
    private int CurrMoveCost;
    enum Terrain { Grass, Wall}
    Terrain cellT;
    // Start is called before the first frame update
    void Start()
    {
        cellT = Terrain.Grass;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public State GetState()
    {
        return state;
    }

    public int GetMoveCost()
    {
        if (state == State.Enemy)
        {
            //return 100;
        }
        return CurrMoveCost;
        
    }

    public void SetMoveCost(int NewMoveCost)
    {
        CurrMoveCost = NewMoveCost;
        
    }

    public void SetOrgMoveCost(int NewMoveCost)
    {
        OrgMoveCost = NewMoveCost;
        CurrMoveCost = NewMoveCost;
    }

    public void MoveUnitTo(Unit NewUnit)
    {
        unit = NewUnit;
        occupied = true;
        //SetMoveCost(1000);
        if (NewUnit.enemy)
        {
            state = State.Enemy;
        } else
        {
            state = State.Friendly;
        }
    }

    public void MoveUnitFrom()
    {
        unit = null;
        occupied = false;
        SetMoveCost(OrgMoveCost);
        state = State.Empty;
    }

    public bool GetReachable()
    {
        return reachable;
    }
    
    public void GetReachable(bool value)
    {
        reachable = value;
    }

    public bool GetAttackable()
    {
        return Attackable;
    }

    public void SetAttackable(bool value)
    {
        Attackable = value;
    }
}
