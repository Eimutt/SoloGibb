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
    // Start is called before the first frame update
    void Start()
    {
        UnitName = gameObject.transform.Find("UnitName").gameObject;
        UnitCloseUp = gameObject.transform.Find("UnitCloseUp").gameObject;
        Finish = gameObject.transform.Find("Finish").gameObject;
        UnitHealth = gameObject.transform.Find("Stats/Health/Text").gameObject;
        eventManager = GameObject.Find("EventManager").GetComponent<EventManager>();
        eventManager.SelectUnit.AddListener(UpdateUI);
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
        } else
        {
            Finish.SetActive(false);
        }
    }
}
