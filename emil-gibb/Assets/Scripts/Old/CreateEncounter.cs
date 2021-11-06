using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CreateEncounter : MonoBehaviour
{
    public int difficulty;
    public GameObject grid;
    public GameObject[] enemies;
    public GameObject[] friendlies;
    public GameObject gameHandler;
    public GameObject units;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(units);
        CreateGrid();
        CreateGameHandler();
        CreateEnemies();
        CreateFriendlies();
        ActivateGameHandler();
        CreateUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateGrid()
    {
        Instantiate(grid);
    }

    void CreateGameHandler()
    {
        var gH = Instantiate(gameHandler);
        gH.name = "GameHandler";
        gH.GetComponent<PlayGrid>().size = new Vector2Int(Random.Range(8, 10), Random.Range(6, 8));
        GameObject.Find("EndTurn").GetComponent<Button>().onClick.AddListener(() => GameObject.Find("GameHandler").GetComponent<CombatHandler>().EndTurn());
    }

    void ActivateGameHandler()
    {
        GameObject.Find("GameHandler").GetComponent<PlayGrid>().enabled = true;
        GameObject.Find("GameHandler").GetComponent<CombatHandler>().enabled = true;

    }

    void CreateEnemies()
    {
        int nEnemies = Random.Range(2, 5);
        CombatHandler combatHandler = GameObject.Find("GameHandler").GetComponent<CombatHandler>();
        for (int i = 0; i < nEnemies; i++)
        {
            int ranEn = Random.Range(0, enemies.Length);
            GameObject unit = Instantiate(enemies[ranEn]);
            unit.transform.parent = GameObject.Find("Enemies").transform;
            Unit unitScript = unit.GetComponent<Unit>();
            unitScript.startPos = new Vector2Int(Random.Range(5, 7), Random.Range(1, 5));
            combatHandler.EnemyUnits.Add(unitScript);
            unitScript.enabled = true;
        }
    }

    //Fix to add actual units
    void CreateFriendlies()
    {
        int nFriend = 2;
        CombatHandler combatHandler = GameObject.Find("GameHandler").GetComponent<CombatHandler>();
        for (int i = 0; i < nFriend; i++)
        {
            int ranEn = Random.Range(0, friendlies.Length);
            GameObject unit = Instantiate(friendlies[ranEn]);
            unit.transform.parent = GameObject.Find("Friendlies").transform;
            Unit unitScript = unit.GetComponent<Unit>();
            unitScript.startPos = new Vector2Int(Random.Range(0, 3), Random.Range(1, 5));
            combatHandler.FriendlyUnits.Add(unitScript);
            unitScript.enabled = true;
        }
    }

    private void CreateUI()
    {
    }
}
