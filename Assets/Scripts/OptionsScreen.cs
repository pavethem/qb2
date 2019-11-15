using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsScreen : MonoBehaviour
{
    private bool isLoading;
    private int hard;
    private int freerotation;
    
    //start position when moving in
    private Vector2 startPosition;
    private RectTransform buttonsTransform;
    private bool isMoving;

    public AudioMixer mixer;
    public Slider backgroundVolumeSlider;
    public Slider effectsVolumeSlider;

    private void Start() {
        
        buttonsTransform = GameObject.Find("OptionsButtons").GetComponent<RectTransform>();
        startPosition = new Vector2( 0 - (Screen.width / 2f + buttonsTransform.rect.width),0);
        buttonsTransform.anchoredPosition = startPosition;
        
        backgroundVolumeSlider.value = PlayerPrefs.GetFloat("BackgroundVolumeSlider", 1f);
        effectsVolumeSlider.value = PlayerPrefs.GetFloat("EffectsVolumeSlider", 1f);
        hard = GameController.hardmode;
        freerotation = GameController.freeRotation;
        SetHardModeText();
        SetFreeRotationText();
        StartCoroutine(MoveIn());
    }
    
    private IEnumerator MoveIn() {
        isMoving = true;
        float step = 0;
        float x = startPosition.x;
        while (buttonsTransform.anchoredPosition.x < 0) {
            buttonsTransform.anchoredPosition = new Vector2(Mathf.Lerp(x, 0, step), 0);
            step += Time.deltaTime;
            yield return null;
        }

        buttonsTransform.anchoredPosition = new Vector2(0,0);
        isMoving = false;
    }
    
    private IEnumerator MoveOut() {
        isMoving = true;
        float step = 0;
        float x = startPosition.x;
        while (buttonsTransform.anchoredPosition.x > x) {
            buttonsTransform.anchoredPosition = new Vector2(Mathf.Lerp(0, x, step), 0);
            step += Time.deltaTime;
            yield return null;
        }

        buttonsTransform.anchoredPosition = startPosition;
        isMoving = false;
    }

    public void BackButton() {
        if(isMoving) return;
        StartCoroutine(MoveOut());
        isLoading = true;
    }
    
    public void HardModeButton() {
        hard *= -1;
        GameController.hardmode = hard;
        PlayerPrefs.SetInt("HardMode",hard);
        PlayerPrefs.Save();
        SetHardModeText();
    }
    
    public void FreeRotationButton() {
        freerotation *= -1;
        GameController.freeRotation = freerotation;
        PlayerPrefs.SetInt("FreeRotation",freerotation);
        PlayerPrefs.Save();
        SetFreeRotationText();
    }
    
    private void SetFreeRotationText() {
        string text = GameObject.Find("MainCanvas").transform.Find("OptionsButtons").Find("RotationButton").Find("Description")
            .GetComponent<TextMeshProUGUI>().text;

        text = text.Substring(0, text.IndexOf("=")+1);
        
        if (freerotation == -1)
            text += " Off";
        else
            text += " On";

        GameObject.Find("MainCanvas").transform.Find("OptionsButtons").Find("RotationButton").Find("Description")
            .GetComponent<TextMeshProUGUI>().text = text;
    }

    private void SetHardModeText() {
        string text = GameObject.Find("MainCanvas").transform.Find("OptionsButtons").Find("HardModeButton").Find("Description")
            .GetComponent<TextMeshProUGUI>().text;

        text = text.Substring(0, text.IndexOf("=")+1);
        
        if (hard == -1)
            text += " Off";
        else
            text += " On";

        GameObject.Find("MainCanvas").transform.Find("OptionsButtons").Find("HardModeButton").Find("Description")
            .GetComponent<TextMeshProUGUI>().text = text;
    }

    public void SetBackgroundVolume(float sliderValue) {
        float volume = Mathf.Log10(sliderValue) * 20;
        mixer.SetFloat("BackgroundVolume", volume);
        if (sliderValue == 0) {
            mixer.SetFloat("BackgroundVolume", -80);
            volume = -80;
        }
        PlayerPrefs.SetFloat("BackgroundVolumeSlider", sliderValue);
        PlayerPrefs.SetFloat("BackgroundVolume", volume);
        PlayerPrefs.Save();
    }
    
    public void SetEffectsVolume(float sliderValue) {
        if(!GetComponent<AudioSource>().isPlaying)
            GetComponent<AudioSource>().Play();
        
        float volume = Mathf.Log10(sliderValue) * 20;
        mixer.SetFloat("EffectsVolume", volume);
        if (sliderValue == 0) {
            mixer.SetFloat("EffectsVolume", -80);
            volume = -80;
        }
        PlayerPrefs.SetFloat("EffectsVolumeSlider", sliderValue);
        PlayerPrefs.SetFloat("EffectsVolume", volume);
        PlayerPrefs.Save();
    }
    
    private void Update() {
        if (isLoading) {
            if (!isMoving) {
                SceneManager.LoadSceneAsync("mainmenu");
            }
        }
    }
}
