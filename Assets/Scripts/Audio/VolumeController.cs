using System.Net.Mime;
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
    [SerializeField] public GameObject buttonMuteImage;
    [SerializeField] public Sprite audioPlay;
    [SerializeField] public Sprite audioMute;
    [SerializeField] private static bool isMute = false;

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
        imageButtonMuteController();
    }

    public void VolumeSFX()
    {
        masterMixer.SetFloat("SFX", sfxSlider.value);
        sfxValueVolume = sfxSlider.value;
        imageButtonMuteController();
    }

    public void MuteGame()
    {
        AudioList.instance.PlayButtonClick();
        if(isMute)
        {
            sfxValueVolume = 0;
            bgmValueVolume = 0;
            isMute = false;
        }
        else if (!isMute)
        {
            sfxValueVolume = -80;
            bgmValueVolume = -80;
            isMute = true;
        }
        bgmSlider.value = bgmValueVolume;
        sfxSlider.value = sfxValueVolume;
        VolumeBGM();
        VolumeSFX();
        imageButtonMuteController();
    }

    public void imageButtonMuteController()
    {
			// Se tiver sprite
			if (audioMute && audioPlay) {
				if ((sfxValueVolume == -80) && (bgmValueVolume == -80)) buttonMuteImage.GetComponent<Image>().sprite = audioMute;
				if ((sfxValueVolume > -80) || (bgmValueVolume > -80)) buttonMuteImage.GetComponent<Image>().sprite = audioPlay;
			}
    }
}