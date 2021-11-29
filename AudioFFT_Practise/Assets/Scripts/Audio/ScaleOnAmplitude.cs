using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class ScaleOnAmplitude : MonoBehaviour
{
    [SerializeField] AudioFFT8 _audioFFT;
    [SerializeField] float _amplituMultiplier = 2;
    [SerializeField] bool _uesBuffer;

    [Header("Emision")]
    [Range(0, 10)]
    public float _emisionValue = 1;
    public bool _openEmision = true;

    Material _material;
    Vector3 ori_scale;
    Color ori_Color;
    private void Start()
    {
        ori_scale = transform.localScale;
        _material = GetComponent<MeshRenderer>().material;

        ori_Color = _material.GetColor("_AlbdeoColor");
    }

    private void Update()
    {
        if (_uesBuffer)
        {
            transform.localScale = ori_scale * (1 + _audioFFT._amplitudeBuffer * _amplituMultiplier);

            if (_openEmision)
            {
                float factor = Mathf.Pow(2, _emisionValue);
                Color c = ori_Color * (factor * _audioFFT._amplitudeBuffer);
                _material.SetColor("_AlbdeoColor", c);
            }
        }
        else
        {
            transform.localScale = ori_scale * (1 + _audioFFT._amplitude * _amplituMultiplier);

            if (_openEmision)
            {
                float factor = Mathf.Pow(2, _emisionValue);
                Color c = ori_Color * (factor * _audioFFT._amplitude);
                _material.SetColor("_AlbdeoColor", c);
            }
        }
    }
}
