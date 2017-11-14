using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hmdGaze : MonoBehaviour {
	//Executes hmdGaze events
	// Update is called once per frame
	public float gazeDistance= 10f;
	void Update () {
		RaycastHit hit;
		if(Physics.Raycast(this.transform.position,this.transform.forward,out hit, gazeDistance)){
			MenuElement menu=hit.collider.gameObject.GetComponent<MenuElement>();
			if(menu){
				menu.OnGazeOver (hit.transform.position, this.transform);
			}
		}
	}
}
