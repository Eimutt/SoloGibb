using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    private bool combatToggle;
    public float rotationSpeed;
    private bool rotating;
    private float t;
    private float direction;

    public GameObject CombatCanvas;
    public GameObject WorldMap;
    // Start is called before the first frame update
    void Start()
    {
        combatToggle = true;
        direction = 1;
        t = 0;
        CombatCanvas = GameObject.Find("Canvas");
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




            if(Mathf.Abs(t) > 90)
            {
                rotating = false;
                t = 0;
                direction *= -1;
            }
        }


        if (Input.GetKeyDown(KeyCode.W))
        {
            rotating = true;
            CombatCanvas.active = direction == 1 ? false : true;
        }
    }
}
