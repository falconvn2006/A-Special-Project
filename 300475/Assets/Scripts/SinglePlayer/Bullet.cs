using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	public GameObject impactEffect;

	public float speed = 10f;

	private Rigidbody rb;
	private float timer = 0f;

	public Vector3 direction;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();

	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;

		if (timer >= 30f) {
			Destroy (gameObject);
		}
	}

	void OnCollisionEnter(Collision col){
		GameObject effect = Instantiate (impactEffect, transform.position, Quaternion.LookRotation(transform.forward));

		Destroy(effect, 1f);

		Destroy (gameObject, 1.5f);
	}
}
