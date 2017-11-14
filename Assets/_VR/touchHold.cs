using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class touchHold : OnControllerTouch {
	// Handles on contact trigger holds by controller.
	// While trigger is held set transform to hand position.
	// Use a slider for localPosition events and clamping.
	// Release is handled by controller.

	private Vector3 holdStartPosition= Vector3.zero;			//Worldspace vector3 of this object when first grabbed.
	private Vector3 handHoldStartPosition= Vector3.zero;		//Worldspace vector3 of the hand when first grabbed.
	public positionSlider slider;								//Optional PositionSlider for clamping
	private Controller lastController;
	//Initial clickdown check, if true then assign controller, otherwise do nothing.
	public bool CheckHeld(){
		//Returns true for held, false for not held
		return this.lastController != null && this.lastController.getCurrentTouchHold () == this;
	}
	public Controller getLastController(){
		return this.lastController;
	}

	private void checkClickDown(Controller controller){
		//Debug.Log ("checkClickDown");
		if (this.lastController == null) {
			//Debug.Log ("lastControllerNull");
			if (controller.getTriggerClickDown ()&& !controller.isHeld()) {
				//Checks to see if a initial click is made when in contact with a controller
				Attachment attach=this.GetComponent<Attachment>();
				if (attach) {
					attach.AttemptDetach ();
				}
				this.lastController = controller;
				handHoldStartPosition = new Vector3(controller.transform.position.x,
					controller.transform.position.y,controller.transform.position.z);
				holdStartPosition = new Vector3(this.transform.position.x,this.transform.position.y,
					this.transform.position.z);
				controller.grabTouchHoldObj (this);
			}
		} else if (controller == lastController) {
			// If we do have a controller holding this object then set its position
			// and call the clamps.(Clamps are in optional positionalSlider)
			/*this.transform.position = controller.transform.position + this.holdStartPosition
			- this.handHoldStartPosition;*/
		} else {
			//If there was a lastController but it isn't the currentController release the grip.
			//releaseControllerGrip ();
		}
	}

	public override void onTriggerEnter (Controller controller)
	{
		checkClickDown (controller);
	}
	public override void onTriggerStay (Controller controller){
		//Checking initial clickDown.
		checkClickDown (controller);
	}
	public override void onTriggerExit(Controller controller){}
		

	public override void onCollisionEnter(Controller controller){
		checkClickDown (controller);
	}
	public override void onCollisionStay (Controller controller){
		checkClickDown (controller);

	}
	public override void onCollisionExit (Controller controller){}
	public void OnClickHold(Vector3 hitPosition, Controller controller){
		//this.transform.position = holdStartPosition + handHoldStartPosition - controller.transform.position;
	}
	public void OnHandHold(Controller controller){
		//The object is still being held by the controller
		//Debug.Log(this.lastController.transform.position + "::"+this.holdStartPosition+"::"
		//	+ this.handHoldStartPosition);
		Vector3 newPos = controller.transform.position + this.holdStartPosition
			- this.handHoldStartPosition;
		if (slider != null) {
			slider.clampPosition (newPos);
		} else {
			this.transform.position = newPos;
		}
		//Debug.Log ("OnHandHold"+this.lastController.name+ this.transform.localPosition);

	}
	public void OnGripUp(Vector3 hitPosition, Controller controller){
	}
	public void controllerRelease(){
		//Being released from controllers grip by the controller
		this.releaseGrasp();
	}
	public void releaseControllerGrip (){
		if (lastController != null) {
			lastController.releaseGrasp ();
		}
	}
	private void releaseGrasp(){
		this.holdStartPosition = Vector3.zero;
		this.handHoldStartPosition = Vector3.zero;
		if (lastController != null) {
			//laters
		}
		this.lastController = null;
	}
}
