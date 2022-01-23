using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class MyUnitEvent : UnityEvent<Unit>
{

}

[System.Serializable]
public class AddUnitEvent : UnityEvent<int, string, bool> { }

[System.Serializable]
public class IntEvent : UnityEvent<int> { }

[System.Serializable]
public class IntListEvent : UnityEvent<List<int>> { }

public class EventManager : MonoBehaviour
{
    public MyUnitEvent SelectUnit;
    public AddUnitEvent AddUnit;
    public IntEvent MoveUnit;
    public IntEvent ActionUnit;
    public UnityEvent Reset;
    public IntListEvent UpdateTurnOrder;
    public IntEvent HighlightUnit;
    public IntEvent NewTurn;
    public IntEvent RemoveUnit;

    public void SelectUnitEvent(Unit unit)
    {
        SelectUnit.Invoke(unit);
    }

    public void AddUnitEvent(int unitId, string name, bool isEnemy)
    {
        AddUnit.Invoke(unitId, name, isEnemy);
    }
    public void RemoveUnitEvent(int unitId)
    {
        RemoveUnit.Invoke(unitId);
    }

    public void MoveUnitEvent(int unitId)
    {
        MoveUnit.Invoke(unitId);
    }

    public void ActionUnitEvent(int unitId)
    {
        ActionUnit.Invoke(unitId);
    }

    public void ResetEvent()
    {
        Reset.Invoke();
    }

    public void NewTurnEvent(int turn)
    {
        NewTurn.Invoke(turn);
    }

    public void UpdateTurnOrderEvent(List<int> units)
    {
        UpdateTurnOrder.Invoke(units);
    }

    public void HighLightUnitEvent(int unitId)
    {
        HighlightUnit.Invoke(unitId);
    }
}
