using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetVoiceVolume : MonoBehaviour
{
    public OutputDataVsSpectrumData _outputDataVsSpectrumData;

    [Range(50, 500)]
    public float rmsScale = 80;
    [Range(1, 5)]
    public float dBScale = 1;
    [Range(1000, 5000)]
    public float volumScale = 150;

    public Image[] rms = new Image[2];
    public Image[] dB = new Image[2];
    public Image[] volume = new Image[2];
    public Image[] pitch = new Image[2];

    private void Update()
    {
        SetImageHight(_outputDataVsSpectrumData.rmsValue, _outputDataVsSpectrumData.rmsBuffer, rms, rmsScale);

        float v = _outputDataVsSpectrumData.dbValue ;
        float b = _outputDataVsSpectrumData.dbBuffer ;
        SetImageHight(v, b, dB);
        SetImageHight(_outputDataVsSpectrumData.volumeValue, _outputDataVsSpectrumData.volumeBuffer, volume, volumScale);

        //float v2 = Mathf.Log(v, 2);
        //float b2 = Mathf.Log(b, 2);
        //SetImageHight(v2, b2, pitch, dBScale);
    }

    private void SetImageHight(float v, float buf, Image[] images, float scale = 1)
    {
        ChangeDeltaSizeY(v, images[0].GetComponent<RectTransform>(), scale);
        ChangeDeltaSizeY(buf, images[1].GetComponent<RectTransform>(), scale);
    }

    private void ChangeDeltaSizeY(float y, RectTransform rtsf, float scale)
    {
        Vector2 v2 = rtsf.sizeDelta;
        v2.y = y * scale;
        rtsf.sizeDelta = v2;

        rtsf.GetChild(0).GetComponent<Text>().text = y.ToString("0.00");
    }
}
