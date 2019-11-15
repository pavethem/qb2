using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectScreen : MonoBehaviour
{
    //button to load another screen has been clicked
    private bool isLoading;
    private string levelName;
    //start position when moving in
    private Vector2 startPosition;
    //the active transform (mobile or desktop)
    private RectTransform buttonsTransform;
    private bool isMoving;

    public Texture replacementTexture;
    
    // Start is called before the first frame update
    void Start() {
        buttonsTransform = GameObject.Find("Scroll View").GetComponent<RectTransform>();
        startPosition = new Vector2(0,0 - (buttonsTransform.rect.height + Screen.height / 2f));
        buttonsTransform.anchoredPosition = startPosition;
        levelName = "1";
        StartCoroutine(MoveIn());

        int levelCount = GameObject.Find("Scroll View").transform.Find("Viewport").GetChild(0).childCount - 1;

        //replace textures with locks, when levels are not yet unlocked
        for (int i = GameController.maxScene + 1; i <= levelCount; i++) {
            GameObject.Find("Scroll View").transform.Find("Viewport").GetChild(0).GetChild(i).Find("LevelImage").GetComponent<RawImage>().texture =
                replacementTexture;
        }

    }
    
    private IEnumerator MoveIn() {
        isMoving = true;
        float step = 0;
        float y = startPosition.y;
        while (buttonsTransform.anchoredPosition.y < 0) {
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
        while (buttonsTransform.anchoredPosition.y > y) {
            buttonsTransform.anchoredPosition = new Vector2(0, Mathf.Lerp(0, y, step));
            step += Time.deltaTime;
            yield return null;
        }

        buttonsTransform.anchoredPosition = startPosition;
        isMoving = false;
    }

    public void LevelSelectButton(string buttonName) {
        if(isMoving || int.Parse(buttonName) > GameController.maxScene) return;
        StartCoroutine(MoveOut());
        isLoading = true;
        levelName = buttonName;
    }
    
    public void BackButton() {
        if(isMoving) return;
        StartCoroutine(MoveOut());
        isLoading = true;
        levelName = "back";
    }
    
    private void Update() {
        //load new level after menu has moved out of screen
        if (isLoading) {
            if (!isMoving) {
                if(levelName == "back")
                    SceneManager.LoadSceneAsync("mainmenu");
                else {
                    GameController.currentScene = int.Parse(levelName);
                    PlayerPrefs.SetInt("currentScene", GameController.currentScene);
                    PlayerPrefs.Save();
                    GameController.LoadCurrentScene();
                }
            }
        }
    }
    
}
