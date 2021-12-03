using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class CheckStageObjcet : StageObject
{
    bool isStartCheck;
    float timer;
    public Color completeColor = Color.white;


    private void Start()
    {
        _material = GetComponent<SpriteRenderer>().material;
    }

    private void Update()
    {
        if (isStartCheck)
        {
            timer += Time.deltaTime;
            if (timer >= 0.2f)
            {
                Complete();
                EndCheck();
            }
        }
    }

    private void Complete()
    {
        Debug.Log("Complete");
        var c = completeColor * Mathf.Pow(2, 0.5f);
        _material.SetColor("_Color", c);
    }

    private void StartCheck()
    {
        isStartCheck = true;
        timer = 0f;
    }

    private void EndCheck()
    {
        isStartCheck = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        StartCheck();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        EndCheck();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        var spr = GetComponent<SpriteRenderer>();
        var size = spr.bounds.size;
        Gizmos.DrawWireCube(transform.position, size);
        float r = 0.05f;

        //left
        var pos = transform.position;
        pos.x -= size.x / 2;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos, r);

        //right
        pos = transform.position;
        pos.x += size.x / 2;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(pos, r);
    }
}
