using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScreen : MonoBehaviour {
    //button to load another screen has been clicked
    private bool isLoading;
    //name of the button clicked
    private string buttonName;
    //start position when moving in
    private Vector2 startPosition;
    //the active transform (mobile or desktop)
    private RectTransform buttonsTransform;
    private bool isMoving;
    
    private void Start() {
        buttonsTransform = Application.isMobilePlatform
            ? GameObject.Find("MobileButtons").GetComponent<RectTransform>()
            : GameObject.Find("DesktopButtons").GetComponent<RectTransform>();
        buttonsTransform.GetComponent<Animation>().Play();
    }

    public void StartButton() {
        if(isMoving) return;
        buttonsTransform.GetComponent<Animation>().Play("MainMenuOut");
        isLoading = true;
        buttonName = "start";
    }    
    
    public void SelectButton() {
        if(isMoving) return;
        buttonsTransform.GetComponent<Animation>().Play("MainMenuOut");
        isLoading = true;
        buttonName = "select";
    }
    
    public void OptionsButton() {
        if(isMoving) return;
        buttonsTransform.GetComponent<Animation>().Play("MainMenuOut");
        isLoading = true;
        buttonName = "options";
    }
    
    public void QuitButton() {
        Application.Quit();
    }

    private void Update() {
        //load new level after menu has moved out of screen
        if (isLoading) {
            if (!buttonsTransform.GetComponent<Animation>().isPlaying) {
                if (buttonName == "start")
                    GameController.LoadMaxScene();
                else if (buttonName == "options")
                    SceneManager.LoadScene("optionsmenu");
                else if (buttonName == "select")
                    SceneManager.LoadScene("selectlevelmenu");
            }
        }
    }
}
