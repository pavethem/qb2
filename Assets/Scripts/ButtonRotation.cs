using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonRotation : MonoBehaviour {
    private bool pointerDown;
    private bool direction;

    public void PointerDown(bool direction) {
        pointerDown = true;
        this.direction = direction;
    }
    
    public void PointerUp() {
        pointerDown = false;
    }

    private void Update() {

        int sign = 1;
        if (direction)
            sign = -1;
        
        if(pointerDown) 
            transform.RotateAround(Vector3.zero, Vector3.up, sign * 2);
    }
}
