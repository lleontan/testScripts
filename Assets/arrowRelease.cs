using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(touchHold))]
//On touch hold start activates the arrow, deactivates the mesh renderer of the notch orb.
//On touch hold release, spawn a projectile at this objects position and rotation with a velocity based on the position sliders zValue. Deactivate this object.
public class arrowRelease : touchHoldHandler {
	public GameObject arrowProjectile;				//The projectile that is instantiated on release;
	public MeshRenderer deactiveOnTouchHoldStart;
	public float minRelease;						//If the tracked objects positionslider local z is less than this value then release. 
	public float minForce=.3f;
	public float maxForce=500f;
	public float forceMultiplier=40f;
	public int minDamage=2;
	public int maxDamage=70;
	public float damageMultiplier=30f;
	private touchHold touch;
	public Transform arrow;
	private Quaternion startingLocalRotation;
	public positionSlider slider;
	private Vector3 startingLocalPos;
	public override void onTouchHold(){
		arrow.gameObject.SetActive (true);
		deactiveOnTouchHoldStart.enabled = false;
	}
	public override void onRelease(){
		float zVal = slider.zGet ();
		//Debug.Log(zVal+"--"+minRelease+"--"+(minRelease > zVal));
		if (minRelease > zVal) {
			GameObject projectile = Instantiate (arrowProjectile, this.arrow.position, this.arrow.rotation);
			float baseVal = 1f - zVal;
			projectile.GetComponent<Rigidbody> ().AddForce (-projectile.transform.up*Mathf.Clamp(this.forceMultiplier * baseVal, minForce, maxForce));
			TriggerEnterDamage arrowhead = projectile.GetComponentInChildren<TriggerEnterDamage> ();
			arrowhead.damage = (int)Mathf.Clamp (baseVal * this.damageMultiplier, minDamage, maxDamage);
			arrowhead.damageOwner = this.touch.getLastController ().getControllerEntity ();
		}
		this.transform.localPosition = startingLocalPos;
		deactiveOnTouchHoldStart.enabled = true;
		this.arrow.gameObject.SetActive (false);
	}
	// Use this for initialization
	void Start () {
		startingLocalRotation = this.transform.localRotation;
		touch = this.GetComponent<touchHold> ();
		if (deactiveOnTouchHoldStart == null) {
			deactiveOnTouchHoldStart = this.GetComponent<MeshRenderer> ();
		}
		startingLocalPos = this.transform.localPosition;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (touch.CheckHeld()) {
			//If touch is held then align the arrow towards forward.
		}
	}
}
