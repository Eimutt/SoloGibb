using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneHandler : MonoBehaviour
{
    private bool combatToggle;
    public float rotationSpeed;
    private bool rotating;
    private float t;
    private float direction;

    public Canvas CombatCanvas;
    public GameObject WorldMap;

    public MapNode mapNode;

    public bool inEncounter;
    public bool init;


    // Start is called before the first frame update
    void Start()
    {
        direction = -1;
        t = 0;
        CombatCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        WorldMap = GameObject.Find("WorldMap");
    }

    // Update is called once per frame
    void Update()
    {
        if (rotating)
        {
            transform.Rotate(rotationSpeed * direction * Time.deltaTime, 0.0f, 0.0f, Space.Self);
            t += rotationSpeed * direction * Time.deltaTime;

            WorldMap.transform.Translate(Vector3.up * 6 * direction * Time.deltaTime);




            if (Mathf.Abs(t) > 90)
            {
                rotating = false;
                t = 0;
                direction *= -1;
                if(init == false)
                {
                    ActivateNode();
                }
            }
        }


        if (Input.GetKeyDown(KeyCode.W))
        {
            rotating = true;
            CombatCanvas.enabled = direction == 1 ? false : true;
        }
    }

    public void MoveToInteractiveScene(MapNode newMapNode)
    {
        inEncounter = true;
        mapNode = newMapNode;
        rotating = true;
        combatToggle = true;
    }

    public void ActivateNode()
    {
        init = true;
        mapNode.ActivateNode();
    }

    public void MoveToMap()
    {

    }

    public void FinishEncounter()
    {
        inEncounter = false;
    }
}
