using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class loadSceneOnTriggerEnter : MonoBehaviour {
	public string sceneName;
	void OnTriggerEnter(Collider col){
		if (col.gameObject.GetComponent<playerRig> ()) {
			GameObject player = col.gameObject;
			Controller[] controllers = FindObjectsOfType (typeof(Controller))as Controller[];
			foreach (Controller controller in controllers) {
				if (controller.pickup != null) {
					DontDestroyOnLoad (controller.pickup.gameObject);
				}
			}
			SceneManager.LoadScene (sceneName);
		}
	}
}
