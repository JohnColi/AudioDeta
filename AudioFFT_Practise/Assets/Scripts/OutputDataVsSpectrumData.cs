using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OutputDataVsSpectrumData : MonoBehaviour
{
    public AudioSource _audio;
    public Text display; // drag a GUIText here to show results

    int qSamples = 1024;  // array size
    float refValue = 0.1f; // RMS value for 0 dB
    [Range(0.01f, 0.06f)]
    public float threshold = 0.02f;      // minimum amplitude to extract pitch

    /// <summary> poople voice hz ~1200hz </summary>
    public float maxHz = 1200f;
    public float minHz = 0f;

    private int maxSampleSize;
    private int minSampleSize;

    [HideInInspector] public float rmsValue;   // sound level - RMS
    [HideInInspector] public float dbValue;    // sound level - dB
    [HideInInspector] public float pitchValue; // sound pitch - Hz
    [HideInInspector] public float volumeValue;

    private float[] outputSamples; // audio samples
    [SerializeField] private float[] spectrum; // audio spectrum
    private float fSample;

    [Header("pro_")]
    [Range(0, 0.005f)]
    public float _buf_decrease = 0.003f;
    [Range(0.3f, 1.5f)]
    public float _buf_DesMulitiple = 1.2f;


    [HideInInspector]
    public float rmsBuffer = 0;
    float rmsBufferDecrease;

    [HideInInspector]
    public float dbBuffer = -160;
    float dbBufferDecrease = 0;

    [HideInInspector]
    public float pitchBuffer = 0;
    float pitchBufferDecrease = 0;

    [HideInInspector]
    public float volumeBuffer = 0;
    float volumeBufferDecrease = 0;

    private void Start()
    {
        outputSamples = new float[qSamples];
        spectrum = new float[qSamples];
        fSample = AudioSettings.outputSampleRate;   //48000
    }

    private void AnalyzeSound()
    {
        float perSize = (fSample / 2) / qSamples;
        maxSampleSize = (int)(maxHz / perSize + 1);
        minSampleSize = (int)(minHz / perSize);

        // get rms and dB
        _audio.GetOutputData(outputSamples, 0); // fill array with samples

        float sum = 0;
        for (int i = minSampleSize; i < maxSampleSize; i++)
        {
            sum += outputSamples[i] * outputSamples[i]; // sum squared samples
        }
        rmsValue = Mathf.Sqrt(sum / (maxSampleSize - minSampleSize)); // rms = square root of average
        dbValue = 20 * Mathf.Log10(rmsValue / refValue); // calculate dB
        if (dbValue < -160) dbValue = -160; // clamp it to -160dB min


        // get voice pitch
        _audio.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
        float maxV = 0;
        int maxN = 0;
        for (int i = 0; i < maxSampleSize; i++)
        { // find max 
            if (spectrum[i] > maxV && spectrum[i] > threshold)
            {
                maxV = spectrum[i];
                maxN = i; // maxN is the index of max
            }
        }
        float freqN = maxN; // pass the index to a float variable
        if (maxN > 0 && maxN < maxSampleSize - 1)
        {
            // interpolate index using neighbours
            var dL = spectrum[maxN - 1] / spectrum[maxN];
            var dR = spectrum[maxN + 1] / spectrum[maxN];
            freqN += 0.5f * (dR * dR - dL * dL);
        }
        pitchValue = freqN * ((fSample / 2) / qSamples); // convert index to frequency
    }

    private void BandVol()
    {
        volumeValue = BandVol(0f, maxHz);
    }

    private float BandVol(float fLow, float fHigh)
    {
        float fMax = fSample / 2;
        fLow = Mathf.Clamp(fLow, 20, fMax); // limit low...
        fHigh = Mathf.Clamp(fHigh, fLow, fMax); // and high frequencies
                                                // get spectrum
                                                //_audio.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
        int n1 = (int)Mathf.Floor(fLow * qSamples / fMax);
        int n2 = (int)Mathf.Floor(fHigh * qSamples / fMax);

        float sum = 0;
        // average the volumes of frequencies fLow to fHigh
        for (int i = n1; i <= n2; i++)
        {
            sum += spectrum[i];
        }
        return sum / (n2 - n1 + 1);
    }

    private void Update()
    {
        AnalyzeSound();
        BandVol();
        BuffSound();

        if (display)
        {
            string hz = "~" + (maxSampleSize * (fSample / 2) / qSamples);

            string nor = "RMS: " + rmsValue.ToString("F2") +
            " (" + dbValue.ToString("F1") + " dB)\n" +
            "Pitch: " + pitchValue.ToString("F0") + " Hz\n";

            string buff = "RMS buf: " + rmsBuffer.ToString("F2") +
            " (" + dbBuffer.ToString("F1") + " dB)\n" +
            "Pitch: " + pitchBuffer.ToString("F0") + " Hz\n";

            display.text = "bandVol : " + volumeValue + "\n" + hz + "\n" + nor;
        }
    }


    private void BuffSound()
    {
        //rms
        if (rmsValue > rmsBuffer)
        {
            rmsBuffer = rmsValue;
            rmsBufferDecrease = _buf_decrease;
        }
        if (rmsValue < rmsBuffer)
        {
            rmsBuffer -= rmsBufferDecrease;
            rmsBufferDecrease *= _buf_DesMulitiple;
        }

        //db
        if (dbValue > dbBuffer)
        {
            dbBuffer = dbValue;
            dbBufferDecrease = _buf_decrease;
        }
        if (dbValue < dbBuffer)
        {
            dbBuffer -= dbBufferDecrease;
            dbBufferDecrease *= _buf_DesMulitiple;
        }

        //pitch
        if (pitchValue > pitchBuffer)
        {
            pitchBuffer = pitchValue;
            pitchBufferDecrease = _buf_decrease;
        }
        if (pitchValue < pitchBuffer)
        {
            pitchBuffer -= pitchBufferDecrease;
            pitchBufferDecrease *= _buf_DesMulitiple;
        }

        // volume+
        if (volumeValue > volumeBuffer)
        {
            volumeBuffer = volumeValue;
            volumeBufferDecrease = _buf_decrease;
        }
        if (volumeValue < volumeBuffer)
        {
            volumeBuffer -= volumeBufferDecrease;
            volumeBufferDecrease *= _buf_DesMulitiple;
        }
    }
}
