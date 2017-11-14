using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {
	//When told to open move to open localposition.
	//When told to close, move to close localposition.
	//Close position defaults to position on start.

	public Vector3 openLocal;
	private Vector3 closeLocal;
	public float duration=.5f;
	private float currentDuration;
	private Vector3 localLerpTarget;
	private float distance;

	public bool openOnStart=false;
	// Use this for initialization
	void Start () {
		this.currentDuration = 1;
		this.closeLocal = this.transform.localPosition;
		this.localLerpTarget = this.closeLocal;
		this.distance = (closeLocal - openLocal).magnitude;
		if (openOnStart) {
			ToggleOpenState ();
		}
	}
	public void ToggleOpenState(){
		//Toggles between open and close State.
		//If localLerpTarget is anything but fully closed it counts as open.
		if (localLerpTarget != closeLocal) {
			Close ();
		} else {
			Open ();
		}
	}
	public void Open(){
		//From a closed state to a open state
		this.currentDuration = (this.transform.localPosition-this.closeLocal).normalized.magnitude;
		this.localLerpTarget = this.openLocal;
	}
	public void Close(){
		//From a open state to a close state.
		this.currentDuration = (this.transform.localPosition-this.openLocal).normalized.magnitude;
		this.localLerpTarget = this.closeLocal;
	}
	// Update is called once per frame
	void Update () {
		if (currentDuration < 1) {
			this.transform.localPosition = Vector3.Lerp (this.transform.localPosition, localLerpTarget, currentDuration += Time.deltaTime / duration);
		}
	}
}
