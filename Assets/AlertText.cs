using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TextMesh))]
public class AlertText : MonoBehaviour {
	//Spawns with message.
	//Lerps upward for n seconds.
	//Self destructs.
	public float duration=2.7f;
	private float elapsed;
	public Vector3 direction = Vector3.up;	//By default goes up 1 meter.
	public float additionalDirectionVariance=.9f;				//added as a % of direction direction
	private Vector3 target;
	void Start () {
		target = this.transform.position + direction*(1f+
			Random.value*additionalDirectionVariance);
		this.elapsed = 0f;
	}
	public void setText(string text){
		this.GetComponent <TextMesh>().text=text;
	}
	// Update is called once per frame
	void Update () {
		if (elapsed > this.duration) {
			Destroy (this.gameObject);
		}
		this.elapsed += Time.deltaTime;
		this.transform.position=Vector3.Lerp(this.transform.position,target,elapsed/duration);
	}
}
