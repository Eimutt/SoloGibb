using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private PlayGrid playgrid;
    public Vector2Int startPos;
    public bool enemy;
    private bool gridSpawned;
    public int MaxHp;
    public int dmg;
    public int movement;
    private int CurrentHp;
    public int range;

    private Vector3Int cellPos;


    private bool moving;
    private Stack<Vector3> stack;
    private Vector3 target;
    private Vector3 origin;
    private float t;
    private HealthBar HpBar;
    private bool MoveLeft = true;
    private bool ActionLeft = true;
    private SpriteRenderer spriteRenderer;
    private Vector4 startcolor;
    private float mSpeed;
    private bool attacking;
    private float importance_value;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {
        HpBar = GetComponentInChildren<HealthBar>();
        CurrentHp = MaxHp;
        spriteRenderer = GetComponent<SpriteRenderer>();
        startcolor = spriteRenderer.color;
        cellPos.x = startPos.x;
        cellPos.y = startPos.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            MoveAlongPath();
        }
    }

    public void calculateImportance()
    {
        importance_value = range * dmg * 1 + (1 - (CurrentHp / MaxHp));
        print(importance_value);
    }

    public void Move(Vector3 pos)
    {
        transform.position = pos + new Vector3(0, 0.5f, 0);
    }

    public void StartMoving(Stack<Vector3> Mstack, Vector3 start, float speed)
    {
        mSpeed = speed;
        stack = Mstack;
        moving = true;
        target = Mstack.Pop();
        origin = start;
        MoveLeft = false;
    }

    private void MoveAlongPath()
    {
        t += Time.deltaTime * mSpeed;
        transform.position = (1-t) * origin + t * target + new Vector3(0, 0.5f, 0);
        if(t > 1)
        {
            t = 0;
            if( stack.Count == 0)
            {
                moving = false;
                GameObject.Find("GameHandler").GetComponent<CombatHandler>().Resume();
            } else
            {
                origin = target;
                target = stack.Pop();
            }
        }
    }

    public bool TakeDamage(int dmg)
    {
        CurrentHp -= dmg;
        if(CurrentHp <= 0)
        {
            Die();
            return true;
        }
        HpBar.SetHealth(MaxHp, CurrentHp);
        return false;
    }

    public void Die()
    {
        GameObject.Find("GameHandler").GetComponent<CombatHandler>().Resume();
        Destroy(gameObject);
    }

    public bool Attack(Unit target)
    {
        ActionLeft = false;
        spriteRenderer.color = Color.gray;
        return target.TakeDamage(dmg);
    }

    public void NewTurn()
    {
        MoveLeft = true;
        ActionLeft = true;
        spriteRenderer.color = startcolor;
    }

    public bool HasActionLeft()
    {
        return ActionLeft;
    }

    public bool HasMoveLeft()
    {
        return MoveLeft;
    }

    public Vector3Int GetCellPos()
    {
        return cellPos;
    }
    public void SetCellPos(Vector3Int newPos)
    {
        cellPos = newPos;

    }

    public int GetMovement()
    {
        return movement;
    }

    public int GetRange()
    {
        return range;
    }
}
