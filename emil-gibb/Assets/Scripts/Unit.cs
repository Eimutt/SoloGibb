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
    }
    
    void StartMove()
    {
        transform.position = playgrid.MoveToCell(this, new Vector3Int(startPos.x, startPos.y, 0)) + new Vector3(0, 0.5f, 0);
    }

    public void Move(Vector3 pos)
    {
        transform.position = pos + new Vector3(0, 0.5f, 0);
    }
}
