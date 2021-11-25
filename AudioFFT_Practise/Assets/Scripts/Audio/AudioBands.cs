using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioBands : MonoBehaviour
{
    //Audio Peer
    [SerializeField] FFTWindow FFT_Window;
    const int _spectrumDtatSize = 512;
    public int spectrumDtatSize { get { return _spectrumDtatSize; } }
    /// <summary> get sample count  </summary>
    const int _frequencyCount = 64;
    public int frequencyCount { get { return _frequencyCount; } }

    /// <summary>
    /// count min-max : 64-8192
    /// </summary>
    [HideInInspector] public float[] _samplesLeft;
    [HideInInspector] public float[] _samplesRight;

    [HideInInspector] public float[] _freoBand;
    [HideInInspector] public float[] _bandBuffer;
    /// <summary> 減少的量 </summary>
    [HideInInspector] float[] _bufferDecrease;

    [HideInInspector] public float[] _freqBandHighest;

    //Ranged Usable Values (ex:emision)
    [HideInInspector] public float[] _audioBand;
    [HideInInspector] public float[] _audioBandBuffer;

    // Get Average Amplitude
    [HideInInspector] public float _amplitude, _amplitudeBuffer;
    [HideInInspector] float _amplitudeHighest;

    [Header("Threshold")]
    public float _audioProfile;

    [Header("Buffer Parameter")]
    [Range(0.0001f, 0.007f)]
    public float _buf_decrease = 0.005f;
    /// <summary> 減少速度 </summary>
    [Range(1, 1.5f)]
    public float _buf_DesMulitiple = 1.2f;

    public EChannel eChannel;
    public enum EChannel { Stereo, Left, Right };

    private void Awake()
    {
        _samplesLeft = new float[_spectrumDtatSize];
        _samplesRight = new float[_spectrumDtatSize];
    }

    private void Start()
    {
        _freoBand = new float[_frequencyCount];
        _freqBandHighest = new float[_frequencyCount];
        _audioBand = new float[_frequencyCount];
        _audioBandBuffer = new float[_frequencyCount];
        //buffer
        _bandBuffer = new float[_frequencyCount];
        _bufferDecrease = new float[_frequencyCount];

        AudioProfile(_audioProfile);
    }

    private void Update()
    {
        GetSpectrumAudioSource();
        MakeFrequencyBands64();
        BandBuffer();
        CreatAudioBands();
        GetAmplitude();
    }

    private void AudioProfile(float audioProfile)
    {
        for (int i = 0; i < _freqBandHighest.Length; i++)
        {
            _freqBandHighest[i] = audioProfile;
        }
    }

    private void GetAmplitude()
    {
        float curAmplitude = 0;
        float curAmplitudeBuffer = 0;

        for (int i = 0; i < _audioBand.Length; i++)
        {
            curAmplitude += _audioBand[i];
            curAmplitudeBuffer += _audioBandBuffer[i];
        }

        if (curAmplitude > _amplitudeHighest)
            _amplitudeHighest = curAmplitude;

        _amplitude = curAmplitude / _amplitudeHighest;
        _amplitudeBuffer = curAmplitudeBuffer / _amplitudeHighest;
    }

    private void CreatAudioBands()
    {
        for (int i = 0; i < _freoBand.Length; i++)
        {
            if (_freoBand[i] > _freqBandHighest[i])
                _freqBandHighest[i] = _freoBand[i];

            _audioBand[i] = (_freoBand[i] / _freqBandHighest[i]);
            _audioBandBuffer[i] = (_bandBuffer[i] / _freqBandHighest[i]);
        }
    }

    //Decrease
    private void BandBuffer()
    {
        for (int g = 0; g < _freoBand.Length; g++)
        {
            if (_freoBand[g] > _bandBuffer[g])
            {
                _bandBuffer[g] = _freoBand[g];
                _bufferDecrease[g] = _buf_decrease; //0.005f
            }

            if (_freoBand[g] < _bandBuffer[g])
            {
                _bandBuffer[g] -= _bufferDecrease[g];
                _bufferDecrease[g] *= _buf_DesMulitiple;   //*1.2f
            }
        }
    }

    private void GetSpectrumAudioSource()
    {
        // 0 left channel, 1 right channel.
        AudioListener.GetSpectrumData(_samplesLeft, 0, FFTWindow.Rectangular);
        AudioListener.GetSpectrumData(_samplesRight, 1, FFTWindow.Rectangular);
    }

    /// <summary> Bands </summary>
    private void MakeFrequencyBands64()
    {
        /**
         * 22050 / 512 = 43 hertz per sample
         *
         * 20 - 60 hertz
         * 60 - 250 hertz
         * 250 - 500 hertz
         * 500 - 2000 hertz
         * 2000 - 4000 hertz
         * 4000 - 6000 hertz
         * 6000 - 20000 hertz
         * 
         * 
         *  0-15 = per 1 sample, totla sample = 16.
         *  16-31 = 2 sample = 32
         *  32-39 = 4 sample = 32
         *  40-47 = 6 sample = 48
         *  48-55 = 16 sample = 128
         *  56-63 = 32 sample = 256 +
         *                      ---
         *                      566
         **/

        int count = 0;
        int sampleCount = 1;
        int power = 0;

        for (int i = 0; i < 64; i++)
        {
            float average = 0;
            if (i == 16 || i == 32 || i == 40 || i == 48 || i == 56)
            {
                power++;
                sampleCount = (int)Mathf.Pow(2, power);
                if (power == 3) sampleCount -= 2;   //6 sample per
            }

            for (int j = 0; j < sampleCount; j++)
            {
                if (eChannel == EChannel.Right)
                    average += _samplesRight[count] * (count + 1);
                else if (eChannel == EChannel.Left)
                    average += _samplesLeft[count] * (count + 1);
                else
                    average += (_samplesLeft[count] + _samplesRight[count]) / 2 * (count + 1);

                count++;
            }

            average /= count;
            _freoBand[i] = average;
        }
    }

    [ContextMenu("Set Audio Profile")]
    private void SetProfile()
    {
        AudioProfile(_audioProfile);
    }

}
