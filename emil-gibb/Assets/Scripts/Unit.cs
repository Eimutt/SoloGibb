using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private PlayGrid playgrid;
    public Vector2Int startPos;
    public bool enemy;
    private bool gridSpawned;
    public int hp;
    public int dmg;
    public int movement;



    private bool moving;
    private Stack<Vector3> stack;
    private Vector3 target;
    private Vector3 origin;
    private float t;
    public float MDurr;



    // Start is called before the first frame update
    void Start()
    {
        playgrid = GameObject.Find("Grid").GetComponent<PlayGrid>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gridSpawned)
        {
            StartMove();
            gridSpawned = true;
        }
        if (moving)
        {
            MoveAlongPath();
        }
    }
    
    void StartMove()
    {
        transform.position = playgrid.MoveToCell(this, new Vector3Int(startPos.x, startPos.y, 0)) + new Vector3(0, 0.5f, 0);
    }

    public void Move(Vector3 pos)
    {
        transform.position = pos + new Vector3(0, 0.5f, 0);
    }

    public void StartMoving(Stack<Vector3> Mstack, Vector3 start)
    {
        stack = Mstack;
        moving = true;
        target = Mstack.Pop();
        origin = start;
    }

    private void MoveAlongPath()
    {
        t += Time.deltaTime;
        transform.position = (1-t) * origin + t * target + new Vector3(0, 0.5f, 0);
        if(t > MDurr)
        {
            t = 0;
            if( stack.Count == 0)
            {
                moving = false;
            } else
            {
                origin = target;
                target = stack.Pop();
            }
        }
    }
}
