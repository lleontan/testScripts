using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menuHoldActive : MonoBehaviour {
	//While menu button is held on the specified contrtoller keep active
	private Controller controller;
	public void Activate(Controller controller){
		this.transform.position = controller.transform.position;
		this.transform.rotation = controller.transform.rotation;
		//this.transform.LookAt (controller.getControllerEntity().head);
		this.controller = controller;
	}
	public void Deactivate(){
		this.controller = null;
		this.gameObject.SetActive (false);
	}
	// Update is called once per frame
	void Update () {
		if (controller&&!controller.getMenuButtonPress ()) {
			this.Deactivate ();
		}	
	}
}
