using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioBandBuffer : MonoBehaviour
{
    [SerializeField] AudioFFT _audioFFT;
    [SerializeField] int bandID;
    [SerializeField] float _scaleMultiplier = 10;
    [SerializeField] bool _uesBuffer;
    [SerializeField] Material _material;

    Vector3 ori_pos;

    private void Start()
    {
        ori_pos = transform.position;
        _material = GetComponent<MeshRenderer>().material;
    }

    public void Update()
    {
        float y;

        if (_uesBuffer)
        {
            y = _audioFFT._audioBandBuffer[bandID] * _scaleMultiplier;
            Color c = new Color(_audioFFT._audioBandBuffer[bandID], _audioFFT._audioBandBuffer[bandID], _audioFFT._audioBandBuffer[bandID]);
            _material.SetColor("_Emission", c);
        }
        else
        {
            y = _audioFFT._audioBand[bandID] * _scaleMultiplier;
            Color c = new Color(_audioFFT._audioBand[bandID], _audioFFT._audioBand[bandID], _audioFFT._audioBand[bandID]);
            _material.SetColor("_Emission", c);
        }
        transform.localScale = new Vector3(1, y, 1);
        transform.position = ori_pos + new Vector3(0, y / 2, 0);
    }
}
