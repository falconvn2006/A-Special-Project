using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour {

	public enum GrenadeType {
		Grenade,
		Lethal
	}

	[Header("Pickup Settings")]
	public int amount = 1;

	[Header("Grenade Settings")]
	public GrenadeType type = GrenadeType.Grenade;
	public float damage = 200f;
	public float delay = 3f;
	public GameObject explosionEffect;
	public GameObject smokeEffect;
	public float radius = 5f;
	public float force = 1000f;
	public AudioSource grenadeSound;

	public float smokeLifeTime = 5f;

	float countdown;
	[HideInInspector] public bool isThrown = false;
	bool hasExploded = false;

	// Use this for initialization
	void Start () {
		countdown = delay;
	}
	
	// Update is called once per frame
	void Update () {
		if(type == GrenadeType.Grenade){
			if(isThrown)
				countdown -= Time.deltaTime;

			if(countdown <= 0f && !hasExploded){
				Explode();
				hasExploded = true;
			}
		}
	}

	void Explode(){
		if(type == GrenadeType.Grenade){
			GameObject effect = Instantiate (explosionEffect, transform.position, Quaternion.identity);
			effect.transform.parent = transform;

			Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

			foreach(Collider col in colliders){
				Rigidbody rb = col.GetComponent<Rigidbody>();
				if(rb != null){
					rb.AddExplosionForce(force, transform.position, radius);
				}
			}

			if(grenadeSound != null)
				grenadeSound.Play();

			Destroy(gameObject, 0.5f);
		}
		else {
			GameObject effect = Instantiate (smokeEffect, transform.position, Quaternion.identity);
			effect.transform.parent = transform;
			Destroy(gameObject, smokeLifeTime);
		}
	}

	void OnCollisionEnter(Collision col){
		if(type == GrenadeType.Lethal && isThrown){
			Explode();
		}
	}
}
