using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterCollider : MonoBehaviour {

	public GameObject otherTeleporter;
	//don't teleport if bub has just been teleported there
	private bool teleported;
	//can only teleport, when there is a spoke inside the other teleporter and not another bub
	private bool canTeleport;
	private bool hasBub;
	private bool speedUp;
	private float speedUpTime;
	private const float MAX_SPEEDUPTIME = 3f;
	//new parent for bub
	private Transform newParent;

	private void OnTriggerStay(UnityEngine.Collider other) {
	    
	    if (other.gameObject.CompareTag("spoke")) {
		    canTeleport = true;
		    otherTeleporter.GetComponent<TeleporterCollider>().newParent = other.gameObject.transform;
	    }
	    
	    if (other.gameObject.CompareTag("bub")) {
		    hasBub = true;
	    }
	    
	    //teleport bub to other teleporter
        if (other.gameObject.CompareTag("bub") && !GameController.rotating && !teleported && 
            otherTeleporter.GetComponent<TeleporterCollider>().canTeleport && !otherTeleporter.GetComponent<TeleporterCollider>().hasBub) {
	        other.gameObject.transform.parent = newParent;
	        other.gameObject.GetComponent<Teleporter>()
		        .StartCoroutine("TeleportIt", otherTeleporter.transform.position);
	        otherTeleporter.GetComponent<TeleporterCollider>().teleported = true;
	        canTeleport = false;
	        hasBub = false;
        }

    }

    private void OnTriggerExit(UnityEngine.Collider other) {
	    if (other.gameObject.CompareTag("bub")) {
		    teleported = false;
		    hasBub = false;
	    }

	    if (other.gameObject.CompareTag("spoke")) {
		    canTeleport = false;
		    otherTeleporter.GetComponent<TeleporterCollider>().teleported = false;
	    }
    }
    
    private void OnMouseUpAsButton() {
	    if (!GameController.rotating) {
		    speedUp = true;
	    }
		    
    }

    private void Update() {
	    if (speedUp && speedUpTime < MAX_SPEEDUPTIME) {
		    ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
		    ParticleSystem.MainModule main = ps.main;
		    main.simulationSpeed = 5f;
		    speedUpTime += Time.deltaTime;
	    } else if (speedUp && speedUpTime > MAX_SPEEDUPTIME) {
		    speedUpTime = 0;
		    speedUp = false;
		    ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
		    ParticleSystem.MainModule main = ps.main;
		    main.simulationSpeed = 1f;
	    }
    }
}
