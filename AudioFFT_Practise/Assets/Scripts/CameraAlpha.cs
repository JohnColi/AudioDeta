using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraAlpha : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] Slider _slider;
    
    void Start()
    {
        _slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    public void OnSliderValueChanged(float value)
    {
        Debug.Log(value);
        Color bgColor = mainCamera.backgroundColor;
        bgColor.a = value;
        mainCamera.backgroundColor = bgColor;
    }
}
