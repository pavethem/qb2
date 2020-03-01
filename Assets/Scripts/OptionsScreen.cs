using TMPro;
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

    public AudioMixer mixer;
    public Slider backgroundVolumeSlider;
    public Slider effectsVolumeSlider;

    private void Start() {
        
        buttonsTransform = GameObject.Find("OptionsButtons").GetComponent<RectTransform>();
//        startPosition = new Vector2( 0 - (Screen.width / 2f + buttonsTransform.rect.width),0);
//        buttonsTransform.anchoredPosition = startPosition;
        
        backgroundVolumeSlider.value = PlayerPrefs.GetFloat("BackgroundVolumeSlider", 1f);
        effectsVolumeSlider.value = PlayerPrefs.GetFloat("EffectsVolumeSlider", 1f);
        backgroundVolumeSlider.GetComponent<Slider>().enabled = true;
        effectsVolumeSlider.GetComponent<Slider>().enabled = true;
        hard = GameController.hardmode;
        freerotation = GameController.freeRotation;
        GameObject.Find("HardModeButton").transform.Find("Panel").gameObject.SetActive(hard == 1);
        GameObject.Find("RotationButton").transform.Find("Panel").gameObject.SetActive(freerotation == 1);
        GameObject.Find("BackgroundButton").GetComponent<Button>().interactable = GameController.gameCompleted;
        // SetHardModeText();
        // SetFreeRotationText();
//        StartCoroutine(MoveIn());
        buttonsTransform.GetComponent<Animation>().Play();
    }

    public void BackButton() {
        if(buttonsTransform.GetComponent<Animation>().isPlaying) return;
        buttonsTransform.GetComponent<Animation>().Play("OptionScreenOut");
        isLoading = true;
    }
    
    public void HardModeButton() {
        if(buttonsTransform.GetComponent<Animation>().isPlaying) return;
        hard *= -1;
        GameController.hardmode = hard;
        PlayerPrefs.SetInt("HardMode",hard);
        PlayerPrefs.Save();
        GameObject.Find("HardModeButton").transform.Find("Panel").gameObject.SetActive(hard == 1);
        // SetHardModeText();
    }
    
    public void FreeRotationButton() {
        if(buttonsTransform.GetComponent<Animation>().isPlaying) return;
        freerotation *= -1;
        GameController.freeRotation = freerotation;
        PlayerPrefs.SetInt("FreeRotation",freerotation);
        PlayerPrefs.Save();
        GameObject.Find("RotationButton").transform.Find("Panel").gameObject.SetActive(freerotation == 1);
        // SetFreeRotationText();
    }

    public void BackgroundButton() {
        if(buttonsTransform.GetComponent<Animation>().isPlaying) return;
        if (GameController.gameCompleted) {
            GameController.ChangeBackgroundOption();
        }
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
        if(!GetComponent<AudioSource>().isPlaying && effectsVolumeSlider.GetComponent<Slider>().enabled) 
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
            if (!buttonsTransform.GetComponent<Animation>().isPlaying) {
                SceneManager.LoadSceneAsync("mainmenu");
            }
        }
    }
}
