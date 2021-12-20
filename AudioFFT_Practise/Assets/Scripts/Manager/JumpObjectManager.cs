using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpObjectManager : IStageManager
{
    bool isStartRecord;
    List<float> tempPitch = new List<float>();
    float hightValue;

    /// <summary> 計算緩衝的Data數量 </summary>
    [Range(30, 150)] public int averageCount = 30;
    /// <summary> 計算增加緩衝的Data數量 </summary>
    [Range(2, 150)] public int upAverageCount = 5;

    private void Start()
    {
        var pos = pitchObject.position;
        pos.y = low_y;
        oriPos = pos;
        pitchObject.position = pos;
    }

    private void Update()
    {
        if (isStartRecord)
        {
            if (tempPitch.Count >= averageCount)
                tempPitch.RemoveAt(0);

            tempPitch.Add(_outputDataVsSpectrumData.pitchValue);

            float p;
            //if (tempPitch.Count > 1 && tempPitch[tempPitch.Count - 1] > tempPitch[tempPitch.Count - 2])
            //{
            //    float sum = 0;
            //    int count = Mathf.Min(tempPitch.Count, upAverageCount);
            //    for (int i = 0; i < count; i++)
            //        sum += tempPitch[i];
            //    p = sum / upAverageCount;
            //}
            //else
            {
                float sum = 0;
                foreach (var f in tempPitch)
                    sum += f;

                p = sum / tempPitch.Count;
            }

            p = Mathf.Clamp(p, min_hz, max_hz);

            float y = low_y + ((hight_y - low_y) * (p - min_hz) / (max_hz - min_hz));
            var v2 = pitchObject.position;
            v2.y = Mathf.Lerp(v2.y, y, lerpT);
            v2.x += Time.deltaTime * speed;
            if (v2.x > 8.5f) v2.x = -8.5f;
            pitchObject.position = v2;
        }
    }

    public void StartMoveObj()
    {
        isStartRecord = true;
    }

    public void EndMoveObj()
    {
        isStartRecord = false;
        pitchObject.position = oriPos;
    }
}
