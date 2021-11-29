using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioBandBuffer : MonoBehaviour
{
    [SerializeField] AudioFFT8 _audioFFT;
    [SerializeField] int bandID;
    [SerializeField] float _scaleMultiplier = 10;
    [SerializeField] bool _uesBuffer;
    [SerializeField] Material _material;

    Vector3 ori_pos;
    Color ori_Color;
    [Range(0, 10)]
    public float _emisionValue = 2;

    private void Start()
    {
        ori_pos = transform.position;
        _material = GetComponent<MeshRenderer>().material;
        ori_Color = _material.GetColor("_AlbdeoColor");
    }

    public void Update()
    {
        float y;

        if (_uesBuffer)
        {
            y = _audioFFT._audioBandBuffer[bandID] * _scaleMultiplier;

            float factor = Mathf.Pow(2, _emisionValue);
            Color c = ori_Color * (factor * _audioFFT._audioBandBuffer[bandID]);
            _material.SetColor("_AlbdeoColor", c);
        }
        else
        {
            y = _audioFFT._audioBand[bandID] * _scaleMultiplier;

            float factor = Mathf.Pow(2, _emisionValue);
            Color c = ori_Color * (factor * _audioFFT._audioBand[bandID]);
            _material.SetColor("_AlbdeoColor", c);
        }

        transform.localScale = new Vector3(1, y, 1);
        transform.position = ori_pos + new Vector3(0, y / 2, 0);
    }
}
