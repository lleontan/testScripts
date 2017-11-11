using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentDistanceBreak : MonoBehaviour {
	//if the distance between this object and its parent is greater than the distanceBreak magnitude then deparent and call handler.
	//if distance is not too great and is not held then reset position.

	public Transform targetBreak;					//The object from which distance is calculated, defaults to the parent of this object.
	public float distanceBreakMag=.4f;				//The distance at which the connection at which the connection between this object and targetBreak is broken.
	public touchHold held;							//The touchHold object given to the connectionBreak handler, defaults to this objects touchHold.
	private Vector3 originalLocalPosition;			//The localPosition of this object at Start();
	public ParentDistanceBreakEvent eventHandler;	//The event handler for breaking and entering connection
	public bool removeParent=true;
	public bool positionReset = true;
	public bool setTargetBreakOnStart=true;			//if true, use initial parent
	private Attachment attachment;					//Attachmentcheck
	private AttachmentSlot slot;
	// Use this for initialization
	void Start () {
		if (this.attachment == null) {
			this.attachment = this.GetComponent<Attachment> ();
		}
		if (this.held == null) {
			this.held = this.GetComponent<touchHold> ();
		}
		if (setTargetBreakOnStart) {
			OnEquip ();
		}
	}
	void Update(){
		if (targetBreak) {
			if (Vector3.Distance (this.transform.position, targetBreak.transform.position) > distanceBreakMag) {
				Vector3 thisPosition = this.transform.position;

				if (this.removeParent) {
					Attachment attach = this.GetComponent<Attachment> ();
					if (attach) {
						attach.AttemptDetach ();
					} else {
						this.transform.SetParent(null,true);
					}
				}
				eventHandler.OnDistanceBreak (held);
				this.transform.position = thisPosition;
				this.positionReset = false;
				targetBreak = null;
				//this.enabled = false;
			} else if (positionReset && held != null && !held.CheckHeld () && this.transform.parent != null) {
				//If a touchHold exists and it is not held while being within the distance constraint return this transforms position to its original value.
				this.transform.localPosition = originalLocalPosition;
				if (attachment&& !this.attachment.isAttached() && this.slot) {
					this.attachment.AttemptAttach (this.slot);
				}
			}
		}
	}
	public void OnEquip(){
		this.positionReset = true;
		//if this object is a attachment and it has been equiped
		if (targetBreak == null) {
			targetBreak = this.transform.parent;
		}
		if (this.held == null) {
			this.held = this.GetComponent<touchHold> ();
		}
		this.originalLocalPosition = this.transform.localPosition;
	}
	public void OnEquip(AttachmentSlot slot){
		this.slot = slot;
		this.OnEquip ();
	}
}
