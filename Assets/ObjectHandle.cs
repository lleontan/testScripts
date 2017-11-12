using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHandle : MonoBehaviour {
	//Manipulation for handle.
	public Vector3 angleOffset= Vector3.zero;
	public int priority = 1;		//Higher is primary, lower is secondary.
	private Holdable holdable;		//Reference to parents holdable script, Assigned in Holdables handshake
	public Transform hand;		//Reference to holding hand.
	private Hand handController = null;
	private Transform parentTrans;		//Reference to parents transform.
	public float xBreakMag=50.0f;		//Break handle connection if not primary and EulerAngle percent is greater than val.
	public float yBreakMag=50.0f;
	public float zBreakMag=50.0f;

	public float maxAngleBreak=40f;
	public float maxDistanceMag=.3f;

	public bool xBreak = false;
	public bool yBreak = false;
	public bool zBreak=false;
	public bool angleBreak=false;
	public bool maxDistanceBreak = true;

	public int controllerZToAxis = 0;		//if true, when primary handle z rotation of this object affects the y rotation of the holdable. Overrides dualHandedZYRotations
	public const int NO_Z_MAP=0;
	public const int X_Z_MAP=1;
	public const int Y_Z_MAP=2;
	public const int Z_Z_MAP=3;

	public bool autoPickup = false;		// Determines whether controllers can autopickup
	public Transform holdTrans;		//Point at which the hand holds the object,
									//If null defaults to this.transform.

	public string defaultLayerName="Default";
	private int defaultLayer=0;
	public string onHoldLayerName="handle";
	private int onHoldLayerInt=0;
	public HandleEvents handleEvents;

	public string getDescription(){
		return this.holdable.getDescription ();
	}
	public float getThrowMultiplier(){
		return this.handController.throwForceMultiplier;
	}
	public virtual bool controllerEvents(Controller controller,bool triggerUp,bool triggerDown,
		bool triggerPress, bool gripUp, Vector2 touchpad, bool menuButtonUp){

		//WARNING: RETURN TRUE TO DROP HANDLE, RETURN FALSE TO KEEP HANDLE.

		if (handleEvents) {
			if (triggerUp) {
				//Executing the OBJECT_HANDLE listeners
				this.handleEvents.onTriggerClickUp (controller);
			} else if (triggerDown) {
				this.handleEvents.onTriggerClickDown (controller);
			} else if (triggerPress) {
				this.handleEvents.onTriggerPress (controller);
			}
			this.handleEvents.touchPadPress (controller, triggerUp, triggerDown, triggerPress, gripUp, touchpad);
			if (gripUp) {
				return this.handleEvents.onGripUp (controller);
			} else {
				return false;
			}
		}else if (gripUp) {
			return true;
		}
		return false;
	}
	public void setHoldable(Holdable newHoldable){
		//Holdable handshake, called by parent with Holdable.
		this.holdable = newHoldable;
		this.parentTrans = holdable.transform;
	}
	void Awake(){
		if (handleEvents == null) {
			this.handleEvents = this.GetComponent<HandleEvents> ();
		}
		onHoldLayerInt = LayerMask.NameToLayer (this.onHoldLayerName);
		this.defaultLayer = LayerMask.NameToLayer (this.defaultLayerName);
		if (this.holdTrans == null) {
			this.holdTrans = this.transform;
		}
	}
	public Vector3 getPosition(){
		return this.holdTrans.position;
	}
	public bool holdableStart(Transform handPos, Hand controller){
		if (this.holdable.canHold){
			if (this.handController) {
				this.handController.fullReleaseGrasp (false);
			}
			if(holdable.attemptAssignHandle (this,controller.handsOwner)) {
			//Check if handRotation is in bounds and assign.
			this.hand = handPos;
			this.handController = controller;
			//Physics.IgnoreCollision(this.GetComponent<Collider>(),
			//	hand.GetComponent<Collider>(),true);

			//Working
			//holdable.assignLayer (LayerMask.NameToLayer ("ignorePlayer"));
				//this.gameObject.layer = onHoldLayerInt;						//WARNING ONHOLDLAYER INT IS TOTALLY BROKEN AT THE MOMENT. DEFAULTS TO ZERO
				this.gameObject.layer=0;
				if (this.handleEvents) {
					this.handleEvents.onPickup (this.handController);
				}
			}
			return true;
		}
		return false;
	}
	public void HoldableReleaseHold(){
		this.HoldableReleaseHold (handController!=null);
	}
	public void HoldableReleaseHold(bool wasHeld){
		//ReleaseHold called by holdable.
		if (this.handController != null) {
			this.handController.setCanAutoPickup (false);
			this.handController.releaseGrasp ();
		}
		this.releaseHold (wasHeld);
	}
	public void ControllerReleaseHold(bool canDepool){
		//Release hold called by controller
		//Physics.IgnoreCollision (
		//	this.GetComponent<Collider> (), hand.root.GetComponent<Collider> (),false);
		//Physics.IgnoreCollision(this.GetComponent<Collider>(),hand.GetComponent<Collider>(),false);

		bool wasHeld=handController!=null;
		if (wasHeld) {
			//this.handController.setCanAutoPickup(false);
			//this.handController.releaseGrasp ();
			this.holdable.objectHandleRelease (this, canDepool);
		}
		if(this.handController){
			this.handController.releaseGrasp();
			this.hand = null;
			this.handController = null;
		}
		//this.releaseHold ();
		HoldableReleaseHold (wasHeld);				//To clear off the references on the old handle
	}
	private void releaseHold(bool wasHeld){
		//Only does releaseing for this object. Do not call except from ControllerReleaseHold or HoldableReleaseHold
		if (wasHeld && this.handleEvents) {
			this.handleEvents.onDrop (handController);
		}
		this.hand = null;
		this.handController=null;
		this.gameObject.layer = this.defaultLayer;
	}
	public bool CheckBounds(Transform primaryHand){
		//Checks distances and angles relative to the other hand.
		//Max distance break is from handle to thisHand
		//returns boolean if out of bounds and true if should be unbroken.
		Vector3 pos=this.hand.position;
		if(this.maxDistanceBreak&&
			(this.transform.position-pos).magnitude>maxDistanceMag){
			return false;
		}
		if(this.angleBreak){
				float angle = Vector3.Angle (
				this.transform.position-primaryHand.transform.position,
				this.hand.position-primaryHand.transform.position);
			if (angle > maxAngleBreak) {
				return false;
			}
		}
		if (this.xBreak && Mathf.Abs(this.transform.position.x-pos.x) > this.xBreakMag) {
			return false;
		}
		if (this.yBreak && Mathf.Abs(this.transform.position.y-pos.y) > this.yBreakMag) {
			return false;
		}
		if (this.zBreak && Mathf.Abs(this.transform.position.z-pos.z) > this.zBreakMag) {
			return false;
		}
		return true;
	}

	public bool checkAutoPickup(){
		// Returns whether the referenced controller can autoPickup
		// Can pick up if- no handcontroller,
		Attachment thisAttachment=this.holdable.gameObject.GetComponent<Attachment>();
		if(thisAttachment&&thisAttachment.isAttached()){
			return false;
		}
		return this.autoPickup;
	}
}
