﻿using Assets.Scripts.New;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CreateEncounter : MapNode
{
    public int difficulty;
    public GameObject grid;
    public GameObject[] enemies;
    public GameObject[] friendlies;
    public GameObject gameHandler;
    public GameObject units;
    public bool start;
    private bool instantiated;

    private Canvas CombatCanvas;

    // Start is called before the first frame update
    void Start()
    {
        CombatCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        if (start)
        {
            Instantiate(units);
            CreateGrid();
            CreateGameHandler();
            CreateEnemies();
            CreateFriendlies();
            ActivateGameHandler();
            CreateUI();
            instantiated = true;
        }

        base.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void OnMouseDown()
    {
        base.OnMouseDown();

    }

    void CreateGrid()
    {
        Instantiate(grid);
    }

    public override void ActivateNode()
    {
        if (!instantiated)
        {
            Instantiate(units);
            CreateGrid();
            CreateUI();
            CreateGameHandler();
            CreateEnemies();
            CreateFriendlies();
            ActivateGameHandler();
            instantiated = true;
        }
    }

    void CreateGameHandler()
    {
        var gH = Instantiate(gameHandler);
        gH.name = "GameHandler";
        gH.GetComponent<PlayGrid>().size = new Vector2Int(8, 8); //add randomness
    }

    void ActivateGameHandler()
    {
        GameObject.Find("GameHandler").GetComponent<PlayGrid>().enabled = true;
        GameObject.Find("GameHandler").GetComponent<CombatHandlerNew>().enabled = true;

    }

    void CreateEnemies()
    {
        int nEnemies = Random.Range(2, 5);
        CombatHandlerNew combatHandler = GameObject.Find("GameHandler").GetComponent<CombatHandlerNew>();
        for (int i = 0; i < nEnemies; i++)
        {
            int ranEn = Random.Range(0, enemies.Length);
            GameObject unit = Instantiate(enemies[ranEn]);
            unit.transform.parent = GameObject.Find("Enemies").transform;
            Unit unitScript = unit.GetComponent<Unit>();
            unitScript.startPos = new Vector2Int(Random.Range(5, 7), 1 + i);
            combatHandler.Units.Add(unitScript);
            unitScript.enabled = true;
        }
    }

    //Fix to add actual units
    void CreateFriendlies()
    {
        int nFriend = 2;
        CombatHandlerNew combatHandler = GameObject.Find("GameHandler").GetComponent<CombatHandlerNew>();
        for (int i = 0; i < nFriend; i++)
        {
            int ranEn = Random.Range(0, friendlies.Length);
            GameObject unit = Instantiate(friendlies[ranEn]);
            unit.transform.parent = GameObject.Find("Friendlies").transform;
            Unit unitScript = unit.GetComponent<Unit>();
            unitScript.startPos = new Vector2Int(Random.Range(0, 3), Random.Range(1, 5));
            combatHandler.Units.Add(unitScript);
            unitScript.enabled = true;
        }
    }

    private void CreateUI()
    {
        CombatCanvas.enabled = true;
    }
}
