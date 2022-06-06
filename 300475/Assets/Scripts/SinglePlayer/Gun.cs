using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour {

	public string weaponName;

	public float damage = 10f;
	public float magazine = 30f;
	public float fireRate = 15f;
	public float inventoryAmmo = 127f;
	private float ammoLost = 0f;
	[HideInInspector] public float currentAmmo;

	public float reloadTime = 3f;

	public Transform gunPoint;
	public Camera fpsCam;

	public GameObject bulletPrefab;

	private float nextTimeToFire = 0f;
	[HideInInspector] public bool isReloading;

	[Header("UI Element")]
	public Text currentAmmoText;
	public Text inventoryAmmoText;
	public Image gunIcon;
	public Sprite gunImage;

	void Awake(){
		currentAmmoText.text = currentAmmo.ToString();
		inventoryAmmoText.text = inventoryAmmo.ToString ();
		gunIcon.sprite = gunImage;
	}

	// Use this for initialization
	void Start () {
		currentAmmo = magazine;
		currentAmmoText.text = currentAmmo.ToString ();

		if (bulletPrefab == null) {
			Debug.LogError ("Missing Prefab");
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (isReloading) {
			transform.Rotate (0, 0, Time.deltaTime * 300, Space.Self);
			return;
		}

		if (Input.GetButton ("Fire1") && Time.time >= nextTimeToFire) {
			nextTimeToFire = Time.time + 1f / fireRate;
			Shoot ();
		}

		if (Input.GetKeyDown (KeyCode.R) && currentAmmo < magazine) {
			StartCoroutine( Reload ());
		}
	}

	void Shoot(){
		Ray ray = fpsCam.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0));
		RaycastHit _hit;

		Vector3 targetPoint;
		if(Physics.Raycast(ray, out _hit))
			targetPoint = _hit.point;
		else
			targetPoint = ray.GetPoint(75);

		Vector3 directionWithoutSpread = targetPoint - gunPoint.position;

		if (currentAmmo <= 0) {
			StartCoroutine( Reload ());
			return;
		}

		GameObject bullet = Instantiate (bulletPrefab, gunPoint.position, Quaternion.identity);
		bullet.transform.forward = directionWithoutSpread.normalized;
		bullet.GetComponent<Rigidbody> ().AddForce (directionWithoutSpread.normalized * 50, ForceMode.Impulse);
		bullet.GetComponent<Rigidbody> ().AddForce (fpsCam.transform.up * 5, ForceMode.Impulse);

		currentAmmo--;
		ammoLost++;
		currentAmmoText.text = currentAmmo.ToString();
	}

	IEnumerator Reload(){
		Debug.Log ("Reloading...");
		Vector3 rotation = transform.localEulerAngles;
		isReloading = true;

		yield return new WaitForSeconds (reloadTime);

		currentAmmo = magazine;
		inventoryAmmo -= ammoLost;
		ammoLost = 0f;
		currentAmmoText.text = currentAmmo.ToString();
		inventoryAmmoText.text = inventoryAmmo.ToString ();

		yield return new WaitForSeconds (0.2f);

		isReloading = false;
		transform.localEulerAngles = rotation;
		Debug.Log ("Finish reloaded");
	}
}
