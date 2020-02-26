using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class TutorialScreen : MonoBehaviour {
    public GameObject description;
    public GameObject nextButtonText;
    public GameObject video;
    private bool destroy;

    //replace webgl video, then wait 1 frame (only for level 1 tutorial)
    private bool replaceWebgl1;

    private int currentDescriptionNumber = 0;
    private string[] currentDescription;

    private string[] level1_pc = {
        "Please align the red <smallcaps><color=red>Nodes</color></smallcaps> with the black <smallcaps>Cubes</smallcaps>.",
        "To do so, you can rotate the <b>Object</b> by dragging the mouse along any of the colored Strips surrounding the <b>Object</b>.",
        "You can also use the Q,W,E,A,S or D keys to rotate the <b>Object</b>."
    };

    private string[] level1_mobile = {
        "Please align the <smallcaps><color=red>Nodes</color></smallcaps> with the black <smallcaps>Cubes</smallcaps>.",
        "To do so, you can rotate the <b>Object</b> by dragging your finger along any of the colored Strips surrounding the <b>Object</b>."
    };

    private string[] level2_pc = {
        "You can also rotate the entire Scene by moving the mouse while holding the right mouse button."
    };

    private string[] level2_mobile = {
        "You can also rotate the entire Scene by dragging your finger along the transparent Strip on the bottom."
    };

    private string[] level2_mobile_free = {
        "You can also rotate the entire Scene by dragging your finger along the transparent Strip on the bottom.",
        "When Free Rotation is enabled, you can also rotate the Scene vertically, by dragging your finger along the transparent Strip on the right."
    };

    private string[] level4 = {
        "You cannot rotate the <b>Object</b> through these <smallcaps>Locks</smallcaps>.\nPlease find a way around them in order to solve the level."
    };

    private string[] level6 = {
        "Aligning any <smallcaps><color=red>Node</color></smallcaps> with a <smallcaps><alpha=#99>Key</color></smallcaps> will attach the <smallcaps><alpha=#99>Key</color></smallcaps> to that <smallcaps><color=red>Node</color></smallcaps>.",
        "You can now remove one <smallcaps>Lock</smallcaps> from the Scene by aligning the <smallcaps><color=red>Node</color></smallcaps>, that holds the <smallcaps><alpha=#99>Key</color></smallcaps> and any <smallcaps>Lock</smallcaps>."
    };

    private string[] level8 = {
        "<smallcaps>Teleporters</smallcaps> will teleport <smallcaps><color=red>Nodes</color></smallcaps> to each other, but only if an empty <smallcaps><alpha=#99>Spoke</color></smallcaps> is aligned with the other <smallcaps>Teleporter</smallcaps>."
    };

    private string[] level12 = {
        "<smallcaps>Arrows</smallcaps> will push <smallcaps><color=red>Nodes</color></smallcaps> in the direction they are pointing.",
        "However if there is no <smallcaps><alpha=#99>Spoke</color></smallcaps> in the direction the <smallcaps>Arrow</smallcaps> is pointing, nothing will happen."
    };

    private string[] level19 = {
        "When a <smallcaps><color=red>Node</color></smallcaps> enters a <smallcaps>Rotator</smallcaps>, one <smallcaps><alpha=#99>Spoke</color></smallcaps> will rotate around another in the direction the <smallcaps>Rotator</smallcaps> is pointing.",
        "Different angles of entry will yield different results."
    };

    void Start() {
        
        //sometimes videos need to be adjusted slightly
        Rect uvRect = video.GetComponent<RawImage>().uvRect;
        
        switch (GameController.currentScene) {
            case 1: {
                if (!Application.isMobilePlatform) {
                    description.GetComponent<TextMeshProUGUI>().text = level1_pc[0];
                    currentDescription = level1_pc;
                    if (Application.platform != RuntimePlatform.WebGLPlayer) 
                        video.GetComponent<VideoPlayer>().Pause();
                }
                else {
                    description.GetComponent<TextMeshProUGUI>().text = level1_mobile[0];
                    currentDescription = level1_mobile;
                    video.GetComponent<VideoPlayer>().clip = Resources.Load<VideoClip>("tutorial_rotation_mobile");
                    if (Application.platform != RuntimePlatform.WebGLPlayer) 
                        video.GetComponent<VideoPlayer>().Pause();
                }

                break;
            }
            case 2: {
                if (!Application.isMobilePlatform) {
                    description.GetComponent<TextMeshProUGUI>().text = level2_pc[0];
                    currentDescription = level2_pc;
                    video.GetComponent<VideoPlayer>().clip = Resources.Load<VideoClip>("tutorial_camera_pc");
                }
                else {
                    description.GetComponent<TextMeshProUGUI>().text = level2_mobile[0];
                    currentDescription = level2_mobile;
                    video.GetComponent<VideoPlayer>().clip = Resources.Load<VideoClip>("tutorial_camera_mobile");
                    //don't want to do the video again
                    uvRect.y = 0.03f;
                    video.GetComponent<RawImage>().uvRect = uvRect;
                    if (GameController.freeRotation == 1) {
                        description.GetComponent<TextMeshProUGUI>().text = level2_mobile_free[0];
                        currentDescription = level2_mobile_free;
                    }
                }

                break;
            }
            case 4: {
                description.GetComponent<TextMeshProUGUI>().text = level4[0];
                currentDescription = level4;
                video.GetComponent<VideoPlayer>().clip = Resources.Load<VideoClip>("tutorial_lock");
                uvRect.y = 0.03f;
                video.GetComponent<RawImage>().uvRect = uvRect;
                break;
            }
            case 6: {
                description.GetComponent<TextMeshProUGUI>().text = level6[0];
                currentDescription = level6;
                video.GetComponent<VideoPlayer>().clip = Resources.Load<VideoClip>("tutorial_key");
                break;
            }
            case 8: {
                description.GetComponent<TextMeshProUGUI>().text = level8[0];
                currentDescription = level8;
                video.GetComponent<VideoPlayer>().clip = Resources.Load<VideoClip>("tutorial_teleporter");
                uvRect.y = 0.03f;
                video.GetComponent<RawImage>().uvRect = uvRect;
                break;
            }
            case 12: {
                description.GetComponent<TextMeshProUGUI>().text = level12[0];
                currentDescription = level12;
                video.GetComponent<VideoPlayer>().clip = Resources.Load<VideoClip>("tutorial_arrow1");
                uvRect.y = 0.03f;
                video.GetComponent<RawImage>().uvRect = uvRect;
                break;
            }
            case 19: {
                description.GetComponent<TextMeshProUGUI>().text = level19[0];
                currentDescription = level19;
                video.GetComponent<VideoPlayer>().clip = Resources.Load<VideoClip>("tutorial_rotator1");
                uvRect.y = 0.03f;
                video.GetComponent<RawImage>().uvRect = uvRect;
                break;
            }
            default: {
                description.GetComponent<TextMeshProUGUI>().text = "Please align the red <color=red>Nodes</color> with the black Cubes.";
                currentDescription = null;
                break;
            }
        }

        if (Application.platform == RuntimePlatform.WebGLPlayer) {
            ReplaceForWebGL();
        }

        //if description only has one element, display Finish button
        if (currentDescription != null && currentDescription.Length == 1) {
            nextButtonText.GetComponent<TextMeshProUGUI>().text = "Finish";
        }
        
        //don't play on awake (but show first frame)
        video.GetComponent<VideoPlayer>().Pause();
    }

    private void ReplaceForWebGL() {
        video.GetComponent<VideoPlayer>().url = System.IO.Path.Combine(Application.streamingAssetsPath,
              video.GetComponent<VideoPlayer>().clip.name + ".ogv");
        if (GameController.currentScene == 1)
            replaceWebgl1 = true;
    }

    private void NextText() {
        currentDescriptionNumber++;
        description.GetComponent<TextMeshProUGUI>().text = currentDescription[currentDescriptionNumber];
        if ((currentDescription == level1_pc || currentDescription == level1_mobile) && !video.GetComponent<VideoPlayer>().isPlaying) {
            video.GetComponent<VideoPlayer>().Play();
        }

        if (currentDescription == level2_mobile_free) {
            video.GetComponent<VideoPlayer>().clip = Resources.Load<VideoClip>("tutorial_camera_free");
            video.GetComponent<RawImage>().uvRect = new Rect(0,0,1,1);
        }

        if (currentDescription == level6) {
            video.GetComponent<VideoPlayer>().clip = Resources.Load<VideoClip>("tutorial_key_open");
        }
        
        if (currentDescription == level12) {
            video.GetComponent<VideoPlayer>().clip = Resources.Load<VideoClip>("tutorial_arrow2");
        }
        
        if (currentDescription == level19) {
            video.GetComponent<VideoPlayer>().clip = Resources.Load<VideoClip>("tutorial_rotator2");
        }

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            ReplaceForWebGL();
        }
    }

    public void NextButton() {
        if (currentDescription != null) {
            //if last description is reached, destroy
            if (description.GetComponent<TextMeshProUGUI>().text == currentDescription[currentDescription.Length - 1]) {
                PlayerPrefs.SetInt("seenTutorial" + GameController.currentScene, 1);
                transform.GetChild(0).GetComponent<Animation>().Play("MainMenuOut");
                GameController.tutorialLock = false;
                GameController.instance.StartCoroutine("MoveInButtons");
                if (Application.isMobilePlatform && GameController.freeRotation == 1)
                    GameController.instance.StartCoroutine("MoveInFromTheRight");
                destroy = true;
                //if second to last description is reached, display Finish button    
            }
            else if (description.GetComponent<TextMeshProUGUI>().text == currentDescription[currentDescription.Length - 2]) {
                nextButtonText.GetComponent<TextMeshProUGUI>().text = "Finish";
                NextText();
            }
            else {
                NextText();
            }
        }
    }

    public void SkipButton() {
        PlayerPrefs.SetInt("skipTutorials", 1);
        GameController.skipTutorials = true;
        transform.GetChild(0).GetComponent<Animation>().Play("MainMenuOut");
        GameController.tutorialLock = false;
        GameController.instance.StartCoroutine("MoveInButtons");
        if (Application.isMobilePlatform && GameController.freeRotation == 1)
            GameController.instance.StartCoroutine("MoveInFromTheRight");
        destroy = true;
    }

    private void LateUpdate() {
        if (replaceWebgl1) {
            video.GetComponent<VideoPlayer>().Pause();
            replaceWebgl1 = false;
        }
        //wait until menu is moved out and unload scene
        if (destroy && !transform.GetChild(0).GetComponent<Animation>().isPlaying) {
            SceneManager.UnloadSceneAsync("tutorialScreen");
        }
        //wait until screen is moved in, then play
        if (transform.GetChild(0).GetComponent<RectTransform>().pivot.y == 0.5f && !video.GetComponent<VideoPlayer>().isPlaying && 
            currentDescription != level1_pc && currentDescription != level1_mobile) {
            video.GetComponent<VideoPlayer>().Play();
        }
    }
}