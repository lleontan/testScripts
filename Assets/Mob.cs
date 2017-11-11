using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(entity))]
public class Mob : MonoBehaviour {
	// Handles walking and stunStates and aggro.
	//	private EntityHandler entityEvents;

	public float maxMoveSpeed = 40f;		//Maximum movespeed with modifiers.
	public float baseMoveSpeed = 3f;			//Base movespeed
	protected float moveSpeed;			//Current moveSpeed with modifiers.
	protected bool notStunned = true;			//If the mob is stunned
	protected bool canMove= true;			//If the mob is permitted to walk or run.

	//Testing out bodyHover, may reimplement in Mob is unsuccessful.
	//public float lifterHeight= .3f;			//Height of lifter

	protected Rigidbody thisRigid;
	private MobHandler mobHandler;
	public int [] hostileTo;
	private entity mobEntity;
	public int team;
	public Transform body;					//Where do AI's point there body to look at this mob?
	public Transform aiHeadLookTrans;		//Where do AI's point their heads when they see this mob
	public entity getEntity(){
		return this.mobEntity ;
	}
	void Start(){
		if (body == null) {
			this.body = this.transform;
		}
		if (aiHeadLookTrans == null) {
			this.aiHeadLookTrans = this.transform;
		}
		this.mobHandler=this.GetComponent<MobHandler>();
		this.mobEntity = this.GetComponent<entity> ();
		thisRigid = this.gameObject.GetComponent<Rigidbody> ();
		this.moveSpeed = this.baseMoveSpeed;
	}
	public Transform getHeadTransform(){
		//Returns where a onlooker should point their head to look at this object
		// Defaults to this.transform.
		return this.aiHeadLookTrans;
	}

	public bool checkHostility(Mob other){
		//True for hostile
		int teamNumber = other.team;
		foreach (int i in hostileTo) {
			if (i == teamNumber) {
				return true;
			}
		}
		return false;
	}
	void Update () {
		//If the mob is capable of controlling movement limit its speed to its walkspeed.
		if (canMove && this.notStunned) {
			this.thisRigid.velocity=
				this.thisRigid.velocity.normalized
				* Mathf.Clamp(this.thisRigid.velocity.magnitude,0f,this.maxMoveSpeed);
		}

	}
	public void Move2d(Vector3 direction){
		//In Absolute XZ * moveSpeed;
		if (this.canMove && this.notStunned) {
			thisRigid.AddForce (direction*this.moveSpeed);
		}
	}

	public void setCanMove(bool setTo){
		this.canMove = setTo;
	}
	public bool getCanMove(){
		return notStunned && canMove;
	}
	public void xZVelocityReset(){
		this.thisRigid.velocity = new Vector3 (0,this.thisRigid.velocity.y,0);
	}
}