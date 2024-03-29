﻿using Assets.Scripts.New;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Enums;

public class Unit : MonoBehaviour
{
    private UnitStateEnum unitState;
    private PlayGrid playgrid;
    public Vector2Int startPos;
    public bool enemy;
    private bool gridSpawned;
    public int MaxHp;
    public int dmg;
    public int movement;
    private int CurrentHp;
    public int range;
    public int speed;
    public string name;

    public Sprite closeup;

    private Vector3Int cellPos;

    private Stack<Vector3> stack;
    private Vector3 target;
    private Vector3 origin;
    private float t;
    private HealthBar HpBar;
    private bool MoveLeft = true;
    private bool ActionLeft = true;
    private SpriteRenderer spriteRenderer;
    private Vector4 startcolor;
    public float mSpeed = 1.0f;
    protected bool attacking;
    private float importance_value = 10;
    private DamageNumberHandler damageNumberHandler;

    private int unitId;
    // Start is called before the first frame update
    void Start()
    {
        HpBar = GetComponentInChildren<HealthBar>();
        CurrentHp = MaxHp;
        spriteRenderer = GetComponent<SpriteRenderer>();
        damageNumberHandler = GetComponent<DamageNumberHandler>();
        startcolor = spriteRenderer.color;
        cellPos.x = startPos.x;
        cellPos.y = startPos.y;
    }

    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (unitState == UnitStateEnum.Moving)
        {
            MoveAlongPath();
        }
    }

    public void calculateImportance()
    {
        importance_value = range * dmg * 1 + (1 - (CurrentHp / MaxHp));
    }

    public float getImportance()
    {
        return importance_value;
    }

    public bool isEnemy()
    {
        return enemy;
    }

    public void Move(Vector3 pos)
    {
        transform.position = pos + new Vector3(0, 0.5f, 0);
    }

    public void StartMoving(Stack<Vector3> Mstack, Vector3 start, float speed, bool canAttack)
    {
        //mSpeed = speed;
        stack = Mstack;
        target = Mstack.Pop();
        origin = start;
        unitState = UnitStateEnum.Moving;
        attacking = canAttack;
    }

    private void MoveAlongPath()
    {
        t += Time.deltaTime * mSpeed;
        transform.position = (1 - t) * origin + t * target + new Vector3(0, 0.5f, 0);
        if (t > 1)
        {
            t = 0;
            if (stack.Count == 0)
            {
                FinishMoving();
            }
            else
            {
                origin = target;
                target = stack.Pop();
            }
        }
    }

    public virtual void FinishMoving()
    {
        MoveLeft = false;
        unitState = UnitStateEnum.Idle;
    }

    public bool TakeDamage(int dmg)
    {
        print("damage taken");
        CurrentHp -= dmg;
        if (CurrentHp <= 0)
        {
            Die();
            return true;
        }
        HpBar.SetHealth(MaxHp, CurrentHp);
        damageNumberHandler.CreateDamageText(dmg);
        return false;
    }

    public void Die()
    {
        GameObject.Find("GameHandler").GetComponent<CombatHandlerNew>().Resume();
        Destroy(gameObject);
    }

    public virtual bool Attack(Unit target, bool offensive)
    {
        //print(this.name + " attacks " + target.name + " for " + this.dmg);
        if (offensive)
        {
            ActionLeft = false;
            spriteRenderer.color = Color.gray;
        }
        unitState = UnitStateEnum.Idle;
        return target.TakeDamage(dmg);
    }

    public void NewTurn()
    {
        //print(this.name + " refreshed");
        MoveLeft = true;
        ActionLeft = true;
        spriteRenderer.color = startcolor;
    }

    public bool HasActionLeft()
    {
        return ActionLeft;
    }

    public void DoAction()
    {
        MoveLeft = false;
        ActionLeft = false;
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

    public int GetCurrentSpeed()
    {
        return speed;
    }

    public string GetName()
    {
        return name;
    }


    public string GetHealthString()
    {
        return CurrentHp.ToString();
    }

    public UnitStateEnum GetUnitState()
    {
        return unitState;
    }

    public int GetUnitId()
    {
        return unitId;
    }

    public void SetUnitId(int unitId)
    {
        this.unitId = unitId;
    }
}
