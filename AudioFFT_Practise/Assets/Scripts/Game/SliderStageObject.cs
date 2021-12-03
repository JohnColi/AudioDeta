using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 拖拉音
/// </summary>
public class SliderStageObject : StageObject
{
    [Range(0f, 1f)]
    public float filledSlider;

    bool isStartPitch;
    Vector2 edge_l;
    Vector2 edge_r;

    void Start()
    {
        var spr = GetComponent<SpriteRenderer>();
        _material = spr.material;

        var size = spr.bounds.size;
        var pos = transform.position;
        pos.x -= size.x / 2;
        edge_l = pos;
        pos = transform.position;
        pos.x += size.x / 2;
        edge_r = pos;

        FilledSlider(0);
    }

    private void StartSlider()
    {
        Debug.Log("Start Slider");
        isStartPitch = true;
    }
    private void EndSlider()
    {
        Debug.Log("End Slider");
        isStartPitch = false;
        FilledSlider(1);
    }

    /// <summary>
    /// 開始
    /// </summary>
    /// <param name="inputHz"></param>
    public void FilledSlider(float value)
    {
        filledSlider = value;
        _material.SetFloat("_Property", filledSlider);

        if (filledSlider == 1)
        {
            Debug.Log("Stage Complete");
            completeEvent.Invoke();
        }
    }

    private void ResetState()
    {
        filledSlider = 0;
        _material.SetFloat("_Property", filledSlider);
        isStartPitch = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        StartSlider();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isStartPitch)
        {
            float v = collision.transform.position.x - edge_l.x;
            v = v / (edge_r.x - edge_l.x);
            FilledSlider(v);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        EndSlider();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("collider enter");
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
