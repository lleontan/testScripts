using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pistolGunHandler : OnShootEvent {
	//Handles the recoil of the reciever for the handgun
	public Gun gun;
	public Transform targetTrans;		//Should be the reciever
	public float recoilDistance= -.1f; //Remember that it should be negative!!!!!


	void Start(){
		if (this.gun == null) {
			this.gun = this.GetComponent<Gun> ();
		}
	}
	public override void OnFire(){
		//When the gun is definitively fired
		if (this.targetTrans != null) {
			//targetRigid.AddForce (targetRigid.transform.forward * this.recoilMagnitude);
			targetTrans.localPosition-=new Vector3(0,0,recoilDistance);
		}
		//this.gun.ChamberRound ();

	}
}
