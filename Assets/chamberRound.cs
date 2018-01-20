using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chamberRound : SliderEvent {
	// Chambers a round when the localY of the reciever is past chamberMagnitude
	// Preforms position delta animation when not held
	public float chamberMagnitude= .1f;	//0.0 to 1.0
	public float chamberReset=0.9f;
	public bool canChamber= true;
	public Gun gun;
	public Transform trackedTrans;
	public touchHold touch;
	public int checkAxisSwitchXYZ = 1;		//x is 1, y is 2, z is 3
	public float positionSpeed=.6f;

	public override void PositionUpdate (float x, float y, float z){
		//Debug.Log ("Chamber "+y);
		switch(checkAxisSwitchXYZ){
		case 1:
			this.chamberPositionCheck (x);
			break;
		case 2:
			this.chamberPositionCheck (y);
			break;
		case 3:
			this.chamberPositionCheck (z);
			break;
		}

	}
	void Start(){
		if (touch == null) {
			this.touch = this.GetComponent<touchHold> ();

		}
		if(touch!=null&&trackedTrans==null){
			this.trackedTrans = this.GetComponentInChildren<touchHold> ().transform;
		}
	}
	private void chamberPositionCheck(float n){
		if (chamberMagnitude > n && canChamber && touch.CheckHeld()&& !gun.isRecoiling()) {
			gun.ChamberRound ();
			canChamber = false;
		}else if(!canChamber&& n>chamberReset){
			canChamber = true;
		}
		if (!touch.CheckHeld ()&& n < chamberReset) {
			Vector3 addVec = Vector3.zero;
			//If this object is not held then lerp its position towards checkAxisXYZ max localposition
			switch(checkAxisSwitchXYZ){
			case 1:
				//.transform.localPosition = new Vector3(transform.localPosition+ positionSpeed * Time.deltaTime,transform.localPosition.y,transform.localPosition.z);
				addVec = new Vector3(this.positionSpeed*Time.deltaTime,0,0);
					
				break;
			case 2:
				//this.transform.localPosition = new Vector3(transform.localPosition.x,transform.localPosition.y+ positionSpeed * Time.deltaTime,transform.localPosition.z);
				addVec = new Vector3(0,this.positionSpeed*Time.deltaTime,0);
					
				break;
			case 3:
				addVec = new Vector3 (0, 0, this.positionSpeed * Time.deltaTime);
				break;
			}
			this.trackedTrans.localPosition = this.trackedTrans.localPosition + addVec;
		}
	}
	public override void ChildOnStart(){
		//trackedTrans = controller.child;
	}
	public override void onPickup(Hand controller){
	}
	public override void onDrop (Hand controller){
	}
	public override void onTriggerClickUp (Controller controller){}
	public override void onTriggerPress (Controller controller){}
	public override void onTriggerClickDown (Controller controller){
	}
	public override bool onGripUp(Controller controller){
		return true;
	}
	public override void touchPadPress (Controller controller, bool triggerUp,
		bool triggerDown, bool triggerPress, bool gripUp, Vector2 touchpad){

	}

}
