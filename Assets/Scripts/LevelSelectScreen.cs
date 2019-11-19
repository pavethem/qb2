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
    private RectTransform contentTransform;
    private bool isMoving;

    public Texture replacementTexture;
    
    // Start is called before the first frame update
    void Start() {
        buttonsTransform = GameObject.Find("Scroll View").GetComponent<RectTransform>();
        contentTransform = buttonsTransform.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
        startPosition = new Vector2(Screen.width / 2f + buttonsTransform.rect.width, 0);
        buttonsTransform.anchoredPosition = startPosition;
        levelName = "1";
        StartCoroutine(MoveIn());

        int levelCount = GameObject.Find("Scroll View").transform.Find("Viewport").GetChild(0).childCount - 1;
        
        //add checkmars if hard mode was completed
        for (int i = 1; i <= GameController.maxScene; i++) {
            if(PlayerPrefs.GetInt("level"+i,-1) == 1)
                GameObject.Find("Scroll View").transform.Find("Viewport").GetChild(0).GetChild(i).Find("Panel").gameObject.SetActive(true);
        }

        //replace textures with locks, when levels are not yet unlocked
        for (int i = GameController.maxScene + 1; i <= levelCount; i++) {
            GameObject.Find("Scroll View").transform.Find("Viewport").GetChild(0).GetChild(i).Find("LevelImage").GetComponent<RawImage>().texture =
                replacementTexture;
        }

    }
    
    private IEnumerator MoveIn() {
        isMoving = true;
        float step = 0;
        float x = startPosition.x;
        while (buttonsTransform.anchoredPosition.x > 0)
        {
            buttonsTransform.anchoredPosition = new Vector2(Mathf.Lerp(x, 0, step), 0);
            step += Time.deltaTime;
            yield return null;
        }

        buttonsTransform.anchoredPosition = new Vector2(0, 0);
        isMoving = false;
    }
    
    private IEnumerator MoveOut() {
        isMoving = true;
        float step = 0;
        float x = startPosition.x;
        while (buttonsTransform.anchoredPosition.x < x)
        {
            buttonsTransform.anchoredPosition = new Vector2(Mathf.Lerp(0, x, step), 0);
            step += Time.deltaTime;
            yield return null;
        }

        buttonsTransform.anchoredPosition = startPosition;
        isMoving = false;
    }

    //scroll content back to the left
    private IEnumerator ResetContent() {
        isMoving = true;
        float step = 0;
        float x = contentTransform.anchoredPosition.x;
        while (contentTransform.anchoredPosition.x < 0)
        {
            contentTransform.anchoredPosition = new Vector2(Mathf.Lerp(x, 0, step), 0);
            step += Time.deltaTime * 2;
            yield return null;
        }

        contentTransform.anchoredPosition = new Vector2(0,0);
        StartCoroutine(nameof(MoveOut));
    }

    public void LevelSelectButton(string buttonName) {
        if(isMoving || int.Parse(buttonName) > GameController.maxScene) return;
        StartCoroutine(ResetContent());
        isLoading = true;
        levelName = buttonName;
    }
    
    public void BackButton() {
        if(isMoving) return;
        StartCoroutine(ResetContent());
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
