using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof(Mob))]
[RequireComponent (typeof(entity))]
[RequireComponent (typeof(NavMeshAgent))]
[RequireComponent (typeof(Animator))]
public class GunmanAI : MobHandler
{
	// Picks up guns and shoots hostile mobs.
	// Looks at targeted enemy or nearest seen mob within its sightline unless fleeing.
	// if fleeing look forward
	// point body towards target if not fleeing otherwise point forward.

	private Rigidbody rigid;
	//This objects rigidbody
	private Animator animator;
	private Transform head;
	private Transform body;
	public Mob lookTarget;
	//The mob of whatever entity we're looking at.
	private Transform lookTargetHead;
	//Where the head looks
	private Transform lookTargetBody;
	//Where the torso looks
	private NavMeshAgent navAgent;
	//this object navAgent
	private Mob mob;
	//This objects mob
	public Gun gun;
	public float aquizitionLossDistance = 7f;
	//Distance at which the target is set to null
	public float destinationReachedSpace = 1f;
	//Amount of space between us and target position
	public float firerate = 1.5f;
	//time between shots.
	private float fireTimer;
	public bool canFire = true;
	public Transform hand;
	//hand with gun
	public float fireDistance = 4f;
	private Transform gunTrans;
	//The transform of this mobs gun gun
	public bool ikActive = true;
	// Use this for initialization
	void Start ()
	{
		hand.GetComponent<Hand> ().PickUpObjectHandle (gun.GetComponent<Holdable> ().GetDefaultHandle ());
		fireTimer = firerate;
		rigid = this.GetComponent<Rigidbody> ();
		this.animator = this.GetComponent<Animator> ();
		this.mob = this.GetComponent<Mob> ();
		navAgent = this.GetComponent<NavMeshAgent> ();

		this.body = animator.GetBoneTransform (HumanBodyBones.Chest);
		this.head = animator.GetBoneTransform (HumanBodyBones.Head);

	}

	public void attemptFire ()
	{
		//AI tries to fire a gun
		if (gun.AttemptFire ()) {
			this.canFire = false;
		} else {
			gun.AutoChamber ();
		}
	}
	// rotate towards look target.
	void Update ()
	{
		if (lookTarget != null) {
			if (!canFire && fireTimer > 0) {	//If cannot fire and on Cooldown.
				fireTimer -= Time.deltaTime;	//Decrement cooldown.
			} else {
				fireTimer = firerate;					//If we can fire or the firetimer is not on CD.
				animator.SetBool ("IsShooting", true);
				attemptFire ();
			}
			if (canFire && Vector3.Distance (lookTarget.transform.position, this.transform.position)
			    < this.fireDistance) {

			} else {
				//this.animator.ResetTrigger ("isShooting");
			}
			navAgent.destination = lookTarget.transform.position;

			if (this.lookTargetBody != null) {
				Vector3 lookatPos = 
					new Vector3 (lookTargetBody.position.x, this.transform.position.y,
						lookTargetBody.position.z);
				this.transform.LookAt (lookatPos);


			} else if (this.lookTargetHead != null) {
				this.head.LookAt (lookTargetHead);
			}
			if (!navAgent.pathPending) {
				/*Vector3.Magnitude (lookTarget.transform.position - this.transform.position)>
				aquizitionLossDistance*/
				if (navAgent.remainingDistance <= navAgent.stoppingDistance) {
					if (!navAgent.hasPath || navAgent.velocity.sqrMagnitude == 0f) {
						animator.SetBool ("isWalking", false);
		
					} else {
						animator.SetBool ("isWalking", true);
					}
				} else if (Vector3.Magnitude (lookTarget.transform.position - this.transform.position) >
				           aquizitionLossDistance) {
					OnTargetLost ();
				}
			}
		} else {
			animator.SetBool ("IsShooting", false);

		}
	}
	//a callback for calculating IK
	/*void LateUpdate ()
	{
		//if the IK is active, set the position and rotation directly to the goal. 
		if (ikActive) {

			// Set the look target position, if one has been assigned
			if (this.lookTarget != null) {
				animator.SetLookAtWeight (1);
				animator.SetLookAtPosition (this.lookTargetHead.position);

			}    

			// Set the right hand target position and rotation, if one has been assigned
			if (hand != null && lookTargetHead != null) {
				Debug.Log ("Look Target Head "+lookTargetHead.name);
				animator.SetIKPositionWeight (AvatarIKGoal.LeftHand, 1);
				animator.SetIKRotationWeight (AvatarIKGoal.LeftHand, 1);  
				animator.SetIKPosition (AvatarIKGoal.LeftHand, lookTargetHead.position);
				animator.SetIKRotation (AvatarIKGoal.LeftHand, lookTargetHead.rotation);
			}        

		} else {    
			//if the IK is not active, set the position and rotation of the hand 
			//and head back to the original position

			animator.SetIKPositionWeight (AvatarIKGoal.LeftHand, 0);
			animator.SetIKRotationWeight (AvatarIKGoal.LeftHand, 0); 
			animator.SetLookAtWeight (0);
		}
	}*/
	void OnAnimatorIK(int layer)
	{
		//if the IK is active, set the position and rotation directly to the goal. 
		if (ikActive) {
			// Set the look target position, if one has been assigned
			if (this.lookTarget != null) {
				animator.SetLookAtWeight (1);
				animator.SetLookAtPosition (this.lookTargetHead.position);
			}    
			// Set the right hand target position and rotation, if one has been assigned
			if (hand != null && lookTargetHead != null) {
				animator.SetIKPositionWeight (AvatarIKGoal.RightHand, 1);
				animator.SetIKRotationWeight (AvatarIKGoal.RightHand, 0);  
				animator.SetIKPosition (AvatarIKGoal.RightHand, lookTargetHead.position);
				animator.SetIKRotation (AvatarIKGoal.RightHand, lookTargetHead.rotation);
			}        

		} else {    
			//if the IK is not active, set the position and rotation of the hand and head back to the original position
			animator.SetIKPositionWeight (AvatarIKGoal.RightHand, 0);
			animator.SetIKRotationWeight (AvatarIKGoal.RightHand, 0); 
			animator.SetLookAtWeight (0);
		}
	}

	public override void OnEnemySighted (Mob otherObj)
	{
		Debug.Log ("TargetSighted");
		this.lookTarget = otherObj;
		lookTargetBody = otherObj.transform;
		lookTargetHead = otherObj.getHeadTransform ();		//Warning, lookTargetHead may be null.
		navAgent.destination = otherObj.body.position;
		animator.SetTrigger ("isWalking");

	}

	public void OnTargetLost ()
	{
		Debug.Log ("TargetLost");
		navAgent.destination = this.transform.position;
		this.lookTarget = null;
		this.lookTargetBody = null;
		this.lookTargetHead = null;

		//navAgent.Stop ();
		animator.ResetTrigger ("isWalking");

	}

	void OnTriggerEnter (Collider col)
	{
		if (this.lookTarget == null) {
			Mob otherObj = col.transform.root.GetComponent<Mob> ();
			if (otherObj && mob.checkHostility (otherObj)) {
				//If the other object is an entity and this mob is hostile to it set it as the target.
				OnEnemySighted (otherObj);
			}
		}
	}
}
