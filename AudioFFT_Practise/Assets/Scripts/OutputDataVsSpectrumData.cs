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

    //[Header("Bar")]
    //public Image rmsBar;
    //public Image dbBar;
    //public Image pitchBar;

    //[Header("Buffer Bar")]
    //public Image buf_rmsBar;
    //public Image buf_dbBar;
    //public Image buf_pitchBar;

    int qSamples = 1024;  // array size
    float refValue = 0.1f; // RMS value for 0 dB
    [Range(0.01f, 0.06f)]
    public float threshold = 0.02f;      // minimum amplitude to extract pitch
    /// <summary>
    /// poople voice hz ~1200hz
    /// </summary>
    public float hzHigh = 1200f;
    private int getSampleSize;

    [HideInInspector] public float rmsValue;   // sound level - RMS
    [HideInInspector] public float dbValue;    // sound level - dB
    [HideInInspector] public float pitchValue; // sound pitch - Hz

    private float[] samples; // audio samples
    [SerializeField] private float[] spectrum; // audio spectrum
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
        fSample = AudioSettings.outputSampleRate;   //48000
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

        float perSize = (fSample / 2) / qSamples;
        getSampleSize = (int)(hzHigh / perSize + 1);

        //get voice pitch
        float maxV = 0;
        int maxN = 0;
        for (int i = 0; i < getSampleSize; i++)
        { // find max 
            if (spectrum[i] > maxV && spectrum[i] > threshold)
            {
                maxV = spectrum[i];
                maxN = i; // maxN is the index of max
            }
        }
        float freqN = maxN; // pass the index to a float variable
        if (maxN > 0 && maxN < getSampleSize - 1)
        {
            // interpolate index using neighbours
            var dL = spectrum[maxN - 1] / spectrum[maxN];
            var dR = spectrum[maxN + 1] / spectrum[maxN];
            freqN += 0.5f * (dR * dR - dL * dL);
        }
        pitchValue = freqN * ((fSample / 2) / qSamples); // convert index to frequency
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
        var bandVol = BandVol(20f, 1200f);

        if (display)
        {
            string hz = "~" + (getSampleSize * (fSample / 2) / qSamples);

            string nor = "RMS: " + rmsValue.ToString("F2") +
            " (" + dbValue.ToString("F1") + " dB)\n" +
            "Pitch: " + pitchValue.ToString("F0") + " Hz\n";

            string buff = "RMS buf: " + rmsBuffer.ToString("F2") +
            " (" + dbBuffer.ToString("F1") + " dB)\n" +
            "Pitch: " + pitchBuffer.ToString("F0") + " Hz\n";

            display.text = "bandVol : " + bandVol + "\n" + hz + "\n" + nor;
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
