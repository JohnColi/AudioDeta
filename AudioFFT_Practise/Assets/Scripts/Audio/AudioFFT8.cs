using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFFT8 : MonoBehaviour
{
    public float _maxScale = 10;

    //Audio Peer
    [SerializeField] FFTWindow FFT_Window;
    const int spectrumDtatSize = 512;
    const int frequencyCount = 8;
    const int bufferCount = 8;

    /// <summary>
    /// count min-max : 64-8192
    /// </summary>
    [HideInInspector] public float[] _samples;
    float[] _samplesLeft;
    float[] _samplesRight;

    [HideInInspector] public float[] _freoBand;
    [HideInInspector] public float[] _bandBuffer;
    /// <summary> 減少的量 </summary>
    [HideInInspector] float[] _bufferDecrease;

    [HideInInspector] public float[] _freqBandHighest = new float[8];

    //Ranged Usable Values (ex:emision)
    [HideInInspector] public float[] _audioBand = new float[8];
    [HideInInspector] public float[] _audioBandBuffer = new float[8];

    // Get Average Amplitude
    [HideInInspector] public float _amplitude, _amplitudeBuffer;
    [HideInInspector] float _amplitudeHighest;

    [Header("Threshold")]
    public float _audioProfile;

    [Header("Cycle Parameter")]
    public float cycle_radius = 90;
    public Vector3 yellowBarScale = Vector3.one;

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
        _samples = new float[spectrumDtatSize];
        _samplesLeft = new float[spectrumDtatSize];
        _samplesRight = new float[spectrumDtatSize];
    }

    private void Start()
    {
        _freoBand = new float[frequencyCount];
        _bandBuffer = new float[bufferCount];
        _bufferDecrease = new float[bufferCount];

        AudioProfile(_audioProfile);
    }

    private void Update()
    {
        GetSpectrumAudioSource();
        MakeFrequencyBands();
        BandBuffer();
        CreatAudioBands();
        GetAmplitude();
    }

    private void AudioProfile(float audioProfile)
    {
        for (int i = 0; i < 8; i++)
        {
            _freqBandHighest[i] = audioProfile;
        }
    }

    private void GetAmplitude()
    {
        float curAmplitude = 0;
        float curAmplitudeBuffer = 0;
        for (int i = 0; i < 8; i++)
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
        for (int i = 0; i < 8; i++)
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
        for (int g = 0; g < 8; g++)
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
    private void MakeFrequencyBands()
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
         * ID
         * 0. 2 = 86 hertz, 0-86 hertz
         * 1. 4 = 172 hertz, 87-258 hertz
         * 2. 8 = 344 hertz, 259-602 hertz
         * 3. 16 = 688 hertz, 603-1290 hertz
         * 4. 32 = 1376 hertz, 1291-2666 hertz
         * 5. 64 = 2752 hertz, 2667-5418 hertz
         * 6. 128 = 5504 hertz, 5419-10922 hertz
         * 7. 256 = 11008 hertz, 10923-21930 hertz
         * total 510, lose 2 byte.
         **/

        int count = 0;
        for (int i = 0; i < 8; i++)
        {
            float average = 0;
            int sampleCount = (int)Mathf.Pow(2, i) * 2;

            if (i == 7)
                sampleCount += 2;

            for (int j = 0; j < sampleCount; j++)
            {
                if (eChannel == EChannel.Right)
                    average += _samplesRight[count] * (count + 1);
                else if (eChannel == EChannel.Left)
                    average += _samplesLeft[count] * (count + 1);
                else
                    average += (_samplesLeft[count] + _samplesRight[count])/2 * (count + 1);

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
