using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowString : MonoBehaviour {
	//Draws line between 3 points.
	public Transform [] positions;
	public GameObject midPosition;
	private LineRenderer lineRenderer;
	void Start () {
		lineRenderer = this.GetComponent <LineRenderer> ();
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (midPosition.activeSelf) {
			lineRenderer.numPositions=3;
			int count = 0;
			foreach (Transform position in positions) {
				lineRenderer.SetPosition (count, position.position);
				count++;
			}
		} else {
			lineRenderer.numPositions=2;
			lineRenderer.SetPosition (0, positions[0].position);
			lineRenderer.SetPosition (1, positions[2].position);
		}
	}
}
