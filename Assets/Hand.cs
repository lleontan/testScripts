using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour {
	/* IMPORTANT: 
	 * Hand controls the holding of objects.
	 * Only one transform can be held at a time.
	 * There are 3 objects for the type of relationship between the held object and the hand.
	 * heldTransform- is the objects transform
	 * heldObj is the touchHold
	 * pickup is the objectHandle.
	 * heldTransform will never be null if held but either heldObj or pickup will be null.
	 * Note: Only objectHandles have autopickup capability.
	*/
	protected Transform heldTransform;
	public ObjectHandle pickup;

	public entity handsOwner;
	public float throwForce=100f; 
	protected bool canAutoPickup = true;		//can the hand currently autopickup? Baisically autopickup cooldown
	public bool autopickup=false;				//Can the hand autopickup at all?
	public float autoPickupCoolDown = .4f;		//Max time for cooldown
	protected float autoPickupDuration=0f;		//Current cooldown time.
	public float throwForceMultiplier=2.5f;
	public string onPickupLayer="Default";		//Layer that this hand changes to when a object is picked up
	protected int defaultLayerInt=0;
	protected int onPickupLayerInt;
	public string onDropLayer="ignoreDefault";
	private int onDropLayerInt;
	public float dropDuration=.5f;
	private float currentDropDuration;
	private bool justDropped=false;
	protected virtual void Start(){
		this.onDropLayerInt = LayerMask.NameToLayer (this.onDropLayer);
		defaultLayerInt=this.gameObject.layer;
		this.onPickupLayerInt = LayerMask.NameToLayer (onPickupLayer);
		if (this.handsOwner == null) {
			this.handsOwner = this.transform.root.GetComponent<entity> ();
		}
	}
	public virtual bool getCanAutoPickup(){
		//Can the controller preform a autoPickup?
		//Autopickups are only possible if nothing is held
		return this.autopickup && canAutoPickup && !isHeld();
	}
	public virtual bool isHeld(){
		//Returns true if this hand is holding an object
		return this.pickup != null || heldTransform != null;
	}
	public void setCanAutoPickup(bool setVal){
		//Does not affect base autopickup ability.
		this.canAutoPickup = setVal;
	}
	protected virtual void OnCollisionEnter(Collision collision){
		//On Collision enter execute OnTouch handlers.
		//Overload this method.
		if (this.getCanAutoPickup()) {
			ObjectHandle handle = collision.collider.GetComponent<ObjectHandle> ();
			if ( handle	&& handle.checkAutoPickup ()) {
				//If the handle exists and says it can be autoPickuped
				this.PickUpObjectHandle (handle);
			}
		}
	}
	public virtual void releaseGrasp(){
		//Resets held references to null
		//YOU MUST RELEASE REFERENCES OF PICKUPS/OBJECTHANDLES BEFORE CALLING THIS METHOD.
		this.heldTransform = null;
		this.pickup = null;
		this.canAutoPickup = false;
		//this.gameObject.layer = this.defaultLayerInt;
		startJustDroppedTimer ();

	}
	public void startJustDroppedTimer(){
		justDropped = true;
		currentDropDuration = dropDuration;
		this.gameObject.layer = this.onDropLayerInt;
	}
	public virtual void fullReleaseGrasp(bool canDepool){
		//RELEASES GRASP, CALLS OBJECTHANDLE RELEASE
		//Controller calls touchHoldRelease
		if (pickup) {
			pickup.ControllerReleaseHold (canDepool);
		}
		this.releaseGrasp ();
	}
	public virtual void grabTouchHoldObj(touchHold newTouch){
		//Grabs a touchHold object
		this.heldTransform = newTouch.transform;
		this.pickup = null;
	}
	public virtual void PickUpObjectHandle(ObjectHandle handle){
		this.fullReleaseGrasp (false);
		handle.ControllerReleaseHold (false);
		if (handle.holdableStart (this.transform, this)) {
			this.pickup = handle;
			this.heldTransform = handle.transform;
			this.gameObject.layer = onPickupLayerInt;
		} else {
			Debug.Log ("Unsuccessful attempt to pickup object handle: "+handle.name);
		}
	}
	protected virtual void OnTriggerEnter(Collider col){
		//On TriggerEnter execute OnTriggerTouch handlers.
		//Overload this method.
		if (!this.isHeld()) {
			ObjectHandle handle=col.GetComponent<ObjectHandle>();
			if (handle && this.getCanAutoPickup () && handle.checkAutoPickup()) {
				this.PickUpObjectHandle (handle);
			}
		}
	}
	protected virtual void OnTriggerStay(Collider col){
		//On TriggerEnter execute OnTriggerTouch handlers.
		//Overload this method.
		if (!this.isHeld()) {
			ObjectHandle handle=col.GetComponent<ObjectHandle>();
			if (handle && this.getCanAutoPickup () && handle.checkAutoPickup()) {
				this.PickUpObjectHandle (handle);
			}
		}
	}
	protected virtual void Update(){
		if (autopickup&&!canAutoPickup&&!isHeld()) {
			//If we can autopickup but autopickup is on Cooldown and we are not held.
			if (this.autoPickupDuration > this.autoPickupCoolDown) {
				this.canAutoPickup = true;
				this.autoPickupDuration = 0f;
			} else {
				this.autoPickupDuration+=Time.deltaTime;
			}

		}
		if (this.justDropped) {
			this.currentDropDuration -= Time.deltaTime;
			if (currentDropDuration < 0) {
				this.gameObject.layer = this.defaultLayerInt;
				justDropped = false;
			}
		}
	}

}
