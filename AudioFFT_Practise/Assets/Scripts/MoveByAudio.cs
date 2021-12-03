using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveByAudio : MonoBehaviour
{
    public OutputDataVsSpectrumData _outputDataVsSpectrumData;
    [Range(1f, 10f)]
    public float _force;
    [Range(-180f, 0)]
    public float dB_Threshold = -30f;

    Material _material;
    Color ori_Color;

    private void Start()
    {
        _material = GetComponent<MeshRenderer>().material;
        ori_Color = _material.GetColor("_AlbdeoColor");
    }

    private void Update()
    {
        Move();
        Jump();
    }

    private void Move()
    {
        if (_outputDataVsSpectrumData.dbValue > dB_Threshold)
        {
            if (transform.position.x > 10f)
            {
                var v3 = transform.position;
                v3.x = -10f;
                transform.position = v3;
            }

            transform.Translate(Vector3.right * Time.deltaTime);
        }

        float factor = Mathf.Pow(2, (_outputDataVsSpectrumData.dbValue + 180) / 180 * 1);
        Color c = ori_Color * factor;
        _material.SetColor("_AlbdeoColor", c);
    }

    float hightest;
    private void Jump()
    {
        if (_outputDataVsSpectrumData.pitchBuffer >= 0)
        {
            float force = _outputDataVsSpectrumData.pitchBuffer / 1200f;
            var v3 = transform.position;
            v3.y = 9 * force;

            v3 = Vector3.Lerp(transform.position, v3, Time.deltaTime);
            transform.position = v3;
        }
    }
}