using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UniRx;

/// <summary>
/// 錄音相關
/// </summary>
public class SaveAudioClip
{
    const int HEADER_SIZE = 44;
    const int RECORD_TIME = 10;

#if UNITY_EDITOR
    static public string audioPath = Application.persistentDataPath + "/Audios/";
#elif UNITY_ANDROID
    static public string audioPath = Application.persistentDataPath + "/Audios/";
#endif

    //保存wav 模式
    public static void Save(string filename, AudioClip clip)
    {
        string path = audioPath;
        Save(filename, audioPath, clip);
    }

    public static bool Save(string filename, string path, AudioClip clip)
    {
        if (!Directory.Exists(path))
        {
            Debug.Log("創建資料夾 : " + path);
            Directory.CreateDirectory(path);
        }

        if (!filename.ToLower().EndsWith(".wav"))
            filename += ".wav";

        path = Path.Combine(path, filename);

        // Make sure directory exists if user is saving to sub dir.
        Directory.CreateDirectory(Path.GetDirectoryName(path));

        using (FileStream fileStream = CreateEmpty(path))
        {
            ConvertAndWrite(fileStream, clip);

            WriteHeader(fileStream, clip);
        }
        Debug.Log("儲存錄音:" + filename);

        // TODO: return false if there's a failure saving the file
        return true; 
    }

    public static bool Delete(string filename)
    {
        return Delete(filename, audioPath);
    }
    
    public static bool Delete(string filename, string path)
    {
        if (!filename.ToLower().EndsWith(".wav"))
            filename += ".wav";

        path = path + filename;

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("檔案刪除成功 : " + path);
            return true;
        }
        else
        {
            Debug.LogError(string.Format("沒有此檔案 {0} ", path));
            return false;
        }
    }

    public static void LoadAudio(string fileName, Action<AudioClip> callback)
    {
        string path = audioPath;
        path = Path.Combine(path, fileName);

        Observable.FromCoroutine(_ => LoadAudioClip(path, callback)).Subscribe();
    }

    static private IEnumerator LoadAudioClip(string path, Action<AudioClip> callback)
    {
        Debug.Log("Load Path : " + path);
        //Android need add file://
        WWW www = new WWW("file://" + path);
        while (!www.isDone)
        {
            yield return false;
        }
        AudioClip clip = www.GetAudioClip();

        if (clip != null)
            Debug.Log("clip name : " + clip.name + " , length: " + clip.length);

        if (callback != null)
            callback(clip);
    }

    public static void LoadAudio(string fileName, Action<AudioClip,int> callback, int index)
    {
        string path = audioPath;
        path = Path.Combine(path, fileName);

        Observable.FromCoroutine(_ => LoadAudioClip(path, callback , index)).Subscribe();
    }

    static private IEnumerator LoadAudioClip(string path, Action<AudioClip,int> callback , int index)
    {
        Debug.Log("Load Path : " + path);
        //Android need add file://
        WWW www = new WWW("file://" + path);
        while (!www.isDone)
        {
            yield return false;
        }
        AudioClip clip = www.GetAudioClip();

        if (clip != null)
            Debug.Log("clip name : " + clip.name + " , length: " + clip.length);

        if (callback != null)
            callback(clip, index);
    }

    static FileStream CreateEmpty(string filepath)
    {
        FileStream fileStream = new FileStream(filepath, FileMode.Create);
        byte emptyByte = new byte();

        for (int i = 0; i < HEADER_SIZE; i++) //preparing the header
        {
            fileStream.WriteByte(emptyByte);
        }

        return fileStream;
    }
    static void ConvertAndWrite(FileStream fileStream, AudioClip clip)
    {
        float[] samples = new float[clip.samples];

        clip.GetData(samples, 0);

        Int16[] intData = new Int16[samples.Length];
        //converting in 2 float[] steps to Int16[], //then Int16[] to Byte[]

        Byte[] bytesData = new Byte[samples.Length * 2];
        //bytesData array is twice the size of
        //dataSource array because a float converted in Int16 is 2 bytes.

        int rescaleFactor = 32767; //to convert float to Int16

        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(samples[i] * rescaleFactor);
            Byte[] byteArr = new Byte[2];
            byteArr = BitConverter.GetBytes(intData[i]);
            byteArr.CopyTo(bytesData, i * 2);
        }

        fileStream.Write(bytesData, 0, bytesData.Length);
    }

    static void WriteHeader(FileStream fileStream, AudioClip clip)
    {
        int hz = clip.frequency;
        int channels = clip.channels;
        int samples = clip.samples;

        fileStream.Seek(0, SeekOrigin.Begin);

        Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
        fileStream.Write(riff, 0, 4);

        Byte[] chunkSize = BitConverter.GetBytes(fileStream.Length - 8);
        fileStream.Write(chunkSize, 0, 4);

        Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
        fileStream.Write(wave, 0, 4);

        Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
        fileStream.Write(fmt, 0, 4);

        Byte[] subChunk1 = BitConverter.GetBytes(16);
        fileStream.Write(subChunk1, 0, 4);

        UInt16 two = 2;
        UInt16 one = 1;

        Byte[] audioFormat = BitConverter.GetBytes(one);
        fileStream.Write(audioFormat, 0, 2);

        Byte[] numChannels = BitConverter.GetBytes(channels);
        fileStream.Write(numChannels, 0, 2);

        Byte[] sampleRate = BitConverter.GetBytes(hz);
        fileStream.Write(sampleRate, 0, 4);

        Byte[] byteRate = BitConverter.GetBytes(hz * channels * 2); // sampleRate * bytesPerSample*number of channels, here 44100*2*2
        fileStream.Write(byteRate, 0, 4);

        UInt16 blockAlign = (ushort)(channels * 2);
        fileStream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

        UInt16 bps = 16;
        Byte[] bitsPerSample = BitConverter.GetBytes(bps);
        fileStream.Write(bitsPerSample, 0, 2);

        Byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
        fileStream.Write(datastring, 0, 4);

        Byte[] subChunk2 = BitConverter.GetBytes(samples * channels * 2);
        fileStream.Write(subChunk2, 0, 4);
    }
}
