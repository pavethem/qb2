﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScreen : MonoBehaviour
{
    public void StartButton() {
        GameController.LoadCurrentScene();
    }    
    
    public void SelectButton() {
//        GameController.LoadCurrentScene();
    }
    
    public void OptionsButton() {
//        GameController.LoadCurrentScene();
    }
}
