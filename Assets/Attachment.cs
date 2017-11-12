using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Rigidbody))]
public class Attachment : MonoBehaviour {
	/* Attachments are attached to objects when the attachment enters the objects correct triggerBox for its attachment ID
	 * For initializing a object with attachments on GameStart assign this attachment in AttachmentSlot.cs's currentAttachment
	 * 
	 * Warning, do not preinitialize rigidbody.
	*/
	public string slotTypeName="small";			//Pairs with slots of this type
	private int slotType;						//private hash of slotTypeName
	public string slotName="nilSlot";			//Pairs with slots of this name.
	private int slotID;							//private hash
	public AttachmentSlot currentSlot;			//Currently attached slot
	public bool destroyRigidbodyOnEquip=false;	//If true destroys this objects rigidbody when equiped
	public float rigidbodyMass = 3f;			//When readding the rigidbody set this much mass
	public float canAttachTimer=.3f;			//When unattached duration before it can be reattached
	private float canAttach = 0f;				//if greater than 0, cannot attach
	Vector3 lastPosition;
	public OnAttach handler;
	private Holdable thisHoldable;
	void Awake(){
		//On awake initializes Holdable and OnAttach.
		thisHoldable = this.GetComponent<Holdable> ();
		if (handler == null) {
			handler = this.GetComponent<OnAttach> ();
		}
		ChangeSlotID (this.slotName);
		this.ChangeSlotType (this.slotTypeName);
		lastPosition = this.transform.position;
	}

	public void ChangeSlotID(string slotName){
		slotID = slotName.GetHashCode ();
	}
	public void ChangeSlotType(string slotName){
		this.slotType = slotName.GetHashCode ();
	}
	void OnTriggerEnter(Collider col){
		AttachmentSlot slot=col.GetComponent<AttachmentSlot>();
		if (slot) {
			if (thisHoldable && (thisHoldable.isHeld() &&slot.getCanAttachWhileHeld()||!thisHoldable.isHeld()) || thisHoldable == null) {
				this.AttemptAttach (slot);
			}
		}
	}
	public void setCanAttach(float newTime){
		this.canAttach = newTime;
	}
	void Update(){
		if (canAttach > 0) {
			this.canAttach -= Time.deltaTime;
		} else if(isAttached()){
			//this.transform.position = currentSlot.GetAttachmentParentTransform ().position;
			//this.lastPosition = this.transform.position;
			//this.transform.rotation = currentSlot.GetAttachmentParentTransform ().rotation;

		}
	}
	public void AttemptAttach(AttachmentSlot slot){
		//Debug.Log ("AttemptAttach"+ slot!=null+":"+!isAttached()+":"+slot.CanAttach(this.slotID));
		if (!isAttached() && canAttach<=0 && slot.CanAttach(this.slotID, this.slotType)) {
			//If this attachment has encountered a AttachmentSlot and does not have a attachmentSlot and the slot has given the goAhead for attachment;
			Attach(slot);
		}
	}
	private void Attach(AttachmentSlot slot){
		this.canAttach = canAttachTimer;
		Debug.Log ("AttachCheck::"+slot.name);
		currentSlot = slot;
		this.transform.parent = slot.GetAttachmentParentTransform ();
		this.transform.position = slot.GetAttachmentParentTransform ().position;
		this.transform.rotation = slot.GetAttachmentParentTransform ().rotation;
		Rigidbody thisRigid = this.GetComponent<Rigidbody> ();
		if (thisRigid) {
			if (destroyRigidbodyOnEquip) {
				Destroy (thisRigid);
			} else {
				thisRigid.velocity = Vector3.zero;
				thisRigid.isKinematic = true;
				thisRigid.useGravity = false;
			}
		}
		//this.transform.parent = slot.GetAttachmentParentTransform ();
		/*Physics.IgnoreCollision (this.GetComponent<Collider>(),slot.GetComponent<Collider>(),true);
	

		//this.transform.localPosition = Vector3.zero;
		this.transform.position=this.transform.parent.position;
		this.transform.rotation = this.transform.parent.rotation;*/
		Holdable thisHoldable = this.GetComponent<Holdable> ();
		if (thisHoldable) {
			thisHoldable.releaseAll (false);
		}
		this.currentSlot.Attach (this);
		if (handler) {
			handler.onAttach (this);
		}
	}
	public void AttemptDetach(){
		Debug.Log (isAttached()+"::"+canDetach());
		//Debug.Log ("CurrentSlot:" + this.currentSlot ==null);//Returns true,
		if(currentSlot){
			Debug.Log ("Attempted Detach:"+this.currentSlot.gameObject.name);//Returns an error indicating null
		}
		if (canDetach()&&this.currentSlot.canDetach) {
			Debug.Log ("canDetach checked success");
			this.currentSlot.Detach ();
			Detach ();
		}

	}
	private void Detach(){				//warning only detaches this attachment not the slot. Only call from Attachment.cs's AttemptDetach()
		this.canAttach = canAttachTimer;
		this.transform.SetParent(null,true);
		if (isAttached()) {
			this.currentSlot.Detach ();
		}
		currentSlot = null;
		//this.transform.position = lastPosition;

		if(this.GetComponent<Rigidbody> ()){
			Rigidbody body = this.gameObject.GetComponent<Rigidbody> ();
			body.isKinematic = false;
			body.useGravity = true;
		}else if (destroyRigidbodyOnEquip) {
			Rigidbody body = this.gameObject.AddComponent<Rigidbody> ();
			body.isKinematic = false;
			body.useGravity = true;
			body.mass = this.rigidbodyMass;
		}
		if (handler) {
			handler.onDetach ();
		}
	}
	public bool canDetach(){
		return isAttached() && currentSlot.canDetach;
	}
	public bool isAttached(){
		//returns true if this attachment is already attached to a attachmentSlot;
		return this.currentSlot!=null && this.transform.parent!=null;
	}

}
