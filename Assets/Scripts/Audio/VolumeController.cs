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

    private void Start()
    {
        VolumeBGM();
        VolumeSFX();
    }

    public void VolumeBGM()
    {
        masterMixer.SetFloat("BGM", bgmSlider.value);
    }

    public void VolumeSFX()
    {
        masterMixer.SetFloat("SFX", sfxSlider.value);
    }
}
