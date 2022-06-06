using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour {

	[Header("Pickup Settings")]
	public int amount = 1;

	[Header("Grenade Settings")]
	public float delay = 3f;
	public GameObject explosionEffect;
	public float radius = 5f;
	public float force = 1000f;

	float countdown;
	[HideInInspector] public bool isThrown = false;
	bool hasExploded = false;

	// Use this for initialization
	void Start () {
		countdown = delay;
	}
	
	// Update is called once per frame
	void Update () {
		if(isThrown)
			countdown -= Time.deltaTime;

		if(countdown <= 0f && !hasExploded){
			Explode();
			hasExploded = true;
		}
	}

	void Explode(){
		Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

		foreach(Collider col in colliders){
			Rigidbody rb = col.GetComponent<Rigidbody>();
			if(rb != null){
				rb.AddExplosionForce(force, transform.position, radius);
			}
		}

		Destroy(gameObject);
	}
}
