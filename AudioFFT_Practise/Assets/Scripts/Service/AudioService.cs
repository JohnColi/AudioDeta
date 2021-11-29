using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// 撥放音效用
/// </summary>
public class AudioService : MonoSingleton<AudioService>
{
    private AudioSource mBGMaudioSource;

    /// <summary> 第一的音效 </summary>
    private AudioSource mSFXaudioSource;
    /// <summary> UI音效 無法暫停 </summary>
    private AudioSource mUIaudioSource;

    /// <summary> 題目音效 </summary>
    private AudioSource mOptionSFXaudio;

    /// <summary> 音效complete </summary>
    Coroutine mSFXCoroutine;
    /// <summary> 選項的音效complete </summary>
    Coroutine mOptionCoroutine;
    /// <summary> BGM切換用 </summary>
    Coroutine mBgmFadeCoroutine;

    bool isPasue = false;

    #region 淡入淡出
    float mTimer;
    float mFadeTimes;
    bool isFadeIn;
    bool isFadeOut;
    float targetVolume;
    #endregion

    private void Awake()
    {
        mBGMaudioSource = this.gameObject.AddComponent<AudioSource>();
        mSFXaudioSource = this.gameObject.AddComponent<AudioSource>();
        mOptionSFXaudio = this.gameObject.AddComponent<AudioSource>();
        mUIaudioSource = this.gameObject.AddComponent<AudioSource>();
    }

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

    #region fade music
    /// <summary>
    /// 淡入音樂
    /// </summary>
    /// <param name="destVolume"></param>
    public void FadeInMusic(float destVolume = 1.0f)
    {
        StartCoroutine(FadeMusic(destVolume));
    }

    /// <summary>
    /// 淡出音樂
    /// </summary>
    /// <param name="destVolume"></param>
    public void FadeOutMusic(float destVolume = 0.3f)
    {
        StartCoroutine(FadeMusic(destVolume));
    }

    /// <summary>
    /// 等0.1秒
    /// </summary>
    private WaitForSeconds m_WaitForPointOneSecond = new WaitForSeconds(0.1f);
    /// <summary>
    /// 是否正在fading 音樂
    /// </summary>
    private bool m_IsFadingMusic = false;
    /// <summary>
    /// Fading 音樂complete
    /// </summary>
    public System.Action OnFadeMusicCompleteEvent;

    private IEnumerator FadeMusic(float destVolume)
    {
        Debug.Log("fadeMusic, destVolume : " + destVolume + " , isFadingMusic : " + m_IsFadingMusic + " , currVolume : " + mBGMaudioSource.volume);
        while (m_IsFadingMusic)
        {
            yield return m_WaitForPointOneSecond;
        }

        m_IsFadingMusic = true;

        if (mBGMaudioSource.volume > destVolume)
        {
            while (mBGMaudioSource.volume > destVolume)
            {
                mBGMaudioSource.volume -= 0.1f;
                yield return m_WaitForPointOneSecond;
            }
        }
        else if (mBGMaudioSource.volume < destVolume)
        {
            while (mBGMaudioSource.volume < destVolume)
            {
                mBGMaudioSource.volume += 0.1f;
                yield return m_WaitForPointOneSecond;
            }
        }

        m_IsFadingMusic = false;
        if (OnFadeMusicCompleteEvent != null)
            OnFadeMusicCompleteEvent();

    }

    #endregion fade music

    #region 音效
    public void PlaySound(AudioClip clip, Action action = null)
    {
        float clipLength = 1f;
        if (mSFXaudioSource != null)
        {
            if (mSFXaudioSource.isPlaying)
                mSFXaudioSource.Stop();

            mSFXaudioSource.clip = clip;
            mSFXaudioSource.Play();
            clipLength = clip.length;
        }
        else
        {
            Debug.LogError("PlaySound, clip is null");
        }

        if (mSFXCoroutine != null) StopCoroutine(mSFXCoroutine);
        mSFXCoroutine = StartCoroutine(SoundOnComplete(mSFXaudioSource, clipLength, action));
    }

    public void StopSound()
    {
        if (mSFXaudioSource.isPlaying)
            mSFXaudioSource.Stop();

        if (mSFXCoroutine != null)
            StopCoroutine(mSFXCoroutine);
    }

    public void SetSoundVolume(float value)
    {
        mSFXaudioSource.volume = value;
    }
    #endregion

    #region UI的音效
    /// <summary>
    /// UI音效，沒有call back，無法暫停
    /// </summary>
    public void PlayUiSound(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogError("PlayNewSound, clip == null");
            return;
        }

        if (mUIaudioSource != null)
        {
            if (mUIaudioSource.isPlaying)
                mUIaudioSource.Stop();
        }

        mUIaudioSource.clip = clip;
        mUIaudioSource.Play();

        //if (mSFXCoroutine2 != null) StopCoroutine(mSFXCoroutine2);
        //mSFXCoroutine2 = StartCoroutine(SoundOnComplete(mSFXaudioSource2, 0f, action));
    }

    public void SetSound2Volume(float value)
    {
        mUIaudioSource.volume = value;
    }

    #endregion

    #region 音效題目
    public void PlayAnsAudio(AudioClip clip, Action action)
    {
        float clipLength = 1f;
        if (clip != null)
        {
            mOptionSFXaudio.clip = clip;
            mOptionSFXaudio.Play();
            clipLength = clip.length;
            BGMFadeOut();
        }
        else
        {
            Debug.LogError("PlayAnsAudio, clip is null");
        }

        if (mOptionCoroutine != null) StopCoroutine(mOptionCoroutine);
        mOptionCoroutine = StartCoroutine(AudioOnComplete(mOptionSFXaudio, clipLength, action));
    }
    #endregion

    //
    IEnumerator AudioOnComplete(AudioSource audioSouce, float times, Action action)
    {
        BGMFadeIn();

        while (audioSouce.isPlaying || isPasue)
        {
            yield return null;
        }

        action?.Invoke();
        audioSouce.clip = null;
        yield break;
    }

    //
    IEnumerator SoundOnComplete(AudioSource audioSouce, float times, Action action)
    {
        if (action == null)
            yield break;

        while (audioSouce.isPlaying || isPasue)
        {
            //Debug.LogFormat("isPlaying: {0} , isPasue: {1}", audioSouce.isPlaying, isPasue);
            yield return null;
        }
        //Debug.LogFormat("音效播完 {0} , isPasue : {1}", audioSouce.isPlaying, isPasue);

        action?.Invoke();
        audioSouce.clip = null;
        yield break;
    }

    public void PasueAllSoundAudio()
    {
        //Debug.Log(string.Format("<color=#00ffff> --- Pasue All Sound Audio --- </color>"));
        isPasue = true;
        mOptionSFXaudio.Pause();
        mSFXaudioSource.Pause();
        //mSFXaudioSource2.Pause();
    }

    public void ResumeAllSoundAudio()
    {
        //Debug.Log(string.Format("<color=#00ffff> --- Resume All Sound Audio --- </color>"));
        isPasue = false;
        if (mOptionSFXaudio.clip != null)
            mOptionSFXaudio.Play();
        if (mSFXaudioSource.clip != null)
            mSFXaudioSource.Play();
        //if (mSFXaudioSource2.clip != null)
        //    mSFXaudioSource2.Play();
    }

    public void StopAnsAudio()
    {
        isPasue = false;
        mOptionSFXaudio.Stop();
        mOptionSFXaudio.clip = null;
        BGMFadeIn();

        if (mOptionCoroutine != null)
            StopCoroutine(mOptionCoroutine);
    }

    private void ClearAudio()
    {
        if (mSFXCoroutine != null)
            StopCoroutine(mSFXCoroutine);
        if (mOptionCoroutine != null)
            StopCoroutine(mOptionCoroutine);
        if (mBgmFadeCoroutine != null)
            StopCoroutine(mBgmFadeCoroutine);

        mBGMaudioSource.clip = null;
        mSFXaudioSource.clip = null;
        mUIaudioSource.clip = null;
        mOptionSFXaudio.clip = null;
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