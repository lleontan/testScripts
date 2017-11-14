using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateOnControlGaze : MenuElement {
	// When this object is raycasted by a controller, activate all specified objects
	//Add deactivator script to activeObj Gameobject.
	public GameObject activeObj;
	public float openDelay=.2f;
	private float openTimer;
	private Hand thisHand;
	void Start(){
		thisHand = this.GetComponent<Hand> ();
		openTimer = openDelay;	
	}
	public override void OnClickUp(Vector3 hitPosition, Transform controller){}
	protected override void OnEnter(Vector3 hitPosition, Transform controller){}
	protected override void OnExit(){
		openTimer = openDelay;
	}
	public override void OnClickDown(Vector3 hitPosition, Transform controller){}
	public override void OnGripUp(Vector3 hitPosition, Transform controller){}
	protected override void OnStay(Vector3 hitPosition, Transform controller){
		if (openTimer < 0 && !activeObj.activeSelf) {
			if (thisHand && thisHand.isHeld()) {
				
			} else {
				openTimer = openDelay;
				activeObj.SetActive (true);
			}
		} else {
			openTimer -= Time.deltaTime;
		}
	}
	public void OnMouseOver(){
		Debug.Log ("Incomplete mouseover");
	}
	protected override void OnGazeEnter(Vector3 hitPosition, Transform controller){}	// HMD Gaze
	protected override void OnGazeStay(Vector3 hitPosition, Transform controller){}
	protected override void OnGazeExit(){}
	protected override void OnClickHold(Vector3 hitPosition, Transform controller){}
}
