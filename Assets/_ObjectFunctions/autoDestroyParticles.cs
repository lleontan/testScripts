using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class autoDestroyParticles : MonoBehaviour {
	private ParticleSystem particles;


	public void Start() 
	{
		particles = GetComponent<ParticleSystem>();
	}

	public void Update() 
	{
		if(particles)
		{
			if(!particles.IsAlive())
			{
				Destroy(this.gameObject);
			}
		}
	}
}
