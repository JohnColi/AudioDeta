using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
public class AudioFrequencyBands : MonoBehaviour
{
    [SerializeField] AudioFFT8 _audioFFT;
    [SerializeField] int bandID;
    [SerializeField] float _scaleMultiplier = 10;
    [SerializeField] bool _uesBuffer;

    Vector3 ori_pos;

    private void Start()
    {
        ori_pos = transform.position;
    }

    public void Update()
    {
        float y;

        if (_uesBuffer)
        {
            y = _audioFFT._bandBuffer[bandID] * _scaleMultiplier;
        }
        else
        {
            y = _audioFFT._freoBand[bandID] * _scaleMultiplier;
        }
        transform.localScale = new Vector3(1, y, 1);
        transform.position = ori_pos + new Vector3(0, y / 2, 0);
    }
}
