using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// 撥放音效用
/// </summary>
public class AudioService : MonoSingleton<AudioService>
{
    private AudioSource mBGMaudioSource;
    /// <summary> 第二個音效 </summary>
    private AudioSource mSFXaudioSource;
    /// <summary> 第三個音效 </summary>
    private AudioSource mSFXaudioSource2;
    /// <summary> 題目音效，完成可+回傳值 </summary>
    private AudioSource mOptionAudioSource;


    /// <summary> 音效complete </summary>
    Coroutine mSFXCoroutine;
    /// <summary> 選項的音效complete </summary>
    Coroutine mOptionCoroutine;
    /// <summary> BGM切換用 </summary>
    Coroutine mBgmFadeCoroutine;
    /// <summary> 錄音用 </summary>
    Coroutine mRecordCoroutine;

    /// <summary> 第二的音效 </summary>
    private float mSFXplaytimes;

    /// <summary> 第二的音效 </summary>
    private Action mSFXevent;

    //淡入淡出
    float mTimer;
    float mFadeTimes;
    bool isFadeIn;
    bool isFadeOut;
    float targetVolume;

    protected override void Awake()
    {
        base.Awake();
        mBGMaudioSource = this.gameObject.AddComponent<AudioSource>();
        mSFXaudioSource = this.gameObject.AddComponent<AudioSource>();
        mOptionAudioSource = this.gameObject.AddComponent<AudioSource>();
        mSFXaudioSource2 = this.gameObject.AddComponent<AudioSource>();
    }

    #region 背景音樂
    /// <summary>
    /// 塞AudioClip進去撥放音樂檔,可以設定是否循環撥放
    /// </summary>
    /// <param name="music"></param>
    /// <param name="isloop"></param>
    public void PlayMusic(AudioClip music, bool isLoop = false)
    {
        if (mBGMaudioSource != null && music != null)
        {
            if (mBGMaudioSource.isPlaying)
                mBGMaudioSource.Stop();
            mBGMaudioSource.loop = isLoop;
            mBGMaudioSource.clip = music;
            mBGMaudioSource.Play();
        }
    }

    public void PasueMusic()
    {
        mBGMaudioSource.Pause();
    }

    public void ResumeMusic()
    {
        mBGMaudioSource.Play();
    }

    public void StopMusic()
    {
        if (mBGMaudioSource != null)
        {
            mBGMaudioSource.Stop();
        }
    }

    public void SetMusicVol(float value)
    {
        mBGMaudioSource.volume = value;
    }

    public void ChangeBGM(AudioClip music, float fadeInTimes)
    {
        //強制0.5秒後開始撥放
        if (mBgmFadeCoroutine != null)
            StopCoroutine(mBgmFadeCoroutine);
        mBgmFadeCoroutine = StartCoroutine(IeFadeInNewBGM(music, 0.5f, fadeInTimes));
    }
    IEnumerator IeFadeInNewBGM(AudioClip music, float fadeOutTimes, float fadeInTimes)
    {
        BGMFadeOut(fadeOutTimes, 0);
        yield return new WaitForSeconds(fadeOutTimes + 0.15f);

        mBGMaudioSource.clip = music;
        mBGMaudioSource.Play();
        BGMFadeIn(fadeInTimes);
    }
    #endregion

    #region 音效
    /// <summary>
    /// 
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="action"></param>
    /// <param name="times"> 開始撥放時間 </param>
    public void PlaySound(AudioClip clip, Action action = null, float times = 0)
    {
        if (mSFXaudioSource != null)
        {
            if (mSFXaudioSource.isPlaying)
                mSFXaudioSource.Stop();

            mSFXaudioSource.clip = clip;
            mSFXaudioSource.time = times;
            mSFXaudioSource.Play();

            if (action == null) return;
            mSFXevent = action;
            if (mSFXCoroutine != null) StopCoroutine(mSFXCoroutine);
            float f = clip.length - times;
            //Debug.Log(string.Format("clip length: {0}, play times: {1}, time left: {2}", clip.length, times, f));
            mSFXCoroutine = StartCoroutine(SoundOnComplete(f, action));
        }
    }

    public void PasueSound()
    {
        mSFXaudioSource.Pause();
        mSFXplaytimes = mSFXaudioSource.time;
        StopCoroutine(mSFXCoroutine);
    }

    public void ResumeSound()
    {
        mSFXaudioSource.Play();
        if (mSFXCoroutine != null) StopCoroutine(mSFXCoroutine);
        mSFXCoroutine = StartCoroutine(SoundOnComplete(mSFXplaytimes, mSFXevent));
    }

    public void StopSound()
    {
        if (mSFXaudioSource.isPlaying)
            mSFXaudioSource.Stop();

        if (mSFXCoroutine != null)
            StopCoroutine(mSFXCoroutine);
    }

    IEnumerator SoundOnComplete(float times, Action action)
    {
        yield return new WaitForSeconds(times);
        action();
    }

    public void SetSoundVolume(float value)
    {
        mSFXaudioSource.volume = value;
    }

    public void PlayNewSound(AudioClip clip)
    {
        if (mSFXaudioSource2 != null)
        {
            if (mSFXaudioSource2.isPlaying)
                mSFXaudioSource2.Stop();
        }

        mSFXaudioSource2.clip = clip;
        mSFXaudioSource2.Play();
        Debug.Log("play audio " + clip.name);
    }

    public float GetSoundTime()
    {
        return mSFXaudioSource.time;
    }
    #endregion

    #region 音效題目
    public void PlayAnsAudio(AudioClip clip, Action action)
    {
        mOptionAudioSource.clip = clip;
        mOptionAudioSource.Play();

        BGMFadeOut();

        if (mOptionCoroutine != null) StopCoroutine(mOptionCoroutine);
        mOptionCoroutine = StartCoroutine(AudioOnComplete(clip.length, action));
    }

    public void StopAnsAudio()
    {
        mOptionAudioSource.Stop();
        mOptionAudioSource.clip = null;
        BGMFadeIn();

        if (mOptionCoroutine != null)
            StopCoroutine(mOptionCoroutine);
    }

    IEnumerator AudioOnComplete(float times, Action action)
    {
        yield return new WaitForSeconds(times);
        BGMFadeIn();
        action?.Invoke();
        mOptionAudioSource.clip = null;
        yield break;
    }
    #endregion

    public void PasueAnsAudio()
    {
        mOptionAudioSource.Pause();
        mSFXaudioSource.Pause();
    }

    public void ResumeAnsAudio()
    {
        if (mOptionAudioSource.clip != null)
            mOptionAudioSource.Play();
        if (mSFXaudioSource.clip != null)
            mSFXaudioSource.Play();
    }

    /// <summary>
    /// 背影音樂淡出
    /// </summary>
    /// <param name="times"> 淡出時間 </param>
    /// <param name="targetVolume"> 目標音量 </param>
    public void BGMFadeOut(float times = 1, float targetVolume = 0.5f)
    {
        isFadeOut = true;
        isFadeIn = false;
        mFadeTimes = times;
        mTimer = 0;
        this.targetVolume = targetVolume;
    }

    /// <summary>
    /// 背影音樂淡入
    /// </summary>
    /// <param name="times"> 淡入時間 </param>
    /// <param name="targetVolume"> 目標音量 </param>
    public void BGMFadeIn(float times = 1, float targetVolume = 1)
    {
        isFadeIn = true;
        isFadeOut = false;
        mFadeTimes = times;
        mTimer = 0;
        this.targetVolume = targetVolume;
    }

    private void Update()
    {
        //BGM 的 FadeOut FadeIn
        if (isFadeOut)
        {
            if (mTimer >= mFadeTimes)
            {
                isFadeOut = false;
                mBGMaudioSource.volume = targetVolume;
            }
            else
            {
                mTimer += Time.deltaTime;
                float value = Time.deltaTime / mFadeTimes;
                mBGMaudioSource.volume = Mathf.Lerp(mBGMaudioSource.volume, targetVolume, value); ;
            }
        }
        else if (isFadeIn)
        {
            if (mTimer >= mFadeTimes)
            {
                isFadeIn = false;
                mBGMaudioSource.volume = targetVolume;
            }
            else
            {
                mTimer += Time.deltaTime;
                float volume = Mathf.Lerp(mBGMaudioSource.volume, targetVolume, Time.deltaTime / mFadeTimes);
                mBGMaudioSource.volume = volume;
            }
        }
    }
}