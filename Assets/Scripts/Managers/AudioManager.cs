using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class AudioManager : MonoBehaviour
{
    [Header("Setting:")]
    [SerializeField] private AudioMixer audioMixer;
    [Header("Sources:")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource vfxSource;
    [Header("Sounds:")]
    [SerializeField] private AudioClip backgroundAudio;
    [SerializeField] private AudioClip buttonAudio;
    [SerializeField] private AudioClip wrongMoveAudio;


    private const string musicVolumeParameter = "MusicVolume";
    private const string vfsVolumeParameter = "VFSVolume";

    public static AudioMixer Mixer { get; private set; }
    public static AudioSource MusicSource { get; private set; }
    public static AudioSource VFXSource { get; private set; }
    public static AudioClip BackgroundAudio { get; private set; }
    public static AudioClip ButtonAudio { get; private set; }
    public static AudioClip WrongMoveAudio { get; private set; }
    public void Initilize()
    {
        Mixer = audioMixer;
        MusicSource = musicSource;
        VFXSource = vfxSource;
        BackgroundAudio = backgroundAudio;
        ButtonAudio = buttonAudio;
        WrongMoveAudio = wrongMoveAudio;

        //Init Audio Value
        Mixer.SetFloat(musicVolumeParameter, PlayerPrefs.GetFloat("Music", 0));
        Mixer.SetFloat(vfsVolumeParameter, PlayerPrefs.GetFloat("VFS", 0));
    }

    public static void ChangeMusicAudioSetting(bool muteMusic)
    {
        float volume = muteMusic ? -80f : 0f;
        Mixer.SetFloat(musicVolumeParameter, volume);
        PlayerPrefs.SetFloat("Music", volume);
    }

    public static bool IsMuteMusic()
    {
        if(Mixer.GetFloat(musicVolumeParameter, out float volume))
        {
            return volume <= -80f;
        }
        return false;
    }

    public static void ChangeVfsAudioSetting(bool muteVfs)
    {
        float volume = muteVfs ? -80f : 0f;
        Mixer.SetFloat(vfsVolumeParameter, volume);
        PlayerPrefs.SetFloat("VFS", volume);
    }

    public static bool IsMuteVFS()
    {
        if (Mixer.GetFloat(vfsVolumeParameter, out float volume))
        {
            return volume <= -80f;
        }
        return false;
    }

    public static void PlayAudio(AudioSource source, AudioClip clip)
    {
        source.clip = clip;
        source.Play();
    }

    public static void PlayerOneShotAudio(AudioSource source, AudioClip clip)
    {
        source.PlayOneShot(clip);
    }

    public static void StopPlay(AudioSource source)
    {
        source.Stop();
    }

    public void PlayButtonSound()
    {
        PlayerOneShotAudio(VFXSource, ButtonAudio);
    }
}
