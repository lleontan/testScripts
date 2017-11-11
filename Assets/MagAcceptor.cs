using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AttachmentSlot))]
public class MagAcceptor : OnAttach {
	/* Is a AttachmentSlot Handler for an attachment slot.
	 * Accepts magazines, handles the magazineObject, gives magazine to Gun
	*/
	public Magazine loadMagOnStart;		//Will load this magazine on start. Magazine must be in resources folder.
										//WARNING, DO NOT USE IN CONJUNCTION WITH ATTACHMENTS INITIAL_ATTACHMENT
	public Gun gun;						//Defaults to the Gun of root transform.
	public AttachmentSlot GetSlot(){
		return this.slot;
	}
	public virtual void Awake(){
		if (this.gun == null) {
			this.gun = this.transform.root.GetComponent<Gun> ();
		}
		if (this.slot == null) {
			this.slot = this.GetComponent<AttachmentSlot> ();
		}
		if (loadMagOnStart) {
			this.loadMagazine (loadMagOnStart);
		}
	}
	public override void onDetach ()
	{
		//Is called before the attachmentSlot gets rid of the attachment
		this.gun.releaseMagazine ();
	}
	public override void onAttach (Attachment attachment)
	{
		Magazine mag = attachment.GetComponent<Magazine>();
		if (mag) {
			loadMagazine (mag);
		}
	}
	protected void giveGunMagazine(Magazine newMag){
		this.gun.LoadMagazine (newMag);	
	}
	public virtual void loadMagazine(Magazine newMag){
		// By default, set rigidbody to kinematic and no Gravity. Gives magazine to gun.
		// To have a virtual magazine, use VirtualMag.cs
		giveGunMagazine(newMag);
	}
	public virtual void releaseMagazine(){		//WARNING, SHOULD ONLY BE CALLED FROM GUN'S RELEASEMAG method

		//Physics.IgnoreCollision (this.GetComponent<Collider>(),
		//	loadedMagazine.GetComponent<Collider>(),false);
		Debug.Log("Attempting magacceptor releaseMag");
		Attachment attachment = this.GetComponent<AttachmentSlot> ().getAttachment ();
		if (attachment) {
			attachment.GetComponent<Magazine> ().setMag (this.gun.getMag ());
		}
	}
}
