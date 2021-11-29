using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathTest : MonoBehaviour
{
    public Color lerpedColor = Color.white;
    public Color C1, C2;

    [ColorUsageAttribute(true, true)]
    public Color colour;

    void Update()
    {
        lerpedColor = Color.Lerp(C1, C2, Mathf.PingPong(Time.time, 1));
    }
}
