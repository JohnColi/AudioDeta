using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleOnAmplitude : MonoBehaviour
{
    [SerializeField] AudioFFT _audioFFT;
    [SerializeField] float _amplituMultiplier = 2;
    [SerializeField] bool _uesBuffer;

    public bool _openEmision;
    Material _material;
    Vector3 ori_scale;

    private void Start()
    {
        ori_scale = transform.localScale;
        _material = GetComponent<MeshRenderer>().material;
    }

    private void Update()
    {
        if (_uesBuffer)
        {
            transform.localScale = ori_scale * (1 + _audioFFT._amplitudeBuffer * _amplituMultiplier);

            if (_openEmision)
            {
                Color c = new Color(_audioFFT._amplitudeBuffer, _audioFFT._amplitudeBuffer, _audioFFT._amplitudeBuffer);
                _material.SetColor("_Emission", c);
            }
        }
        else
        {
            transform.localScale = ori_scale * (1 + _audioFFT._amplitude * _amplituMultiplier);

            if (_openEmision)
            {
                Color c = new Color(_audioFFT._amplitude, _audioFFT._amplitude, _audioFFT._amplitude);
                _material.SetColor("_Emission", c);
            }
        }
    }
}
