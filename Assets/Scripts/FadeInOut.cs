using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class FadeInOut : MonoBehaviour {

    private Color color;
    private Color secondColor;
    private float alpha = 0;
    private float secondalpha = 0;

    private bool fadingOut;
    
    private void OnEnable() {

        fadingOut = false;
        
        if (gameObject.name.Equals("TeleporterParticles")) {
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[gameObject.GetComponent<ParticleSystem>().main.maxParticles];
            color = gameObject.GetComponent<ParticleSystem>().main.startColor.color;
            int particleCount = gameObject.GetComponent<ParticleSystem>().GetParticles(particles);
            for (int i = 0; i < particleCount; i++) {
                particles[i].startColor = new Color(color.r,color.g,color.b,alpha); 
            }
            gameObject.GetComponent<ParticleSystem>().SetParticles(particles);
        } else if (gameObject.name.Equals("ResetButton") || gameObject.name.Equals("RightButton") || gameObject.name.Equals("LeftButton")) {
            color = gameObject.GetComponent<Image>().color;
            gameObject.GetComponent<Image>().color = new Color(color.r, color.g, color.b, alpha);
            secondColor = gameObject.transform.GetChild(0).GetComponent<Text>().color;
            gameObject.transform.GetChild(0).GetComponent<Text>().color = new Color(secondColor.r, secondColor.g, secondColor.b, alpha);
            StartCoroutine(FadeIn());
        } else if (gameObject.CompareTag("key")) {
            color = gameObject.GetComponent<Outline>().OutlineColor;
            gameObject.GetComponent<Outline>().OutlineColor = new Color(color.r,color.g,color.b,alpha);
            secondColor = gameObject.GetComponent<Renderer>().material.color;
            secondalpha = secondColor.a;
            gameObject.GetComponent<Renderer>().material.color = new Color(secondColor.r,secondColor.g,secondColor.b,secondalpha);
        }
        else {
            
            color = gameObject.GetComponent<Renderer>().material.color;
            gameObject.GetComponent<Renderer>().material.color = new Color(color.r,color.g,color.b,alpha);
            
        }
        
        SceneManager.sceneLoaded += OnSceneLoaded;
        
    }
    
    private void OnDisable() {
        
        SceneManager.sceneLoaded -= OnSceneLoaded;
        
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FadeIn());
    }

    private void Update() {

        if (GameController.gameOver && !fadingOut) {
            
            if (gameObject.name.Equals("TeleporterParticles")) {
                color = gameObject.GetComponent<ParticleSystem>().main.startColor.color;
            }
            else if (gameObject.name.Equals("ResetButton") || gameObject.name.Equals("RightButton") || gameObject.name.Equals("LeftButton")) {
                color = gameObject.GetComponent<Image>().color;
            } else if (gameObject.CompareTag("key")) {
                color = gameObject.GetComponent<Outline>().OutlineColor;
                secondColor = gameObject.GetComponent<Renderer>().material.color;
            }
            else {
                color = gameObject.GetComponent<Renderer>().material.color;            
            }
            
            fadingOut = true;
            StartCoroutine(FadeOut());
        }
    }

    internal IEnumerator FadeIn() {
        
        float timeCount = 0;

        while (alpha < 1f) {
            
            alpha = Mathf.Lerp(0, 1, timeCount);

            if (gameObject.name.Equals("TeleporterParticles")) {
                ParticleSystem.Particle[] particles = new ParticleSystem.Particle[gameObject.GetComponent<ParticleSystem>().main.maxParticles];
                int particleCount = gameObject.GetComponent<ParticleSystem>().GetParticles(particles);
                for (int i = 0; i < particleCount; i++) {
                    particles[i].startColor = new Color(color.r,color.g,color.b,alpha);        
                }
                gameObject.GetComponent<ParticleSystem>().SetParticles(particles);
            }else if (gameObject.name.Equals("ResetButton") || gameObject.name.Equals("RightButton") || gameObject.name.Equals("LeftButton")) {
                gameObject.GetComponent<Image>().color = new Color(color.r, color.g, color.b, alpha);
                gameObject.transform.GetChild(0).GetComponent<Text>().color = new Color(secondColor.r, secondColor.g, secondColor.b, alpha);
            } else if (gameObject.CompareTag("key")) {
                gameObject.GetComponent<Outline>().OutlineColor = new Color(color.r,color.g,color.b,alpha);
                if(alpha <= secondalpha)
                    gameObject.GetComponent<Renderer>().material.color = new Color(secondColor.r,secondColor.g,secondColor.b,alpha);
            }
            else {
            
                gameObject.GetComponent<Renderer>().material.color = new Color(color.r,color.g,color.b,alpha);
            
            }


            timeCount += Time.deltaTime;
            yield return null;
        }
        if (gameObject.name.Equals("TeleporterParticles")) {
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[gameObject.GetComponent<ParticleSystem>().main.maxParticles];
            int particleCount = gameObject.GetComponent<ParticleSystem>().GetParticles(particles);
            for (int i = 0; i < particleCount; i++) {
                particles[i].startColor = new Color(color.r,color.g,color.b,alpha);        
            }
            gameObject.GetComponent<ParticleSystem>().SetParticles(particles);
        }else if (gameObject.name.Equals("ResetButton") || gameObject.name.Equals("RightButton") || gameObject.name.Equals("LeftButton")) {
            gameObject.GetComponent<Image>().color = new Color(color.r, color.g, color.b, alpha);
            gameObject.transform.GetChild(0).GetComponent<Text>().color = new Color(secondColor.r, secondColor.g, secondColor.b, alpha);
        } else if (gameObject.CompareTag("key")) {
            gameObject.GetComponent<Outline>().OutlineColor = new Color(color.r,color.g,color.b,alpha);
            gameObject.GetComponent<Renderer>().material.color = new Color(secondColor.r,secondColor.g,secondColor.b,secondalpha);
        }
        else {
            
            gameObject.GetComponent<Renderer>().material.color = new Color(color.r,color.g,color.b,alpha);
            
        }
    }
    
    internal IEnumerator FadeOut(bool wait=true) {
        
        //wait a bit
        if(wait)
            yield return new WaitForSeconds(1);
        
        float timeCount = 0;

        while (alpha > 0f) {
            
            alpha = Mathf.Lerp(1, 0, timeCount);

            if (gameObject.name.Equals("TeleporterParticles")) {
                ParticleSystem.Particle[] particles = new ParticleSystem.Particle[gameObject.GetComponent<ParticleSystem>().main.maxParticles];
                int particleCount = gameObject.GetComponent<ParticleSystem>().GetParticles(particles);
                for (int i = 0; i < particleCount; i++) {
                    particles[i].startColor = new Color(color.r,color.g,color.b,alpha);        
                }
            }else if (gameObject.name.Equals("ResetButton") || gameObject.name.Equals("RightButton") || gameObject.name.Equals("LeftButton")) {
                gameObject.GetComponent<Image>().color = new Color(color.r, color.g, color.b, alpha);
                gameObject.transform.GetChild(0).GetComponent<Text>().color = new Color(secondColor.r, secondColor.g, secondColor.b, alpha);
            } else if (gameObject.CompareTag("key")) {
                gameObject.GetComponent<Outline>().OutlineColor = new Color(color.r,color.g,color.b,alpha);
                if(alpha <= secondalpha)
                    gameObject.GetComponent<Renderer>().material.color = new Color(secondColor.r,secondColor.g,secondColor.b,alpha);
            }
            else {
            
                gameObject.GetComponent<Renderer>().material.color = new Color(color.r,color.g,color.b,alpha);
            
            }

            timeCount += Time.deltaTime;
            yield return null;
        }
        
        if (gameObject.name.Equals("TeleporterParticles")) {
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[gameObject.GetComponent<ParticleSystem>().main.maxParticles];
            int particleCount = gameObject.GetComponent<ParticleSystem>().GetParticles(particles);
            for (int i = 0; i < particleCount; i++) {
                particles[i].startColor = new Color(color.r,color.g,color.b,alpha);        
            }
        }else if (gameObject.name.Equals("ResetButton") || gameObject.name.Equals("RightButton") || gameObject.name.Equals("LeftButton")) {
            gameObject.GetComponent<Image>().color = new Color(color.r, color.g, color.b, alpha);
            gameObject.transform.GetChild(0).GetComponent<Text>().color = new Color(secondColor.r, secondColor.g, secondColor.b, alpha);
        } else if (gameObject.CompareTag("key")) {
            gameObject.GetComponent<Outline>().OutlineColor = new Color(color.r,color.g,color.b,alpha);
            gameObject.GetComponent<Renderer>().material.color = new Color(secondColor.r,secondColor.g,secondColor.b,alpha);
        }
        else {
            
            gameObject.GetComponent<Renderer>().material.color = new Color(color.r,color.g,color.b,alpha);
            
        }
        fadingOut = false;
        gameObject.SetActive(false);
    }
}
