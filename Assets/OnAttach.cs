using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnAttach : MonoBehaviour {
	/* Handles special attachmentBehavior for OnAttach(), OnDetach for Attachment and AttachmentSlots*/
	protected AttachmentSlot slot;
	protected virtual void Start(){
		this.slot=this.GetComponent<AttachmentSlot>();
	}
	public virtual void onTriggerClickUp(Controller controller){
	}
	public virtual void onAttach(Attachment attachment){
	}
	public virtual void onDetach(){
	}
	public virtual void onLateUpdate(){
		
	}
}
