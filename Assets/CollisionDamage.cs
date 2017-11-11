using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CollisionDamage : MonoBehaviour {
	//PAIRS WITH:OnCollisionDamageDealt handler

	//Requires the gameobject this script is attached to to have a rigidbody.
	//Must attach this script to the rigidbody, not the damaging colliders or rootTransform.

	//WARNING: Initialization is slow so always pool if using for a projectile.

	public float velocityMultiplier=7f;		//Each unit of velocity adds this much damage
	public int minDamage= 0;				//Minimum damage on collision
	public int maxDamage= 40;				//Maximum damage from velocity multiplier.
	public string weaponName="Weapon";
	public float minDamageSpeed=0.01f;		//If relative velocity is below this value no damage will be applied.
	private Rigidbody rigid;						//This objects rigidbody
	public bool allCollidersDamage = true;			/*if true then all collisions will cause damage
														if allCollidersDamage is false then iterate though array for damaging colliders.*/
	public Transform[] damagingChildTransforms;				//All children with colliders of these transforms are damagingColliders.
	private Collider[] damagingColliders;					//The damaging colliders, taken from damagingChildTransforms

	public entity lastHolder;					//Who was the last entity to wield this object?
	public float damageBreakDuration = .5f;		//Deactivates colliders for this duration once a successful hit is made.
	private float currentDamageBreakTimer=0;	//pairs with damageBreakDuration
	public bool canHit = true;					//If true can damage.
	public string[] ignoreDamageLayers;			//Does not damage these layers.
	private int[] ignoreDamageLayerInts;		//pairs with ignoreDamageLayers
	private OnCollisionDamageDealt collisionDamageHandler;
	protected virtual void Start(){
		collisionDamageHandler = this.GetComponent<OnCollisionDamageDealt> ();
		int layers = ignoreDamageLayers.Length;
		this.ignoreDamageLayerInts = new int[layers];

		for (int a =0;a<layers;a++) {
			this.ignoreDamageLayerInts [a] = LayerMask.NameToLayer (ignoreDamageLayers [a]);
		}
		this.currentDamageBreakTimer = damageBreakDuration;
		rigid = this.transform.GetComponent<Rigidbody> ();
		setDamagingColliders ();
	}
	private Stack<Collider> setDamagingCollidersTraversal(Transform target, Stack<Collider>colliders){
		//returns a stack of all colliders contained by this transform and its children.
		Collider thisCollider=this.GetComponent<Collider>();
		if (thisCollider) {
			colliders.Push (thisCollider);
		}
		foreach (Transform child in this.transform) {
			colliders=setDamagingCollidersTraversal(child,colliders);
		}
		return colliders;

	}
	public void setDamagingColliders(){
		//Adds all children of the specified to damagingColliders.
		Stack<Collider> colliderCollection=new Stack<Collider>();
		foreach(Transform targetTrans in damagingChildTransforms){
			setDamagingCollidersTraversal (targetTrans,colliderCollection);		//Pass by reference
		}
		this.damagingColliders= colliderCollection.ToArray ();
	}

	public void OnPickup(entity newHolder){
		//When a entity picks up this object set lastholder to them.
		this.lastHolder=newHolder;
	}

	//Prioritises damaging limbs over the enemy itself
	void OnCollisionEnter(Collision collision){
		GameObject collidedObj = collision.gameObject;
		entity target = collidedObj.GetComponent<entity> ();
		foreach(int layer in this.ignoreDamageLayerInts){
			if (collidedObj.layer == layer) {
				return;
			}
		}
		if (target) {
			if (allCollidersDamage) {
				applyDamage (target, collision);
			}
			else {
				//Search through contact points. if contact point is in damaging colliders do damage
				foreach(ContactPoint contact in collision.contacts){
					if (checkDamageSource (contact.thisCollider)) {
						applyDamage (target,collision);
						break;
					}
				}
			}
		}
	}
	private bool checkDamageSource(Collider checkCol){
		//Checks this objects damaging colliders and returns true if the collider is on the list.
		foreach(Collider col in this.damagingColliders){
			if (checkCol == col) {
				return true;
			}
		}
		return false;
	}
	private void applyDamage(entity target, Collision collision){
		//Checks for limbHitbox on collider.
		//Applies damage to a entity, gives lastHolder if it exists otherwise credits itself for damage.
		currentDamageBreakTimer = this.damageBreakDuration;
		Vector3 rigidVel = target.GetComponent<Rigidbody> ().velocity;
		limbHitbox limb = collision.collider.gameObject.GetComponent<limbHitbox> ();
		int calculatedDamage = calculateDamage (rigidVel);
		if (limb) {
			canHit = false;
			limb.takeDamage (calculatedDamage,
				this, collision.contacts [0].point);
		} else if (target) {
			this.canHit = false;
			foreach (Collider col in this.damagingColliders) {
				col.isTrigger = true;
			}
			if (lastHolder) {
				target.TakeDamage (calculatedDamage,
					lastHolder, collision.contacts [0].point);
			} else {
				target.TakeDamage (calculatedDamage,
					this, collision.contacts [0].point);
			}
		}
		if (this.collisionDamageHandler) {
			collisionDamageHandler.onDamagingCollision (collision, calculatedDamage,this);
		}
	}
	void Update(){
		if (!canHit) {
			this.currentDamageBreakTimer -= Time.deltaTime;
			if (currentDamageBreakTimer < 0) {
				canHit = true;
				currentDamageBreakTimer = this.damageBreakDuration;
				foreach (Collider col in this.damagingColliders) {
					col.isTrigger = false;
				}
			}
		}
	}
	private int calculateDamage(Vector3 vel){
		Vector3 relativeVelocity = this.rigid.velocity - vel;
		float relMag = relativeVelocity.magnitude;
		if (relMag > this.minDamageSpeed) {
			return (int)Mathf.Min (relMag * velocityMultiplier + minDamage, maxDamage);
		} else{
			return 0;
		}
	}
}
