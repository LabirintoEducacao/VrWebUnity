using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    [SerializeField] public AudioMixer masterMixer;
    [SerializeField] public Slider bgmSlider;
    [SerializeField] public Slider sfxSlider;
    [SerializeField] private static float sfxValueVolume;
    [SerializeField] private static float bgmValueVolume = -30;

    public static VolumeController instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        bgmSlider.value = bgmValueVolume;
        sfxSlider.value = sfxValueVolume;
        VolumeBGM();
        VolumeSFX();
    }

    public void VolumeBGM()
    {
        masterMixer.SetFloat("BGM", bgmSlider.value);
        bgmValueVolume = bgmSlider.value;
    }

    public void VolumeSFX()
    {
        masterMixer.SetFloat("SFX", sfxSlider.value);
        sfxValueVolume = sfxSlider.value;
    }
}