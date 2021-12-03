using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioRawDataBar : MonoBehaviour
{
    [SerializeField] AudioSource _audioSource;
    [SerializeField] Gradient _gradient;
    [SerializeField] Transform tsf_bar;
    public Image barPrb;
    [Range(0, 1)]
    public float h_space;
    public float maxScale = 400;
    public bool showBar;

    RectTransform[] bars;

    public float[] _samples_Stereo;
    public float[] _samples_Left;
    public float[] _samples_Right;
    [HideInInspector] public float[] _samples;

    private int spectrumDtatSize;

    [Header("Get Spectrum")]
    [SerializeField] EAudioType eAudioType;
    [SerializeField] FFTWindow FFT_Window;
    [SerializeField] EChannel eChannel;
    [SerializeField] EDataSize eDataSize = EDataSize._512;

    enum EAudioType
    {
        AudioSource,
        AudioListen,
    }

    enum EDataSize
    {
        Custom,
        _64 = 64,
        _128 = 128,
        _256 = 256,
        _512 = 512,
        _1024 = 1024,
        _2048 = 2048,
        _4096 = 4096,
        _8192 = 8192
    }

    enum EChannel
    {
        Stereo, Left, Right,
    }

    // Start is called before the first frame update
    void Start()
    {
        spectrumDtatSize = (int)eDataSize;
        _samples = new float[spectrumDtatSize];
        _samples_Stereo = new float[spectrumDtatSize];
        _samples_Left = new float[spectrumDtatSize];
        _samples_Right = new float[spectrumDtatSize];
        bars = new RectTransform[spectrumDtatSize];

        for (int i = 0; i < tsf_bar.childCount; i++)
            bars[i] = tsf_bar.GetChild(i).GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        GetSpectrumData();
        if (showBar) ShowBars();
    }

    private void ShowBars()
    {
        float[] samples;
        if (eChannel == EChannel.Left)
            samples = _samples_Left;
        else if (eChannel == EChannel.Right)
            samples = _samples_Right;
        else
            samples = _samples_Stereo;

        for (int i = 0; i < spectrumDtatSize; i++)
        {
            var v2 = bars[i].sizeDelta;
            v2.y = 1 + samples[i] * 10 * maxScale;
            bars[i].sizeDelta = v2;
        }
    }

    private void GetSpectrumData()
    {
        if (eAudioType == EAudioType.AudioSource)
        {
            _audioSource.GetSpectrumData(_samples_Right, 1, FFTWindow.Rectangular);
            _audioSource.GetSpectrumData(_samples_Left, 0, FFTWindow.Rectangular);
        }
        else
        {
            AudioListener.GetSpectrumData(_samples_Right, 1, FFTWindow.Rectangular);
            AudioListener.GetSpectrumData(_samples_Left, 0, FFTWindow.Rectangular);
        }

        if (eChannel == EChannel.Left)
        {
            _samples = _samples_Left;
        }
        else if (eChannel == EChannel.Right)
        {
            _samples = _samples_Right;
        }
        else if (eChannel == EChannel.Stereo)
        {
            for (int i = 0; i < spectrumDtatSize; i++)
                _samples_Stereo[i] = (_samples_Left[i] + _samples_Right[i]) / 2;
            _samples = _samples_Stereo;
        }
    }

    [ContextMenu("Creat Base Bar")]
    private void CreatBaseBar()
    {
        DeleteBaseBar();

        float w = barPrb.rectTransform.rect.width;
        float count = (int)eDataSize;

        for (int i = 0; i < count; i++)
        {
            var go = Instantiate(barPrb, tsf_bar);
            go.name = "Hz_" + i.ToString("000");
            go.transform.position = tsf_bar.position + (Vector3.right * (w + h_space) * (i - count / 2f));
            float time = 1f / count;
            go.GetComponent<Image>().color = _gradient.Evaluate(time * i);
        }
    }

    [ContextMenu("Delte Base Bar")]
    private void DeleteBaseBar()
    {
        int count = tsf_bar.childCount;
        for (int i = count - 1; i >= 0; i--)
        {
            DestroyImmediate(tsf_bar.GetChild(i).gameObject);
        }
    }
}
