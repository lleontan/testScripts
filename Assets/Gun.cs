using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {
	// Handles chambering logic for physical bullets,
	// In the future firing logic should be separated out into 2+ handlers for raycasts, projectiles, and other.
	/* firing, chambering logic.
	 * Gets bullets from a magAcceptor
	*/
	public GameObject shell;				//Empty shell
	public GameObject bullet;				//Active projectile
	public GameObject fullBullet;			//Shell with bullet.
	public Transform shootingPoint;			//Point at which bullets are instantiated.
	public Transform shellEjectionPoint;	//Point at which shells are instantiated
	public MagAcceptor magAcceptor;			//Source of bullets

	private int chamberState = 1;			//0 for empty, 1 for active, 2 for used round
	private int magHash = -1;
	public const int EMPTY_ROUND = 2;
	public const int ACTIVE_ROUND = 1;
	public const int CHAMBER_EMPTY = 0;
	private Animator animator;

	private bool hasMag=false;
	private mag magazine;					//The magazine of this gun. Cannot be null.

	public bool autoChamber = false;		//Gun will automatically chamber on equip if chamber empty
	public OnShootEvent gunHandler;
	public ParticleSystem muzzleFlash;
	public int fireType=1;				
	public const int BOLT_ACTION = 0;
	public const int SEMI_AUTO = 1;
	public const int FULL_AUTO = 2;
	public float fireDelay=.01f;
	public float fireDelayCurrent=.11f;

	private Quaternion zeroQuat=Quaternion.Euler(Vector3.zero);
	public mag getMag(){
		return this.magazine;
	}

	public void LoadMagazine(Magazine newMag){
		LoadMagazine (newMag.getMag());
	}
	public void LoadMagazine(mag newMag){
		if (!this.hasMag) {
			this.magazine = newMag;
			Debug.Log (this.name + ":nRounds:" + this.magazine.rounds);
			this.hasMag = true;
		}
	}
	public void releaseMagazine(){
		Debug.Log ("Attempting MagRelease");
		if (this.hasMag) {
			this.magAcceptor.releaseMagazine ();
			this.magazine = new mag (0);
			this.hasMag = false;
		}
	}

	void Start () {
		this.fireDelayCurrent = 0f;
		if (this.muzzleFlash == null && this.shootingPoint != null) {
			this.muzzleFlash = this.shootingPoint.GetComponent<ParticleSystem> ();
		}
		if (this.gunHandler == null) {
			this.gunHandler = this.GetComponent<OnShootEvent> ();
		}

		this.animator = this.GetComponent<Animator> ();
		if (this.magAcceptor) {
			this.magHash = this.magAcceptor.GetSlot().SlotName.GetHashCode();
		}

	}

	public void AutoChamber(){
		//adds a round without chambering the old one
		if(this.autoChamber){
			if (this.chamberState == EMPTY_ROUND) {
				this.ChamberRound ();
			} else if (this.chamberState == CHAMBER_EMPTY) {
				this.chamberState = ACTIVE_ROUND;
			}
		}
	}
	public void FillChamber(){
		//By default fills with activeRounds
		this.chamberState = ACTIVE_ROUND;
	}
	public void FillChamberWithSpecialRounds (int roundType){
		this.chamberState = roundType;
	}
	public void Fire(){
		//Fires gun,
		Instantiate(bullet, shootingPoint.transform.position, shootingPoint.rotation);
		this.muzzleFlash.Play ();
		this.chamberState = EMPTY_ROUND;
		gunHandler.OnFire();

	}
	public void ChamberRound(){
		//Attempts to chambers a round from the magazine
		if (this.chamberState != CHAMBER_EMPTY) {
			EjectRound ();
		}
		if(this.fireType>0 && this.hasMag && this.chamberRound()){
			FillChamber ();
		}
	}
	public void EjectRound(){
		switch(this.chamberState){
			case EMPTY_ROUND:
				Instantiate(shell, shellEjectionPoint.transform.position, shellEjectionPoint.rotation);
				break;
			case ACTIVE_ROUND:
				Instantiate(fullBullet, shellEjectionPoint.transform.position, shellEjectionPoint.rotation);
				break;
		}
		this.chamberState = CHAMBER_EMPTY;
	}
	public bool chamberRound(){
		//Attempts to chambera round;
		//Returns true for successful chambering
		return this.magazine.chamberRound();
	}
	public bool AttemptFire(){
		//Attempts to do a full fire cycle
		//Debug.Log ("checkChamber"+CheckChamber()+"::"+this.fireDelayCurrent);
		bool ret = false;
		if (CheckChamber () && fireDelayCurrent <= 0) {
			Fire ();
			this.fireDelayCurrent = this.fireDelay;
			this.animator.SetTrigger ("startShoot");
			if (this.fireType > 0) {
				ChamberRound ();
			}
			ret = true;
		} else {
			//this.animator.ResetTrigger ("startShoot");
		}
		return ret;
	}
	public void FireOver(){
		
	}
	public bool CheckChamber(){
		return this.chamberState == ACTIVE_ROUND;
	}
 
	void Update(){
		if (this.fireDelayCurrent > 0) {
			this.fireDelayCurrent = fireDelayCurrent- Time.deltaTime;
		} else {
			this.animator.ResetTrigger ("startShoot");
		}
	}

	public bool isRecoiling(){
		//Returns true for is currently recoiling from a shot
		return this.fireDelayCurrent>0;
	}
}
