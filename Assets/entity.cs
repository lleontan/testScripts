using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class entity : MonoBehaviour {
	/* Handles all entity stats
	*/
	public float baseDamageMultiplier = 1f;
	private float damageMultiplier;
	public int maxHealth = 100;
	public int health = 100;		//DO NOT ALTER FROM SCRIPT, FOR EDITOR VIEWING PURPOSES ONLY
	public GameObject healthText;	//Text for healthChanges, like damage
	public GameObject healingText;	//Text for healing

	public GameObject garunteedDeathSpawn;		//If null will not spawn an item or mob
	//protected Rigidbody thisRigid;	//Add rigidbody requirement to another class
	public bool showDamageText=true;
	private GameObject playerGameObj;
	public Transform head;						//Where AI's should point their head
	public string entityName="entity";
	public Transform defaultAlertTextTransform;
	private void Start () {
		//Do not override Start;
		damageMultiplier = baseDamageMultiplier;
		this.playerGameObj=GameObject.Find("Camera (eye)");
		this.checkHealth ();
		if (this.defaultAlertTextTransform==null) {
			this.defaultAlertTextTransform = Instantiate(new GameObject()).transform;
			this.defaultAlertTextTransform.transform.position = this.transform.position + Vector3.up;
			this.defaultAlertTextTransform.parent = this.transform.root;
		}
		if (this.healthText == null) {
			this.healthText = (GameObject)Resources.Load ("damageText");
		}
		if (this.healingText == null) {
			this.healingText = (GameObject)Resources.Load ("healingText");
		}
		//thisRigid = this.gameObject.GetComponent<Rigidbody> ();
		OnSpawn ();
	}

	protected virtual void OnSpawn(){
		//Executed OnSpawn
	}
	public void Heal(int amount){
		Heal (amount, this.defaultAlertTextTransform.position);
	}
	public void Heal(int amount, Vector3 position){
		this.health += amount;
		this.onHeal (amount,position);
		this.checkHealth ();
	}
	public virtual void onHeal(int amount, Vector3 position){
		textAlert(this.healingText, position, amount + " Healed");

	}
	virtual protected void OnDamageTaken(int damage, entity source){
		//When damage is taken override to do an effect.
		//Warning source may be null
	}
	public void TakeDamage(int damage){
		this.TakeDamage (damage,this.transform.position+Vector3.up,"",null);
	}
	public void TakeDamage(int damage, entity source){
		this.TakeDamage (damage, source,this.transform.position+Vector3.up);
	}
	public void TakeDamage(int damage, entity source, Vector3 point){
		if (source == null) {
			this.TakeDamage (damage, point, "", source);
		} else {
			this.TakeDamage (damage, point, source.entityName, source);
		}
	}

	public void TakeDamage(int damage, CollisionDamage source){
		//Damage point is assumed to be this position + 1y
		this.TakeDamage(damage,source,this.transform.position+Vector3.up);
	}
	public void TakeDamage(int damage, CollisionDamage source, Vector3 point){
		//With a given collisionDamage source and a point.
		if (source == null) {
			this.TakeDamage (damage, point, "", null);
		} else {
			TakeDamage (damage, point, source.weaponName, null);
		}
	}
	private void TakeDamage(int damage, Vector3 point, string damageText, entity source){
		// This is the TakeDamage that actually applies damage
		// WARNING source may be null.
		Debug.Log(damage+" taken by "+this.entityName+" from "+damageText);
		textAlert(healthText, point, damage + "taken");
		this.health -= (int)(damage * this.damageMultiplier);
		checkHealth ();
		OnDamageTaken (damage, source);
	}

	public void textAlert (GameObject mesh, Vector3 point, string text){
		Vector3 lookPoint = new Vector3 (this.playerGameObj.transform.position.x,
			point.y,this.playerGameObj.transform.position.z);
		GameObject newText = Instantiate (mesh, point,
			Quaternion.LookRotation(point,lookPoint));
		newText.GetComponent<AlertText> ().setText(text);
	}
	protected void checkHealth(){
		if (this.health < 1) {
			this.OnDeath ();
		} else if (this.health > this.maxHealth) {
			this.health = this.maxHealth;
		}
	}
	protected virtual void OnDeath(){
		//Do Things on Death
		if(garunteedDeathSpawn){
			Instantiate(garunteedDeathSpawn);
		}
		Holdable holdable = this.GetComponent<Holdable> ();
		if (holdable) {
			holdable.releaseAll (true);
		}
		Destroy(this.gameObject);
	}
	void OnCollisionEnter(Collision collision){
		GameObject collidedObj = collision.gameObject;
		/*Rigidbody rigid = collidedObj.GetComponent<Rigidbody> ();
		if (rigid) {
			Vector3 relativeVelocity = this.thisRigid.velocity - rigid.velocity;
			float relMag = relativeVelocity.magnitude;
			if(relMag > 9){
				this.TakeDamage ((int)(.5*Mathf.Pow(relMag,2)*rigid.mass));
			}
		}*/
	}
}
