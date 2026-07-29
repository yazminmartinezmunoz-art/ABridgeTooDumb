using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class ConfigurarAudio : MonoBehaviour
{
    //private bool pause = false;
    public GameObject pauseMenuUI;
    public AudioMixer masterMixer;
    public Slider sliderMaster;
    public Slider sliderSFX;
    public Slider sliderMusica;

    private void Start()
    {
        pauseMenuUI.SetActive(false);

        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        SetMasterVolume(masterVolume);
        sliderMaster.value = masterVolume;

        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        SetSFXVolume(sfxVolume);
        sliderSFX.value = sfxVolume;

        float musicaVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        SetMusicVolume(musicaVolume);
        sliderMusica.value = musicaVolume;
        
    }
    void Update()
    {

    }
    public void SetMasterVolume(float volume)
    {
        masterMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20f);
        
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        masterMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20f);

        PlayerPrefs.SetFloat("SFXVolume", volume);
    }
    public void SetMusicVolume(float volume)
    {
        masterMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20f);

        PlayerPrefs.SetFloat("MusicVolume", volume);
    }
}
