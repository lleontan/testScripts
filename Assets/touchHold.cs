using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class touchHold : OnControllerTouch {
	// Handles on contact trigger holds by controller.
	// While trigger is held set transform to hand position.
	// Use a slider for localPosition events and clamping.
	// Release is handled by controller.
	// Handler is touchHoldHandler
	public bool resetPositionOnRelease = true;
	public Transform touchedTransform;							//The transform that is actually moved. Defaults to this transform;
	private Vector3 holdStartPosition= Vector3.zero;			//Worldspace vector3 of this object when first grabbed.
	private Vector3 handHoldStartPosition= Vector3.zero;		//Worldspace vector3 of the hand when first grabbed.
	public positionSlider slider;								//Optional PositionSlider for clamping
	public touchHoldHandler handler;
	private Controller lastController;					//Currently held controller
	void Awake(){
		if (this.touchedTransform == null) {
			this.touchedTransform = this.transform;
		}
		if (handler == null) {
			handler = this.GetComponent<touchHoldHandler> ();
		}
	}


	//Initial clickdown check, if true then assign controller, otherwise do nothing.
	public bool CheckHeld(){
		//Returns true for held, false for not held
		return this.lastController != null && this.lastController.getCurrentTouchHold () == this;
	}
	public Controller getLastController(){
		return this.lastController;
	}

	private void checkClickDown(Controller controller){
		if (this.lastController == null) {														
			if (controller.getTriggerClickDown () && !controller.isHeld ()) {

				Debug.Log ("TouchHoldStart");
				//Checks to see if a initial click is made when in contact with a controller
				Attachment attach = this.GetComponent<Attachment> ();
				if (attach) {
					attach.AttemptDetach ();
				}
				this.lastController = controller;
				handHoldStartPosition = controller.transform.position;
				holdStartPosition = touchedTransform.position;
				controller.grabTouchHoldObj (this);
				if (handler) {
					handler.onTouchHold ();
				}
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
		Debug.Log("TriggerStayTouchHold");
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
	

		//Worldspace coordinates
		/*Vector3 newPos = controller.transform.position + this.holdStartPosition
			- this.handHoldStartPosition;
		*/
		Vector3 newPos = controller.transform.position;
		if (slider != null) {
			slider.clampPosition (newPos);
		} else {
			touchedTransform.position = newPos;
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
		if (handler) {
			handler.onRelease ();
		}
		if (resetPositionOnRelease) {
			this.holdStartPosition = Vector3.zero;
			this.handHoldStartPosition = Vector3.zero;
		}
		if (lastController != null) {
			//laters
		}
		this.lastController = null;
	}
}
