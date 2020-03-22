using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectionCamera : MonoBehaviour {
    
    public float FPS = 10f;
    private Camera renderCam;
    
    void Start () {
        renderCam = GetComponent<Camera>();
        InvokeRepeating ("Render", 0f, 1f / FPS);
    }
    void Render(){
        renderCam.enabled = true;
    }
    void OnPostRender(){
        renderCam.enabled = false;
    }
    
}
