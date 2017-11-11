using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGrenadePinPull : ParentDistanceBreakEvent {
	// When the ParentDistanceBreak calls this script, blows up the grenade.
	// The owner of the grenade when it detonates is firstly the entity that pulled the pin and
	// secondly the entity that was last holding the grenade if the pin was not held when it was pulled.
	public float explosionForce=10f;
	public float explosionRadius=6f;
	public float upwardsModifier=.1f;
	public string []layers={"Default","player"};
	public GameObject explosionParticles;
	public float explosionDamageMin=5f;
	public float explosionDamageMax=10f;	//Linearly scaled
	private float damageScaleFactor;
	public touchHold pin;				//The triggering pin.
	private entity lastUser;			//The last user to hold this grenade
	private bool explosionTimerActive=false;
	private float currentExplosionTime;
	public float explosionTimer=3f;
	public ParticleSystem pinPullParticles;


	void Start(){
		if (explosionTimerActive) {
			pinPullParticles.Play ();
		} else {
			this.pinPullParticles.Stop ();
		}
		this.currentExplosionTime = explosionTimer;
		if (explosionDamageMax < explosionDamageMin) {
			float temp = explosionDamageMin;
			explosionDamageMin = explosionDamageMax;
			explosionDamageMax = temp;
		}
		this.damageScaleFactor = this.explosionDamageMax - explosionDamageMin;
	}
	public void setLastUser(entity newUser){
		this.lastUser = newUser;
	}
	void Update(){
		if (explosionTimerActive) {
			this.currentExplosionTime -= Time.deltaTime;
			if (currentExplosionTime < 0) {
				explode ();
			}
		}
	}
	public override void OnDistanceBreak(touchHold held){
		explosionTimerActive = true;
		pinPullParticles.Play();
		Rigidbody pinRigid = pin.GetComponent<Rigidbody> ();
		pinRigid.useGravity = true;
		pinRigid.isKinematic=false;
	
		pin.GetComponent<DelayDestroy> ().timerActive = true;
		Controller control = pin.getLastController ();
		if(control){
			control.getControllerEntity ();
		}
	}
	private void explode(){
		if (lastUser == null) {
			this.lastUser = this.GetComponent<Holdable> ().getLastHolder ();
		}
		Collider[] collidersInRange = Physics.OverlapSphere(this.transform.position, explosionRadius, 
			LayerMask.GetMask (layers), QueryTriggerInteraction.Ignore);
		HashSet <Transform>objectsInRange=new HashSet<Transform>();
		foreach (Collider col in collidersInRange) {
			objectsInRange.Add (col.transform.root);
		}
		foreach (Transform trans in objectsInRange) {
			Rigidbody rigid = trans.GetComponent<Rigidbody> ();
			if (rigid) {
				rigid.AddExplosionForce (explosionForce, this.transform.position, explosionRadius, upwardsModifier, ForceMode.Force);
				entity obj = rigid.GetComponent<entity> ();
				if (obj) {
					float distance = Vector3.Distance (this.transform.position, trans.transform.position);
					int damage = 
						Mathf.RoundToInt (this.explosionDamageMin + distance /
							explosionRadius * damageScaleFactor);
					obj.TakeDamage (damage,lastUser);
				}
			}
		}
		Instantiate (explosionParticles,this.transform.transform.position,this.transform.rotation);
		Destroy(gameObject);
	}
}
