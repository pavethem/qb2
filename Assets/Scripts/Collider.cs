using System.Collections;
using UnityEngine;

public class Collider : MonoBehaviour {
	
	private float stayTime;
	private bool collided;
	private bool rotating;
	private const float STAYTIME_THRESHOLD = 0.2f;

	public Material selectedMaterial;
	public Material unselectedMaterial;

    void OnTriggerStay(UnityEngine.Collider other) {
		
		if (other.gameObject.CompareTag("bub") && !gameObject.CompareTag("key") && !gameObject.CompareTag("lock")) {
			if (stayTime > STAYTIME_THRESHOLD && !collided) {
				GameController.cubeCount++;
				collided = true;
			}
			else
				stayTime += Time.fixedDeltaTime;
			
			other.gameObject.transform.Find("particle").gameObject.SetActive(true);
			other.gameObject.GetComponent<MeshRenderer>().material = selectedMaterial;
		}
		
	}

	void OnTriggerExit(UnityEngine.Collider other) 
	{
		if (other.gameObject.CompareTag ("bub"))
		{
			if(collided)
				GameController.cubeCount--;

			other.gameObject.GetComponent<MeshRenderer>().material = unselectedMaterial;
			other.gameObject.transform.Find("particle").gameObject.SetActive(false);
			stayTime = 0f;
			collided = false;
		}
	}
	
	private void OnMouseUpAsButton() {
		if (!GameController.rotating && !rotating && !GameController.rotatingSpoke)
			StartCoroutine(Rotate());
	}

	private IEnumerator Rotate() {
		rotating = true;
		
		int random = Random.Range(1, 7);
		float step = 0;
		Quaternion fromRotation = transform.rotation;
		Quaternion toRotation = Quaternion.identity;

		switch (random) {
			case 1: {
				toRotation = Quaternion.AngleAxis(90f, transform.right) * transform.rotation;
				break;
			}
			case 2: {
				toRotation = Quaternion.AngleAxis(-90f, transform.right) * transform.rotation;
				break;
			}
			case 3: {
				toRotation = Quaternion.AngleAxis(90f, transform.up) * transform.rotation;
				break;
			}
			case 4: {
				toRotation = Quaternion.AngleAxis(-90f, transform.up) * transform.rotation;
				break;
			}
			case 5: {
				toRotation = Quaternion.AngleAxis(90f, transform.forward) * transform.rotation;
				break;
			}
			case 6: {
				toRotation = Quaternion.AngleAxis(-90f, transform.forward) * transform.rotation;
				break;
			}
		}
		
		while (transform.rotation != toRotation) {
			transform.rotation = Quaternion.Slerp(fromRotation, toRotation, step);
			step += Time.deltaTime;
			if (step > 1)
				break;
			yield return null;
		}
		
		transform.rotation = toRotation;
		rotating = false;

	}
}
