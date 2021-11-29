using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OutputDataVsSpectrumData : MonoBehaviour
{
    public AudioSource _audio;
    public bool ifShowBar;
    public Text display; // drag a GUIText here to show results

    [Header("Bar")]
    public Image rmsBar;
    public Image dbBar;
    public Image pitchBar;

    [Header("Buffer Bar")]
    public Image buf_rmsBar;
    public Image buf_dbBar;
    public Image buf_pitchBar;

    int qSamples = 1024;  // array size
    float refValue = 0.1f; // RMS value for 0 dB
    float threshold = 0.02f;      // minimum amplitude to extract pitch

    [HideInInspector] public float rmsValue;   // sound level - RMS
    [HideInInspector] public float dbValue;    // sound level - dB
    [HideInInspector] public float pitchValue; // sound pitch - Hz

    private float[] samples; // audio samples
    private float[] spectrum; // audio spectrum
    private float fSample;

    [Header("pro_")]
    [Range(0, 0.005f)]
    public float _buf_decrease = 0.003f;
    [Range(0.3f, 1.5f)]
    public float _buf_DesMulitiple = 1.2f;



    [HideInInspector] public float rmsBuffer = 0;
    float rmsBufferDecrease;

    [HideInInspector] public float dbBuffer = -180;
    float dbBufferDecrease = 0;

    [HideInInspector] public float pitchBuffer = 0;
    float pitchBufferDecrease = 0;


    private void Start()
    {
        samples = new float[qSamples];
        spectrum = new float[qSamples];
        fSample = AudioSettings.outputSampleRate;
        Debug.Log(fSample);
    }

    private void AnalyzeSound()
    {
        _audio.GetOutputData(samples, 0); // fill array with samples

        float sum = 0;
        for (int i = 0; i < qSamples; i++)
        {
            sum += samples[i] * samples[i]; // sum squared samples
        }
        rmsValue = Mathf.Sqrt(sum / qSamples); // rms = square root of average
        dbValue = 20 * Mathf.Log10(rmsValue / refValue); // calculate dB
        if (dbValue < -160) dbValue = -160; // clamp it to -160dB min

        // get sound spectrum
        _audio.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

        float maxV = 0;
        int maxN = 0;

        for (int i = 0; i < qSamples; i++)
        { // find max 
            if (spectrum[i] > maxV && spectrum[i] > threshold)
            {
                maxV = spectrum[i];
                maxN = i; // maxN is the index of max
            }
        }

        float freqN = maxN; // pass the index to a float variable
        if (maxN > 0 && maxN < qSamples - 1)
        {
            // interpolate index using neighbours
            var dL = spectrum[maxN - 1] / spectrum[maxN];
            var dR = spectrum[maxN + 1] / spectrum[maxN];
            freqN += 0.5f * (dR * dR - dL * dL);
        }
        pitchValue = freqN * (fSample / 2) / qSamples; // convert index to frequency
    }


    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    _audio.Play();
        //}
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    _audio.Stop();
        //}

        AnalyzeSound();
        BuffSound();

        if (display)
        {
            display.text = "RMS: " + rmsValue.ToString("F2") +
            " (" + dbValue.ToString("F1") + " dB)\n" +
            "Pitch: " + pitchValue.ToString("F0") + " Hz\n" +
            "RMS buf: " + rmsBuffer.ToString("F2") +
            " (" + dbBuffer.ToString("F1") + " dB)\n" +
            "Pitch: " + pitchBuffer.ToString("F0") + " Hz\n"
            ;
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
    }
}
