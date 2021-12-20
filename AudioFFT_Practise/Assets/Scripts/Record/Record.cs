using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
#if UNITY_ANDROID 
using UnityEngine.Android;
#endif

[RequireComponent(typeof(AudioSource))]
public class Record : MonoBehaviour
{
    AudioSource _audio;
    /// <summary> 麥克風數量 </summary>
    int deviceCount;
    string devices;
    int sec = 90;

    [SerializeField] AudioMixerGroup microPhoneMixerGruop;
    [SerializeField] AudioMixerGroup masterMixerGruop;

    private void Awake()
    {
#if UNITY_ANDROID && UNITY_EDITOR
        if (Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Debug.LogError("沒有麥克風權限");
            Permission.RequestUserPermission(Permission.Microphone);
        }

        if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            Debug.LogError("沒有寫入權限");
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }
#endif
    }

    void Start()
    {
        _audio = GetComponent<AudioSource>();
        SetMicroPhone();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartRecord();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            StopRecord();
        }
    }

    void PrintLog(string log)
    {
        Debug.Log(log);
    }

    private void SetMicroPhone()
    {
        string[] ms = Microphone.devices;
        deviceCount = ms.Length;

        if (deviceCount == 0)
        {
            PrintLog("no microphone found");
            devices = "";
        }
        else
        {
            devices = ms[0];
            PrintLog("麥克風 : " + deviceCount);
            PrintLog("devices : " + devices);
        }
    }

    #region button event
    public void StartRecord()
    {
        _audio.Stop();
        _audio.clip = null;
        _audio.clip = Microphone.Start(devices, false, sec, AudioSettings.outputSampleRate);
        _audio.outputAudioMixerGroup = microPhoneMixerGruop;
        PrintLog(string.Format("開始錄音 {0} {1} {2} {3}", devices, false, sec, AudioSettings.outputSampleRate));

        while (!(Microphone.GetPosition(null) > 0)) { }
        _audio.Play();
    }

    public void StopRecord()
    {
        if (!Microphone.IsRecording(null))
        {
            return;
        }
        PrintLog("關閉錄音");

        _audio.outputAudioMixerGroup = masterMixerGruop;
        EndRecording(_audio, devices);
        Microphone.End(devices);
        _audio.Stop();
    }

    public void PrintRecord()
    {
        if (Microphone.IsRecording(null))
        {
            return;
        }

        byte[] data = GetClipData();
        string slog = "total length:" + data.Length + " time:" + _audio.time;
        PrintLog(slog);
    }

    public void PlayRecord()
    {
        if (_audio.clip == null)
        {
            return;
        }

        _audio.outputAudioMixerGroup = masterMixerGruop;
        PrintLog("撥放錄音:" + _audio.clip.name);
        _audio.Play();
    }

    public void SaveRecord()
    {
        if (_audio.clip == null)
            return;

        string name = _audio.clip.name + "_" + System.DateTime.Now.ToString("yyyyMMddhhmmss");
        PrintLog("儲存錄音:" + name);
        SaveAudioClip.Save(name, _audio.clip);
    }

    public void LoadRecord()
    {
        PrintLog("讀取錄音");
        SaveAudioClip.LoadAudio("Level_1.wav", PlayAudioClip);
    }
    #endregion

    void EndRecording(AudioSource audS, string deviceName)
    {
        //Capture the current clip data
        AudioClip recordedClip = audS.clip;
        var position = Microphone.GetPosition(deviceName);

        Debug.Log(string.Format("deviceName : {0} / position : {1}", deviceName, position));

        var soundData = new float[recordedClip.samples * recordedClip.channels];
        recordedClip.GetData(soundData, 0);

        //Create shortened array for the data that was used for recording
        var newData = new float[position * recordedClip.channels];

        Debug.Log("" + newData.Length);

        //Copy the used samples to a new array
        for (int i = 0; i < newData.Length; i++)
        {
            newData[i] = soundData[i];
        }

        //One does not simply shorten an AudioClip, so we make a new one with the appropriate length
        var newClip = AudioClip.Create(recordedClip.name, position, recordedClip.channels, recordedClip.frequency, false);

        //Give it the data from the old clip
        newClip.SetData(newData, 0);

        //Replace the old clip
        AudioClip.Destroy(recordedClip);
        audS.clip = newClip;
    }

    private void PlayAudioClip(AudioClip clip)
    {
        if (clip == null)
        {
            PrintLog("讀取失敗");
            return;
        }

        _audio.clip = clip;
        PrintLog("讀取完成 開始撥放 " + _audio.clip.name);

        _audio.Play();
    }

    public byte[] GetClipData()
    {
        if (_audio.clip == null)
        {
            Debug.Log("GetClipData audio.clip is null");
            return null;
        }

        float[] samples = new float[_audio.clip.samples];

        _audio.clip.GetData(samples, 0);

        //bytesData array is twice the size of
        //dataSource array because a float converted in Int16 is 2 bytes.
        byte[] outData = new byte[samples.Length * 2];

        int rescaleFactor = 32767; //to convert float to Int16

        for (int i = 0; i < samples.Length; i++)
        {
            short temshort = (short)(samples[i] * rescaleFactor);

            byte[] temdata = System.BitConverter.GetBytes(temshort);

            outData[i * 2] = temdata[0];
            outData[i * 2 + 1] = temdata[1];
        }

        if (outData == null || outData.Length <= 0)
        {
            Debug.Log("GetClipData intData is null");
            return null;
        }
        return outData;
    }

    void SaveAudioData()
    {
        File.WriteAllBytes(Application.dataPath + "/Resources/audioTest", GetClipData());
    }


    void DataTest()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        float[] samples = new float[audioSource.clip.samples * audioSource.clip.channels];
        audioSource.clip.GetData(samples, 0);

        for (int i = 0; i < samples.Length; ++i)
        {
            samples[i] = samples[i] * 0.5f;
        }

        audioSource.clip.SetData(samples, 0);
    }

}
