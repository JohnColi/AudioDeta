using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathTest : MonoBehaviour
{
    [Range(0, 1)]
    public float t;
    private void Start()
    {
        //for (int i = 0; i < 13; i++)
        //{
        //    Debug.Log(string.Format("i = {0} , {1}", i, Mathf.Pow(1.05946f, i)));
        //}

        float[] values = { 0.002f, 0.157f, 2.33f, 4.58f };
        short[] intData = new short[values.Length];


        for (int i = 0; i < values.Length; i++)
        {
            intData[i] = (short)values[i];
            byte[] bytes = BitConverter.GetBytes(intData[i]);
            UnityEngine.Debug.LogFormat("value:{0}, {1}, {2}", values[i], intData[i], BitConverter.ToString(bytes));

            intData[i] = (short)(values[i] * 32767);
            bytes = BitConverter.GetBytes(intData[i]);
            UnityEngine.Debug.LogFormat("value:{0}, {1}, {2}", values[i], intData[i], BitConverter.ToString(bytes));
        }
    }

    private void Update()
    {
        //float f = Mathf.Lerp(1, 10, t);
        //Debug.Log(f);
    }
}
