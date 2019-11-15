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
        startPosition = new Vector2(0,Screen.height / 2f + buttonsTransform.rect.height);
        buttonsTransform.anchoredPosition = startPosition;
        StartCoroutine(MoveIn());
    }

    private IEnumerator MoveIn() {
        isMoving = true;
        float step = 0;
        float y = startPosition.y;
        while (buttonsTransform.anchoredPosition.y > 0) {
            buttonsTransform.anchoredPosition = new Vector2(0, Mathf.Lerp(y, 0, step));
            step += Time.deltaTime;
            yield return null;
        }

        buttonsTransform.anchoredPosition = new Vector2(0,0);
        isMoving = false;
    }
    
    private IEnumerator MoveOut() {
        isMoving = true;
        float step = 0;
        float y = startPosition.y;
        while (buttonsTransform.anchoredPosition.y < y) {
            buttonsTransform.anchoredPosition = new Vector2(0, Mathf.Lerp(0, y, step));
            step += Time.deltaTime;
            yield return null;
        }

        buttonsTransform.anchoredPosition = startPosition;
        isMoving = false;
    }

    public void StartButton() {
        if(isMoving) return;
        StartCoroutine(MoveOut());
        isLoading = true;
        buttonName = "start";
    }    
    
    public void SelectButton() {
        if(isMoving) return;
        StartCoroutine(MoveOut());
        isLoading = true;
        buttonName = "select";
    }
    
    public void OptionsButton() {
        if(isMoving) return;
        StartCoroutine(MoveOut());
        isLoading = true;
        buttonName = "options";
    }
    
    public void QuitButton() {
        Application.Quit();
    }

    private void Update() {
        //load new level after menu has moved out of screen
        if (isLoading) {
            if (!isMoving) {
                if (buttonName == "start")
                    GameController.LoadMaxScene();
                else if (buttonName == "options")
                    SceneManager.LoadSceneAsync("optionsmenu");
                else if (buttonName == "select")
                    SceneManager.LoadSceneAsync("selectlevelmenu");
            }
        }
    }
}
