using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    public bool occupied;
    public Unit unit;
    private bool reachable;
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

    public int GetMoveCost()
    {
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
    }

    public void MoveUnitFrom()
    {
        unit = null;
        occupied = false;
        SetMoveCost(OrgMoveCost);
    }

    public bool GetReachable()
    {
        return reachable;
    }
    
    public void SetReachable(bool value)
    {
        reachable = value;
    }
}
