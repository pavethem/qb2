using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialScreen : MonoBehaviour {
    public GameObject description;
    private bool destroy;
    
    void Start() {
        description.GetComponent<TextMeshProUGUI>().text = "Yo Dawg";
    }
    
    public void NextButton() {
        PlayerPrefs.SetInt("seenTutorial" + GameController.currentScene,1);
        transform.GetChild(0).GetComponent<Animation>().Play("MainMenuOut");
        GameController.tutorialLock = false;
        GameController.instance.StartCoroutine("MoveInButtons");
        if (Application.isMobilePlatform && GameController.freeRotation == 1)
            GameController.instance.StartCoroutine("MoveInButtons");
        destroy = true;
    }

    public void SkipButton() {
        PlayerPrefs.SetInt("skipTutorials",1);
        GameController.skipTutorials = true;
        transform.GetChild(0).GetComponent<Animation>().Play("MainMenuOut");
        GameController.tutorialLock = false;
        GameController.instance.StartCoroutine("MoveInButtons");
        if (Application.isMobilePlatform && GameController.freeRotation == 1)
            GameController.instance.StartCoroutine("MoveInButtons");
        destroy = true;

    }

    private void LateUpdate() {
        //wait until menu is moved out and unload scene
        if (destroy && !transform.GetChild(0).GetComponent<Animation>().isPlaying) {
            SceneManager.UnloadSceneAsync("tutorialScreen");
        }
        
    }
}
