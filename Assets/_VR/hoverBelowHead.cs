using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hoverBelowHead : MonoBehaviour {
	//Has body face head direction but hover below it.
	public Transform head;
	public Vector3 offset = new Vector3 (0,-.3f, .2f);
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.position = head.transform.position + offset;
		this.transform.rotation = Quaternion.Euler (0, head.transform.rotation.eulerAngles.y, 0);
	}
}
