using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageObjectManager : IStageManager
{
    bool isStartVoice;
    float dB_threshold = -30f;

    // Start is called before the first frame update
    void Start()
    {
        var pos = pitchObject.position;
        pos.y = low_y;
        oriPos = pos;

        StartVoice();
    }

    // Update is called once per frame
    void Update()
    {
        if (isStartVoice)
        {
            if (_outputDataVsSpectrumData.rmsValue > 0.01f)
            {
                float y = low_y + ((hight_y - low_y) * (_outputDataVsSpectrumData.pitchValue - min_hz) / (max_hz - min_hz));
                var v2 = pitchObject.position;
                y = Mathf.Clamp(y, low_y, hight_y);
                y = Mathf.Lerp(v2.y, y, lerpT);
                v2.y = y;
                v2.x += Time.deltaTime * speed;

                if (v2.x > 8.5f) v2.x = -8.5f;
                pitchObject.position = v2;
            }
        }
    }

    #region Pitch Object
    public void StartVoice()
    {
        isStartVoice = true;
    }

    public void StopVoice()
    {
        isStartVoice = false;
        //Reset point
    }
    #endregion
}
