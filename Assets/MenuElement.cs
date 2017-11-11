using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class MenuElement: MonoBehaviour {
	/* PlayerObject Interaction Handlers.
	 * Handles controller clicks, playerGaze, controllerGaze.
	*/

	protected bool isActive=false;
	protected bool wasMousedOver=false;

	protected bool GazeActive=false;
	protected bool wasGazedOver=false;

	abstract protected void OnEnter(Vector3 hitPosition, Transform controller);		// Controller raycasts object.
	abstract protected void OnStay(Vector3 hitPosition, Transform controller);
	abstract protected void OnExit();

	abstract public void OnClickUp(Vector3 hitPosition, Transform controller);	//Controller Clicks(Defaults to Trigger)
	abstract public void OnClickDown(Vector3 hitPosition, Transform controller);
	abstract public void OnGripUp(Vector3 hitPosition, Transform controller);

	abstract protected void OnGazeEnter(Vector3 hitPosition, Transform head);	// HMD Gaze
	abstract protected void OnGazeStay(Vector3 hitPosition, Transform head);
	abstract protected void OnGazeExit();
	abstract protected void OnClickHold (Vector3 hitPosition, Transform controller);
	public void OnCastOver(Vector3 hitPosition, bool triggerPress, Transform controller){
		//ClickUps and ClickDowns are handled by Controller.cs
		//Activated from forward raycast of controller.
		if (isActive) {
			//if the object was already raycast
			if (triggerPress) {
				OnClickHold (hitPosition, controller);
			}
			OnStay (hitPosition, controller);	
		} else {
			//If the object was
			OnEnter (hitPosition, controller);
		}
		isActive = true;
		wasMousedOver = true;
	}
	public void OnGazeOver(Vector3 hitPosition, Transform head){
		// Gaze event from a hmdGaze raycaster.
		if (GazeActive) {
			OnGazeStay (hitPosition, head);	
		} else {
			OnGazeEnter (hitPosition, head);
		}
		GazeActive = true;
		wasGazedOver = true;
	}
	void Update(){
		if (wasMousedOver) {
			wasMousedOver = false;
		} else if(isActive){
			this.isActive = false;
			this.OnExit ();
		}
		if (wasGazedOver) {
			wasGazedOver = false;
		} else if(GazeActive){
			this.GazeActive = false;
			this.OnGazeExit ();
		}
	}
}
