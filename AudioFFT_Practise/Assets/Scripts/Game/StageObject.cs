using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageObject : MonoBehaviour
{
    public EMusicNote musciNote = EMusicNote.A3;
    public Action completeEvent;
    protected Material _material;

    [ContextMenu("Move To Pitch Hz")]
    protected void MoveToPitchHz()
    {
        var _stageManager = FindObjectOfType<IStageManager>();

        float pitch = (float)musciNote;
        float t = (pitch - _stageManager.min_hz) / (_stageManager.max_hz - _stageManager.min_hz);

        float y = Mathf.Lerp(_stageManager.low_y, _stageManager.hight_y, t);

        var pos = transform.position;
        pos.y = y;
        transform.position = pos;
    }
}
