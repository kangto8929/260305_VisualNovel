using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioSource BgmSource;
    public AudioSource SfxSource;

    public List<SoundData> BgmList;
    public List<SoundData> SfxList;

    private Dictionary<string, AudioClip> _bgmDict;
    private Dictionary<string, AudioClip> _sfxDict;

    public bool BgmEnabled = true;
    public bool SfxEnabled = true;

    void Awake()
    {
        Instance = this;

        _bgmDict = new Dictionary<string, AudioClip>();
        _sfxDict = new Dictionary<string, AudioClip>();

        foreach (var bgm in BgmList)
        {
            _bgmDict[bgm.Name] = bgm.Clip;
        }

        foreach (var sfx in SfxList)
        {
            _sfxDict[sfx.Name] = sfx.Clip;
        }
    }

    public void PlayBGM(string name)
    {
        if (!BgmEnabled) return;

        if (_bgmDict.TryGetValue(name, out var clip))
        {
            BgmSource.clip = clip;
            BgmSource.loop = true;
            BgmSource.Play();
        }
    }

    public void StopBGM()
    {
        BgmSource.Stop();
    }

    public void PlaySFX(string name)
    {
        if (!SfxEnabled) return;

        if (_sfxDict.TryGetValue(name, out var clip))
        {
            SfxSource.PlayOneShot(clip);
        }
    }

    public void SetBgmEnabled(bool enabled)
    {
        BgmEnabled = enabled;

        if (!enabled)
            BgmSource.Stop();
    }

    public void SetSfxEnabled(bool enabled)
    {
        SfxEnabled = enabled;
    }
}