using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode : MonoBehaviour
{
    public int level;
    public List<MapNode> reachableFrom = new List<MapNode>();
    protected WorldMap worldMap;
    // Start is called before the first frame update
    public virtual void Start()
    {
        worldMap = GameObject.Find("WorldMap").GetComponent<WorldMap>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void OnMouseDown()
    {
        print(gameObject.name + " selected");

        bool canGoTo = IsReachable();
        if (canGoTo)
        {
            print("moving");
            worldMap.MoveTo(this);
            //Start encounter or smthing

        } else
        {
            print("cant move");

        }
    }

    protected bool IsReachable()
    {
        WorldMap worldMap = GameObject.Find("WorldMap").GetComponent<WorldMap>();

        return reachableFrom.Contains(worldMap.currentLevel);
    }
}
