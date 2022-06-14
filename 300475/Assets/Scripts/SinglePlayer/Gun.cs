using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour {

	public enum GameModeType {
		SinglePlayer,
		Multiplayer
	}

	public GameModeType gameModeType;

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

	[Header("Other Values")]
	public bool useInvoke = false;
	public bool isMelee = false;
	public Vector3 scale = new Vector3(1f, 1f, 1f);
	public Animator anim;

	public float spread;
	public float bulletPerTap;
	public bool allowHoldDown;
	public float shootForce, upwardForce;
	bool isShooting;
	public GameObject muzzleFlash;

	public bool allowInvoke = true;
	public bool readyToShoot;
	public float timeBetweenShoot;

	void Awake(){
		/*
		currentAmmoText.text = currentAmmo.ToString();
		inventoryAmmoText.text = inventoryAmmo.ToString ();
		gunIcon.sprite = gunImage;
		*/
	}

	void OnEnable(){
		if(isMelee)
			anim.SetBool ("isAttacking", false);
	}

	// Use this for initialization
	void Start () {
		if(gameModeType == GameModeType.Multiplayer)
			if(GetComponentInParent<PlayerManager>().health <= 0)
				return;

		if(!isMelee)
			currentAmmo = magazine;

		if(gameModeType == GameModeType.SinglePlayer)
			currentAmmoText.text = currentAmmo.ToString ();

		if(gameModeType == GameModeType.Multiplayer){
			currentAmmoText = GameObject.Find ("CurrentAmmoText").GetComponent<Text> ();
			inventoryAmmoText = GameObject.Find("InventoryAmmoText").GetComponent<Text>();
			gunIcon = GameObject.Find ("WeaponIcon").GetComponent<Image> ();

			currentAmmoText.text = currentAmmo.ToString ();
			inventoryAmmoText.text = inventoryAmmo.ToString ();
		}

		readyToShoot = true;

		if (bulletPrefab == null && !isMelee) {
			Debug.LogError ("Missing Prefab");
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(allowHoldDown) isShooting = Input.GetButton ("Fire1");
		else isShooting = Input.GetButtonDown ("Fire1"); 

		if(isMelee){
			currentAmmoText.text = "--";
			inventoryAmmoText.text = "--";
			
			if(Input.GetButtonDown("Fire1")){
				Melee();
			}
			else if(Input.GetButtonUp("Fire1")){
				anim.SetBool("isAttacking", false);
			}

			// Check current state of the animator
			if(anim.GetBool("isAttacking") && gameModeType == GameModeType.Multiplayer){
				ClientSend.MeleeAttack(transform.position, scale, transform.rotation);
			}
		}

		if (isReloading && !isMelee) {
			transform.Rotate (0, 0, Time.deltaTime * 300, Space.Self);
			return;
		}



		if (isShooting && !useInvoke && Time.time >= nextTimeToFire && !isMelee) {
			nextTimeToFire = Time.time + 1f / fireRate;
			Shoot ();
		}

		if(isShooting && readyToShoot && !isReloading && currentAmmo > 0 && useInvoke && !isMelee){
			Shoot ();
		}

		if (Input.GetKeyDown (KeyCode.R) && currentAmmo < magazine && inventoryAmmo > 0f && !isMelee) {
			StartCoroutine( Reload ());
		}
	}

	void Shoot(){

		if (currentAmmo <= 0 && inventoryAmmo > 0f) {
			StartCoroutine( Reload ());
			return;
		}

		if (inventoryAmmo <= 0)
			inventoryAmmoText.color = Color.red;

		if (currentAmmo <= 0) {
			currentAmmoText.color = Color.red;
			return;
		}

		readyToShoot = false;

		Ray ray = fpsCam.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0));
		RaycastHit _hit;

		Vector3 targetPoint;
		if(Physics.Raycast(ray, out _hit))
			targetPoint = _hit.point;
		else
			targetPoint = ray.GetPoint(75);

		Vector3 directionWithoutSpread = targetPoint - gunPoint.position;

		float x = Random.Range (-spread, spread);
		float y = Random.Range(-spread, spread);

		Vector3 directionWithSpread = directionWithoutSpread + new Vector3 (x, y, 0);

		GameObject bullet = Instantiate (bulletPrefab, gunPoint.position, Quaternion.identity);
		// bullet.transform.localEulerAngles = new Vector3 (90f, 0f, FindObjectOfType<Movement> ().transform.localEulerAngles.y);
		bullet.transform.forward = directionWithSpread.normalized;
		bullet.GetComponent<Rigidbody> ().AddForce (directionWithSpread.normalized * shootForce, ForceMode.Impulse);
		bullet.GetComponent<Rigidbody> ().AddForce (fpsCam.transform.up * upwardForce, ForceMode.Impulse);
		if(gameModeType == GameModeType.Multiplayer)
			ClientSend.PlayerShoot(directionWithSpread.normalized);

		if(muzzleFlash != null)
			Instantiate(muzzleFlash, gunPoint.position, Quaternion.identity);

		currentAmmo--;
		ammoLost++;
		currentAmmoText.text = currentAmmo.ToString();

		//Reset shoot function (if not already invoked)
		if(allowInvoke && useInvoke){
			Invoke ("ResetShoot", timeBetweenShoot);
			allowInvoke = false;
		}

		if(ammoLost < bulletPerTap && currentAmmo > 0 && useInvoke)
			Invoke("Shoot", timeBetweenShoot);

	}

	void ResetShoot(){
		readyToShoot = true;
		allowInvoke = true;
	}

	IEnumerator Reload(){
		Debug.Log ("Reloading...");
		Vector3 rotation = transform.localEulerAngles;
		isReloading = true;

		yield return new WaitForSeconds (reloadTime);

		if (inventoryAmmo <= magazine) {
			currentAmmo = inventoryAmmo;
			inventoryAmmo = 0f;
		} else {
			currentAmmo = magazine;
			inventoryAmmo -= ammoLost;
		}


		ammoLost = 0f;

		currentAmmoText.text = currentAmmo.ToString();
		currentAmmoText.color = Color.white;
		inventoryAmmoText.text = inventoryAmmo.ToString ();

		yield return new WaitForSeconds (0.2f);

		isReloading = false;
		transform.localEulerAngles = rotation;
		Debug.Log ("Finish reloaded");
	}

	void Melee(){
		if(anim != null){
			anim.SetBool("isAttacking", true);
		}

		RaycastHit _hit;
		if(Physics.SphereCast (transform.position, 0.5f, transform.forward, out _hit, 1f)){
			if(_hit.collider.tag == "Enemy"){
				// Enemy Script
			}

			if(gameModeType == GameModeType.Multiplayer){
				// Send the position of weapon to the server
				
			}
		}
	}

	void OnDrawGizmosSelected()
    {
		if(!isMelee)
			return;
        // Display the explosion radius when selected
        Gizmos.color = new Color(1, 1, 0, 0.75F);
        Gizmos.DrawWireCube(transform.position, scale);
    }
}
