using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class MyUnitEvent : UnityEvent<Unit>
{

}
public class EventManager : MonoBehaviour
{
    public MyUnitEvent SelectUnit;


    public void SelectUnitEvent(Unit unit)
    {
        print("invocked");
        SelectUnit.Invoke(unit);
    }
}
