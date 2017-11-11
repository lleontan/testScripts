using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualMag : MagAcceptor {
	/*When a magazine is attached destroy it, reveal the virtualMag
	*/
	public GameObject acceptedMagazine;		//The magazine that is ejected when released
	public GameObject visualMag;
	void Start(){
		base.Start ();
	}
	public override void loadMagazine(Magazine magazine){
		this.visualMag.SetActive (true);
		this.gun.LoadMagazine(new mag(magazine.getMag()));
		//Destroy (magazine.gameObject);
	}
	public override void onAttach (Attachment attachment)
	{
		//Only called when a mag is attempted to be loaded through collision
		Magazine mag = attachment.GetComponent<Magazine>();
		if (mag) {
			loadMagazine (mag);
			Destroy (attachment.gameObject);
		}
	}
	public override void releaseMagazine(){
		this.visualMag.SetActive (false);
		GameObject newMag = Instantiate (acceptedMagazine, this.slot.attachmentParentTransform.position,
			this.slot.attachmentParentTransform.rotation);
		Magazine newMagMagazine = newMag.GetComponent<Magazine> ();
		newMagMagazine.setAcceptanceTimer (.7f);
			newMagMagazine.setMag (this.gun.getMag());
	}
}


