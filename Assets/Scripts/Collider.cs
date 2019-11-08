using UnityEditor;
using UnityEngine;

public class Collider : MonoBehaviour {
	
	private float stayTime;
	private bool collided;
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
}
