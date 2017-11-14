using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(OnAttach))]
[RequireComponent(typeof(Collider))]
public class AttachmentSlot : MonoBehaviour {
	// Seen by attachments when their triggerEnter() is called;
	// Parents the Attachment to the attachmentParentTransform, sets rigidbodies to kinematic and noGrav
	// WARNING: detachment should be done on the visualAttachment through a ParentDistanceBreak.
	public string SlotName="nullAttachmentSlot";
	public string slotTypeName="small";
	private int slotType;
	private int slotID;
	private Attachment currentAttachment;					
	public Attachment intialAttachment;						//Assign in editor to attach a attachment on GameStart
	public Transform attachmentParentTransform;				//The object that attachments are parented to at localPosition Vector3.zero and zero localRotation
	public bool canDetach=true;					//if true can detach
	public bool useSlotType=false;
	public bool canAttach=true;					//if true, attachments are allowed
	public string onAttachLayer="onlyDefault";
	public bool changeLayerOnAttach=false;
	private int defaultLayer;
	public bool acceptAll = false;
	public string[] acceptAllExceptions;
	private int[] acceptAllExceptionsInt;
	public bool canAttachWhenHeld=true;
	public bool getCanAttachWhileHeld(){
		return this.canAttachWhenHeld;
	}
	void Awake(){
		if(attachmentParentTransform==null){
			this.attachmentParentTransform = this.transform;
		}
	}
	void Start(){
		int acceptAllExLength = acceptAllExceptions.Length;
		acceptAllExceptionsInt=new int[acceptAllExLength];
		for (int i=0;i<acceptAllExLength;i++) {
			acceptAllExceptionsInt [i] = acceptAllExceptions [i].GetHashCode ();
		}
		defaultLayer = LayerMask.NameToLayer ("Default");
		ChangeSlotID (SlotName);
		this.ChangeSlotType (this.slotTypeName);
		if (this.intialAttachment) {
			this.intialAttachment.AttemptAttach (this);
		}
	}
	public void ChangeSlotID(string slotName){
		slotID = slotName.GetHashCode ();
	}	
	public void ChangeSlotType(string slotName){
		this.slotType = slotName.GetHashCode ();
	}
	public bool CanAttach(int id, int type){
		//Returns true if the slot can attach to either the id or the attachment type.
		if(this.currentAttachment==null&&this.canAttach){
			if(this.acceptAll){
				foreach (int exceptionID in acceptAllExceptionsInt) {
					if(type==exceptionID|| id==this.slotID){
						return false;
					}
				}
				return true;
			}
			if ((id == slotID  || (this.useSlotType&&this.slotType==type))) {
				return true;
			}
		}
		return false;
	}
	public void Attach(Attachment attachment){		//Warning, does not reset any Attachment settings, only call From Attachment.cs's Attach()
		this.currentAttachment = attachment;
		attachment.transform.parent = this.attachmentParentTransform;
		Rigidbody attachmentRigid = attachment.GetComponent<Rigidbody> ();
		if(attachmentRigid){
			attachmentRigid.isKinematic = true;
			attachmentRigid.useGravity = false;
		}
		if (this.changeLayerOnAttach) {
			this.defaultLayer = currentAttachment.gameObject.layer;
			this.currentAttachment.gameObject.layer = LayerMask.NameToLayer (this.onAttachLayer);
		} else {
			this.defaultLayer = -1;
		}
		ParentDistanceBreak distanceBreak = attachment.GetComponent<ParentDistanceBreak> ();
		if (distanceBreak) {
			distanceBreak.OnEquip (this);
		}
		this.GetComponent<OnAttach> ().onAttach (attachment);
	}
	public void AttemptDetach(){
		if (this.currentAttachment) {
			this.currentAttachment.AttemptDetach ();
		}
	}
	public void Detach(){
		//Can detach check is within Attachment.cs's attemptDetach()
		//WARNING, does not reset any Attachment settings, only call From Attachment.cs's Detach()
		//Physics.IgnoreCollision (currentAttachment.GetComponent<Collider>(),this.GetComponent<Collider>(),false);
		if (this.changeLayerOnAttach&&this.defaultLayer!=-1) {
			this.currentAttachment.gameObject.layer = defaultLayer;
		}
		this.GetComponent<OnAttach> ().onDetach ();
		this.currentAttachment = null;
	}
	public Attachment getAttachment (){
		return this.currentAttachment;
	}
	public void DestroyAttachment(){
		//Destroys and detaches the attachment in this attachmentSlot if it exists
		GameObject attachment=this.currentAttachment.gameObject;
		if (attachment) {
			currentAttachment.AttemptDetach ();
			Destroy (attachment);
		}
	}
	public Transform GetAttachmentParentTransform(){
		return this.attachmentParentTransform;
	}
}
