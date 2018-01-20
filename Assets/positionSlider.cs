using System.Collections.Generic;
using UnityEngine;


public class positionSlider : MonoBehaviour {
	/* Clamps objects local position between max and min floats for XYZ axises.
	 * Calls SliderEvent with localPosition magnitude 0.0f being equal to min and 1.0f being max.
	*/
	public Transform child;				// Clamped transform.
	public SliderEvent handler;
	public float maxClampX=0f;
	public float minClampX=0f;
	public float minClampY=0f;
	public float maxClampY=0f;
	public float maxClampZ=.45f;		//By default assumes Z is forward axis
	public float minClampZ = -.45f;
	private bool hasHandler;
	void Start () {
		this.handler = this.GetComponent<SliderEvent> ();
		if (handler) {
			handler.onStart (this);
			hasHandler = true;
		} else {
			hasHandler = false;
		}
		clampPosition ();
	}
	public float zGet(){
		//0f to 1f, magnitude of distance from minimumClamp
		float l=this.maxClampZ-this.minClampZ;
		if(l>0){
			return (child.transform.localPosition.z-this.minClampZ)/l;
		}
		return 0;
	}
	public float yGet(){
		//0f to 1f
		float l=this.maxClampY-this.minClampY;
		if (l > 0) {
			return (child.transform.localPosition.y-this.minClampY) / l;
		}
		return 0;
	}
	public float xGet(){
		//0f to 1f
		float l= this.maxClampX-this.minClampX;
		if (l > 0) {
			return (child.transform.localPosition.x -this.minClampX)/ l;
		}
		return 0;
	}
	public virtual void clampPosition(){
		Vector3 loc = child.transform.localPosition;
		//Debug.Log ("Regular Clamp touchhold "+loc);

		child.transform.localPosition = new Vector3 (
			Mathf.Clamp(loc.x,minClampX,maxClampX),
			Mathf.Clamp(loc.y,minClampY,maxClampY),
			Mathf.Clamp(loc.z,minClampZ,maxClampZ));
		if (hasHandler) {
			this.handler.PositionUpdate (this.xGet (), this.yGet (), this.zGet ());
		}
	}
	public void clampPosition(Vector3 newWorldSpace){
		Vector3 loc = this.transform.InverseTransformPoint (newWorldSpace);
		//Debug.Log ("GlobalClamp touchhold "+loc);

		child.transform.localPosition = new Vector3 (
			Mathf.Clamp(loc.x,minClampX,maxClampX),
			Mathf.Clamp(loc.y,minClampY,maxClampY),
			Mathf.Clamp(loc.z,minClampZ,maxClampZ));
		if (hasHandler) {
			this.handler.PositionUpdate (this.xGet (), this.yGet (), this.zGet ());
		}
	}
	void Update(){
		this.clampPosition ();
	}
}
