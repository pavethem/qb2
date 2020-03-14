using System.Collections;
using TMPro;
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
    private bool hardModeComplete;

    public Texture replacementTexture;
    
    // Start is called before the first frame update
    void Start() {
        buttonsTransform = GameObject.Find("Scroll View").GetComponent<RectTransform>();
        contentTransform = buttonsTransform.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
        buttonsTransform.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(
            buttonsTransform.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta.x,
            Mathf.Clamp(Screen.height / 15f, 25f, 30f));
        levelName = "1";
        buttonsTransform.GetComponent<Animation>().Play();

        int levelCount = GameObject.Find("Scroll View").transform.Find("Viewport").GetChild(0).childCount - 1;
        
        //add checkmars if hard mode was completed
        for (int i = 1; i <= GameController.maxScene; i++) {
            if(PlayerPrefs.GetInt("level"+i,-1) == 1)
                GameObject.Find("Scroll View").transform.Find("Viewport").GetChild(0).GetChild(i).Find("Panel").gameObject.SetActive(true);
        }
        if(PlayerPrefs.GetInt("level99",-1) == 1)
            GameObject.Find("Scroll View").transform.Find("Viewport").GetChild(0).Find("Bonus").Find("Panel").gameObject.SetActive(true);

        //replace textures with locks, when levels are not yet unlocked
        for (int i = GameController.maxScene + 1; i <= levelCount; i++) {
            GameObject.Find("Scroll View").transform.Find("Viewport").GetChild(0).GetChild(i).Find("LevelImage").GetComponent<RawImage>().texture =
                replacementTexture;
        }

        //unlock bonus level if all other levels are completed in hard mode
        if (GameController.gameCompleted) {
            hardModeComplete = true;
            for (int i = 1; i <= GameController.LEVELCOUNT; i++) {
                hardModeComplete &= PlayerPrefs.GetInt("level" + i, -1) == 1;
            }

            if (hardModeComplete) {
                GameObject.Find("Scroll View").transform.Find("Viewport").GetChild(0).Find("Bonus").Find("LevelImage").gameObject.SetActive(false);
                GameObject.Find("Scroll View").transform.Find("Viewport").GetChild(0).Find("Bonus").Find("QuestionMark").gameObject.
                    GetComponent<TextMeshProUGUI>().enabled = true;
            }
        }

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
        isMoving = false;
        buttonsTransform.GetComponent<Animation>().Play("LevelSelectOut");
    }

    public void LevelSelectButton(string buttonName) {
        if(isMoving || int.Parse(buttonName) > GameController.maxScene || buttonsTransform.GetComponent<Animation>().isPlaying) return;
        if (buttonName == "-99" && !hardModeComplete) return;
        StartCoroutine(ResetContent());
        isLoading = true;
        levelName = buttonName;
    }
    
    public void BackButton() {
        if(isMoving || buttonsTransform.GetComponent<Animation>().isPlaying) return;
        StartCoroutine(ResetContent());
        isLoading = true;
        levelName = "back";
    }
    
    private void Update() {
        //load new level after menu has moved out of screen
        if (isLoading) {
            if (!buttonsTransform.GetComponent<Animation>().isPlaying && !isMoving) {
                if(levelName == "back")
                    SceneManager.LoadSceneAsync("mainmenu");
                else {
                    if (levelName == "-99")
                        GameController.currentScene = 99;
                    else
                        GameController.currentScene = int.Parse(levelName);
                    PlayerPrefs.SetInt("currentScene", GameController.currentScene);
                    PlayerPrefs.Save();
                    GameController.instance.LoadCurrentScene();
                }
            }
        }
    }
    
}
