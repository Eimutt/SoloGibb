using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMap : MonoBehaviour
{
    public int length;
    public int maxOptions;
    public int minOptions;
    public int currentLevel;

    private List<MapNode> mapNodes;

    public List<GameObject> NodeObjectPrefabs;
    public List<GameObject> EncounterPrefabs;
    public GameObject NodeBase;
    public GameObject Path;

    public Vector2 start;
    // Start is called before the first frame update
    void Start()
    {
        mapNodes = new List<MapNode>();
        generateWorld();
    }

    // Update is called once per frame
    void Update()
    {
        start = new Vector2(0, 0);
    }

    void generateWorld()
    {
        //generate start node
        StartNode();


        for (int i = 1; i < length; i++)
        {
            int options = Random.Range(minOptions, maxOptions);

            bool encounter = (i % 1 == 0);
            for (int j = 1; j <= options; j++)
            {
                var node = Instantiate(NodeBase, transform);

                node.name = i + " " + j;

                node.transform.localPosition = posAlongLine(j * (1f/((float)options + 1)) ,i);

                //create path objects
            }




            if (i % 1 == 0)
            {
                for (int j = 0; j < options; j++)
                {
                    //Generate Encounter
                }
            }
            else
            {
                for (int j = 0; j < options; j++)
                {
                    //Generate other nodes from prefab list
                }
            }

        }
        //generate boss node
    }

    void StartNode()
    {
        var startNode = Instantiate(NodeBase, transform);

        //set pos
        startNode.transform.localPosition = new Vector3(start.x, start.y, 0);


        var s = startNode.AddComponent<MapNode>();
        s.level = 0;
        mapNodes.Add(s);

    }

    void BossNode()
    {

    }


    Vector3 posAlongLine(float t, float i)
    {
        Vector3 pos = new Vector3();

        Vector3 lineStart = new Vector3(start.x + (i * 1), start.y + i * 3, 0);
        Vector3 lineEnd = new Vector3(start.y + i * 1 - (start.x * 0.5f), start.y + (i * 1.5f), 0);

        pos.x = lineStart.x * t + lineEnd.x * (1 - t);

        pos.y = lineStart.y * t + lineEnd.y * (1 - t);

        return pos;

    }
}
