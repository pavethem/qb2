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
    
    private IEnumerator Start() {
        buttonsTransform = Application.isMobilePlatform
            ? GameObject.Find("MobileButtons").GetComponent<RectTransform>()
            : GameObject.Find("DesktopButtons").GetComponent<RectTransform>();

        if (GameController.firstTimeLoading) {
            //hide buttons on first startup
            buttonsTransform.GetComponent<Animation>()["MainMenuIn"].enabled = true;
            buttonsTransform.GetComponent<Animation>()["MainMenuIn"].weight = 1;
            buttonsTransform.GetComponent<Animation>()["MainMenuIn"].time = 0;
            buttonsTransform.GetComponent<Animation>()["MainMenuIn"].speed = 0;
            yield return new WaitForSeconds(2);
            GameController.firstTimeLoading = false;
            buttonsTransform.GetComponent<Animation>()["MainMenuIn"].speed = 1;
        }

        buttonsTransform.GetComponent<Animation>().Play();
    }

    public void StartButton() {
        if (buttonsTransform.GetComponent<Animation>().isPlaying) return;
        buttonsTransform.GetComponent<Animation>().Play("MainMenuOut");
        isLoading = true;
        buttonName = "start";
    }    
    
    public void SelectButton() {
        if (buttonsTransform.GetComponent<Animation>().isPlaying) return;
        buttonsTransform.GetComponent<Animation>().Play("MainMenuOut");
        isLoading = true;
        buttonName = "select";
    }
    
    public void OptionsButton() {
        if (buttonsTransform.GetComponent<Animation>().isPlaying) return;
        buttonsTransform.GetComponent<Animation>().Play("MainMenuOut");
        isLoading = true;
        buttonName = "options";
    }
    
    public void QuitButton() {
        if (buttonsTransform.GetComponent<Animation>().isPlaying) return;
        Application.Quit();
    }

    private void Update() {
        //load new level after menu has moved out of screen
        if (isLoading) {
            if (!buttonsTransform.GetComponent<Animation>().isPlaying) {
                if (buttonName == "start")
                    GameController.instance.LoadLastScene();
                else if (buttonName == "options")
                    SceneManager.LoadScene("optionsmenu");
                else if (buttonName == "select")
                    SceneManager.LoadScene("selectlevelmenu");
            }
        }
    }
}
