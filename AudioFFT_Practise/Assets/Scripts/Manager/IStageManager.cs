using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IStageManager : MonoBehaviour
{
    public OutputDataVsSpectrumData _outputDataVsSpectrumData;
    public Transform pitchObject;

    [Range(1, 5)] public float speed;
    [Range(0.1f, 1)] public float lerpT = 1;

    /// <summary> pitch obj 最高位置 </summary>
    public float hight_y;
    /// <summary> pitch obj 最低位置 </summary>
    public float low_y;
    /// <summary> 顯示最大的hz </summary>
    public float max_hz = 660;
    /// <summary> 顯示最大的hz </summary>
    public float min_hz = 100;

    protected Vector3 oriPos;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (pitchObject)
        {
            var pos = pitchObject.position;
            pos.y = low_y;
            Gizmos.DrawWireSphere(pos, 0.2f);
        }

        Gizmos.DrawLine(new Vector3(-10, hight_y, 0), new Vector3(10, hight_y, 0));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(-10, low_y, 0), new Vector3(10, low_y, 0));
    }
}
