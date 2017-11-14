using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magazine : OnAttach {
	/* Warning, showrounds must be inputed to the showRounds array in editor from the top of the mag to the bottom.
	*/

	private int magHash=-1;
	private mag magazine;

	public int maxRounds = 30;
	public int rounds = 30;
	public GameObject[] showRounds;		//Round that is shown in mag
	public bool onlyFirstRoundShow=false;					//Only show the first round in show rounds?
	public string onAcceptLayer="ignoreDefault";
	private int onAcceptLayerInt;
	private int defaultLayer;
	public float acceptanceTimer=.5f;
	public mag getMag(){
		if(magazine==null){
			magazine = new mag (rounds,maxRounds);
		}
		return this.magazine;
	}
	public void setMag(mag newMag){
		this.magazine = new mag(newMag);
		checkBullets();
	}
	public void checkBullets(){
		//Deactivates rounds starting from bottom up
		//The last round to be deactivated is in array index 0 of showRounds;
		if (this.onlyFirstRoundShow) {
			if (this.magazine.rounds < 1) {
				showRounds [0].SetActive (false);
			}
		} else if (this.magazine.rounds < showRounds.Length) {
			int rounds = this.magazine.rounds;
			for (int a = showRounds.Length - 1; a >= rounds; a--) {
				this.showRounds [a].SetActive (false);
			}
		}
	}

	void Update(){
		if (acceptanceTimer > 0) {
			acceptanceTimer -= Time.deltaTime;
		}
	}
	public bool checkLoadable(){
		return this.acceptanceTimer < 0;
	}
	public void setAcceptanceTimer(float time){
		this.GetComponent<Attachment> ().setCanAttach (time);
	}
	void Start(){
		if (magazine == null){
			magazine = new mag (rounds, maxRounds);
		}
		onAcceptLayerInt = LayerMask.NameToLayer (this.onAcceptLayer);
		defaultLayer = this.gameObject.layer;
	}
	private void recursiveChildLayerAssignment(Transform obj, int layer){
		foreach (Transform child in obj) {
			child.gameObject.layer = layer;
		}
	}
	public void changeAllLayers(){
		this.gameObject.layer = onAcceptLayerInt;
		this.recursiveChildLayerAssignment (this.transform,this.onAcceptLayerInt);	
	}
	public void setToDefaultLayer(){
		this.gameObject.layer = defaultLayer;
		this.recursiveChildLayerAssignment (this.transform,this.defaultLayer);	
	}
	public int getMagHash(){
		return this.magHash;
	}

	public void releaseHold(){
		this.GetComponent<touchHold> ().releaseControllerGrip();
	}

}
