using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(entity))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CollisionDamage))]
public class SelfDamageOnCollision : OnCollisionDamageDealt {
	//When this collider applies damage make itself take damage.
	private entity thisEntity;
	protected void Start ()
	{
		if (thisEntity == null) {
			thisEntity = this.gameObject.GetComponent<entity> ();
		}
	}
	public override void onDamagingCollision(Collision col, int damageDealt, CollisionDamage damager){
		//WARNING: CAN RETURN A NULL FOR DAMAGER.
		thisEntity.TakeDamage (damageDealt, damager.lastHolder);	
	}
	public override void onDamagingCollision(Collision col, int damageDealt){
		thisEntity.TakeDamage (damageDealt, thisEntity);	
	}
}
