using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell
{
    public enum State { Empty, Friendly, Enemy, OutOfBounds}
    private State state;
    public bool occupied;
    public Unit unit;
    private bool reachable;
    private bool Attackable;
    private int OrgMoveCost;
    private int CurrMoveCost;
    private int distance;
    private Vector3Int attackableFrom;
    enum Terrain { Grass, Wall}
    Terrain cellT = Terrain.Grass;
    // Start is called before the first frame update
    void Start()
    {
        cellT = Terrain.Grass;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetDistance()
    {
        return distance;
    }

    public void SetDistance(int newDistance)
    {
        distance = newDistance;
    }

    public State GetState()
    {
        return state;
    }


    public void SetState(State newS)
    {
        state = newS;
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

    public void MoveUnitTo(bool enemy)
    {
        occupied = true;
        //SetMoveCost(1000);
        if (enemy)
        {
            state = State.Enemy;
        } else
        {
            state = State.Friendly;
        }
    }

    public void MoveUnitFrom()
    {
        occupied = false;
        SetMoveCost(OrgMoveCost);
        SetState(State.Empty);
    }

    public bool GetReachable()
    {
        return reachable;
    }
    
    public void SetReachable(bool value)
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

    public Vector3Int GetAttackableFrom()
    {
        return attackableFrom;
    }

    public void SetAttackableFrom(Vector3Int cell)
    {
        attackableFrom = cell;
        SetAttackable(true);
    }

    public void KillUnit()
    {
        SetAttackable(false);
        SetReachable(false);
        unit = null;
        SetState(State.Empty);
    }

}
