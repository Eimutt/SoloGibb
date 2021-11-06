using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private SpriteRenderer sprite;
    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        SetHealth(1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHealth(float max, float current)
    {
        float percentage = current/max;
        sprite.color = new Color(1 - percentage, percentage, 0, 1);
        transform.localScale = new Vector3(percentage, 1, 1);
        transform.localPosition = new Vector3((percentage-1)/2, 0, 0) * 0.3f;
    }
}
