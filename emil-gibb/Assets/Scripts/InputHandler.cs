using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    CombatHandler combatHandler;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {
        combatHandler = GetComponent<CombatHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (combatHandler.ReadyForInput())
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                combatHandler.SelectNextUnit();
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                combatHandler.EndTurn();
            }
        }
    }
}
