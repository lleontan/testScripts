using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlGazeDeactivator : MenuElement {
	//Deactivates this Gameobject if a controller is not pointed towards it for the specified period of time.
	public float deactivationDelay=.2f;
	private float closeTimer;
	void Start(){
		closeTimer = deactivationDelay;	
	}
	void Update(){
		if (closeTimer < 0) {
			this.closeTimer = deactivationDelay;
			this.gameObject.SetActive (false);
		} else {
			this.closeTimer -= Time.deltaTime;
		}
	}
	public override void OnClickUp(Vector3 hitPosition, Transform controller){}
	protected override void OnEnter(Vector3 hitPosition, Transform controller){}
	protected override void OnExit(){}
	public override void OnClickDown(Vector3 hitPosition, Transform controller){}
	public override void OnGripUp(Vector3 hitPosition, Transform controller){}
	protected override void OnStay(Vector3 hitPosition, Transform controller){
		this.closeTimer = this.deactivationDelay;
	}
	protected override void OnGazeEnter(Vector3 hitPosition, Transform controller){}	// HMD Gaze
	protected override void OnGazeStay(Vector3 hitPosition, Transform controller){}
	protected override void OnGazeExit(){}
	protected override void OnClickHold(Vector3 hitPosition, Transform controller){}

}
