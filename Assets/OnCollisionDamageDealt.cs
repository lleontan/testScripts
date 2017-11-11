using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnCollisionDamageDealt : MonoBehaviour {
	//A collisionDamage script dealt damage to something.
	public abstract void onDamagingCollision(Collision col, int damageDealt, CollisionDamage damager);
	public abstract void onDamagingCollision(Collision col, int damageDealt);
}
