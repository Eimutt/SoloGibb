using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedUnitUiHandler : MonoBehaviour
{
    private GameObject UnitName;
    private GameObject UnitCloseUp;
    private GameObject UnitHealth;
    private GameObject Finish;
    private EventManager eventManager;
    private GameObject Turn;

    private GameObject UnitList;
    public GameObject UnitPrefab;

    public List<int> unitsId;
    public int startPos;
    // Start is called before the first frame update
    void Start()
    {
        UnitName = gameObject.transform.Find("UnitName").gameObject;
        UnitCloseUp = gameObject.transform.Find("UnitCloseUp").gameObject;
        Finish = gameObject.transform.Find("Finish").gameObject;
        UnitList = gameObject.transform.Find("UnitList").gameObject;
        Turn = gameObject.transform.Find("Turn").gameObject;
        UnitHealth = gameObject.transform.Find("Stats/Health/Text").gameObject;
        unitsId = new List<int>();



        eventManager = GameObject.Find("EventManager").GetComponent<EventManager>();
        eventManager.SelectUnit.AddListener(UpdateUI);
        eventManager.AddUnit.AddListener(AddUnit);
        eventManager.UpdateTurnOrder.AddListener(UpdateListOrder);
        eventManager.MoveUnit.AddListener(MoveUnit);
        eventManager.ActionUnit.AddListener(ActionUnit);
        eventManager.NewTurn.AddListener(NewTurn);
        eventManager.RemoveUnit.AddListener(RemoveUnit);

    }

    // Update is called once per frame
    void Update()
    {

    }

    void UpdateUI(Unit unitinfo)
    {
        UnitName.GetComponent<Text>().text = unitinfo.GetName();
        UnitHealth.GetComponent<Text>().text = unitinfo.GetHealthString();
        UnitCloseUp.GetComponent<Image>().sprite = unitinfo.closeup;

        if (!unitinfo.enemy)
        {
            Finish.SetActive(true);
        }
        else
        {
            Finish.SetActive(false);
        }
    }

    void AddUnit(int unitId, string name, bool enemy)
    {
        var UnitEntry = Instantiate(UnitPrefab, UnitList.transform);
        UnitEntry.name = unitId.ToString();
        UnitEntry.transform.Find("Name").GetComponent<Text>().text = name;
        UnitEntry.GetComponent<Image>().color = enemy ? Color.red : Color.green;
    }

    void UpdateListOrder(List<int> unitIds)
    {
        var y = startPos;
        foreach (int i in unitIds)
        {
            var t = UnitList.transform.Find(i.ToString());

            if (t != null)
            {
                t.GetComponent<RectTransform>().localPosition = new Vector3(0, y, 0);
                y -= 30;
            }
        }
    }

    void NewTurn(int turn)
    {
        Turn.GetComponent<Text>().text = turn.ToString();
        

        foreach (Transform t in UnitList.transform){

            t.Find("Action").GetComponent<Image>().color = Color.white;
            t.Find("Move").GetComponent<Image>().color = Color.white;

            if(t.gameObject.active == false)
            {
                Destroy(t.gameObject);
            }
        }
    }

    void ActionUnit(int unitId)
    {
        transform.Find("UnitList/" + unitId.ToString() + "/Action").GetComponent<Image>().color = Color.grey;
    }

    void MoveUnit(int unitId)
    {
        transform.Find("UnitList/" + unitId.ToString() + "/Move").GetComponent<Image>().color = Color.grey;
    }

    void RemoveUnit(int unitId)
    {
        transform.Find("UnitList/" + unitId.ToString()).gameObject.active = false;
        //Destroy(transform.Find("UnitList/" + unitId.ToString()));
    }
}
