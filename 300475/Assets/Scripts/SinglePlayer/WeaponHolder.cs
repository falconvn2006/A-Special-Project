using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponHolder : MonoBehaviour {

	public LayerMask weaponMask;
	public Transform cameraTrans;

	[Header("Aim Down Sight")]
	public Vector3 aimDownSightPos;
	private Vector3 defaultPos = new Vector3(0.15f, -0.1159999f, 0.266f);
	public bool isAiming;
	public GameObject crossHair;

	[Header("UI")]
	public Image weaponIcon;
	public Image pickUpWeaponIcon;
	public Image pickUpGrenadeIcon;

	public Sprite defaultImage;
	public Text currentAmmoText;
	public Text invetoryAmmoText;
	public GameObject pickupHolder;
	public Text weaponNamePickupText;

	// Image
	public Sprite grenadeIcon;

	public Text grenadeAmmountText;

	[Header("Other values")]
	// Other info
	public int grenadeAmount = 0;
	public float throwForce = 40f;
	public GameObject grenadePrefab;
	public Transform grenadeThrowPos;

	// weapon index
	public int selectedWeapon = 0;

	// transform of the current weapon
	private Transform currentWeapon;

	// Use this for initialization
	void Start () {
		//defaultPos = transform;

		if (grenadeAmount == 0) {
			Color color = grenadeAmmountText.color;
			color.a = 0.5f;
			grenadeAmmountText.color = color;
		} else {
			Color color = grenadeAmmountText.color;
			color.a = 1f;
			grenadeAmmountText.color = color;
		}

		grenadeAmmountText.text = grenadeAmount.ToString ();

		// Set the weapon before start
		SelectWeapon ();
	}
	
	// Update is called once per frame
	void Update () {
		
		int previousSelectedWeapon = selectedWeapon;

		// Check for weapon on the ground
		RaycastHit hit;
		if (Physics.Raycast (cameraTrans.position, -transform.right, out hit, 10f, weaponMask)) {
			if (hit.transform.GetComponent<Gun> () != null) {
				pickupHolder.SetActive (true); // Set the ui
				weaponNamePickupText.text = hit.transform.GetComponent<Gun> ().weaponName;
				pickUpWeaponIcon.sprite = hit.transform.GetComponent<Gun> ().gunImage;

				if (Input.GetKeyDown (KeyCode.F)) {
					// Set the transform to a varible
					Transform pickTrans = hit.transform;
					Vector3 pickPos = hit.transform.position;

					// Turn on kinematic for the pickup weapon on the rigidbody
					pickTrans.GetComponent<Rigidbody> ().isKinematic = true;
					pickTrans.SetParent (transform); // Set the parent to the weapon holder
					pickTrans.position = currentWeapon.position; // Set the position
					pickTrans.rotation = currentWeapon.rotation; // Set the rotation
					currentWeapon.parent = null; // Detach the current weapon from the holder
					currentWeapon.GetComponent<Rigidbody> ().isKinematic = false; // Turn of kinematic for the current weapon
					currentWeapon.position = pickPos;
					pickTrans.GetComponent<Gun> ().enabled = true; // Turn on the weapon script for the pickup weapon
					currentWeapon.GetComponent<Gun> ().enabled = false; // Turn off the current weapon script

					weaponIcon.sprite = pickTrans.GetComponent<Gun> ().gunImage; // Set the icon
					currentAmmoText.text = pickTrans.GetComponent<Gun> ().currentAmmo.ToString (); // Set the ammo
					invetoryAmmoText.text = pickTrans.GetComponent<Gun> ().inventoryAmmo.ToString ();

					currentWeapon = pickTrans; // Set the current weapon
				}
			}

			if (hit.transform.GetComponent<Grenade> () != null) {
				pickupHolder.SetActive (true);
				weaponNamePickupText.text = "Grenade";

				pickUpWeaponIcon.gameObject.SetActive (false);
				pickUpWeaponIcon.sprite = grenadeIcon;
				pickUpGrenadeIcon.gameObject.SetActive (true);

				if (Input.GetKeyDown (KeyCode.F)) {
					grenadeAmount++;
					grenadeAmmountText.text = grenadeAmount.ToString ();

					if (grenadeAmount == 0) {
						Color color = grenadeAmmountText.color;
						color.a = 0.5f;
						grenadeAmmountText.color = color;
					} else {
						Color color = grenadeAmmountText.color;
						color.a = 1f;
						grenadeAmmountText.color = color;
					}

					Destroy (hit.transform.gameObject);
				}
			}

		} else {
			pickupHolder.SetActive (false); // Turn off the pickup icon if the ray didn't hit a weapon
			pickUpWeaponIcon.gameObject.SetActive(true);
			pickUpGrenadeIcon.gameObject.SetActive (false);
		}

		isAiming = Input.GetButton ("Fire2");

		Aim ();
		ThrowGrenade ();

		if (Input.GetAxis ("Mouse ScrollWheel") > 0f) {
			if (selectedWeapon <= 0)
				selectedWeapon = transform.childCount - 1;
			else
				selectedWeapon--;
		}

		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			selectedWeapon = 0;
		}

		if (Input.GetKeyDown (KeyCode.Alpha2) && transform.childCount >= 2) {
			selectedWeapon = 1;
		}

		if (previousSelectedWeapon != selectedWeapon) {
			SelectWeapon ();
		}
	}

	void Aim(){
		if (isAiming) {
			//transform.position = Vector3.Lerp (transform.position, aimDownSightPos, Time.deltaTime * 2);
			transform.localPosition = aimDownSightPos;
			crossHair.SetActive (false);
		} else {
			//transform.position = Vector3.Lerp (aimDownSightPos, defaultPos.position, Time.deltaTime * 2);
			transform.localPosition = defaultPos;
			crossHair.SetActive (true);
		}

		if (transform.childCount <= 0){
			weaponIcon.sprite = defaultImage;
			currentAmmoText.text = "--";
			invetoryAmmoText.text = "--";
		}
	}

	void ThrowGrenade(){
		if (Input.GetKeyDown (KeyCode.G) && grenadeAmount > 0) {

			int defaultLayer = LayerMask.NameToLayer ("Default");

			GameObject grenade = Instantiate (grenadePrefab, grenadeThrowPos.position, Quaternion.identity);
			grenade.layer = defaultLayer;
			grenade.transform.parent = null;
			Rigidbody rb = grenade.GetComponent<Rigidbody> ();
			grenade.GetComponent<Grenade> ().isThrown = true;
			rb.AddForce (grenadeThrowPos.forward * throwForce, ForceMode.VelocityChange);
			grenadeAmount--;
			grenadeAmmountText.text = grenadeAmount.ToString ();

			if (grenadeAmount == 0) {
				Color color = grenadeAmmountText.color;
				color.a = 0.5f;
				grenadeAmmountText.color = color;
			} else {
				Color color = grenadeAmmountText.color;
				color.a = 1f;
				grenadeAmmountText.color = color;
			}
		}
	}

	void SelectWeapon(){

		int i = 0;
		foreach (Transform weapon in transform) {
			if (i == selectedWeapon) {
				weapon.gameObject.SetActive (true);
				weaponIcon.sprite = weapon.GetComponent<Gun> ().gunImage;
				weapon.GetComponent<Gun> ().enabled = true;
				currentAmmoText.text = weapon.GetComponent<Gun> ().currentAmmo.ToString();
				invetoryAmmoText.text = weapon.GetComponent<Gun> ().inventoryAmmo.ToString ();

				RenewText (weapon.GetComponent<Gun> ());

				currentWeapon = weapon;
			} else {
				weapon.gameObject.SetActive (false);
				weapon.GetComponent<Gun> ().enabled = false;
			}
			i++;
		}
	}

	void RenewText(Gun gun){
		if (gun.currentAmmo <= 0) {
			currentAmmoText.color = Color.red;
		} else {
			currentAmmoText.color = Color.white;
		}

		if (gun.inventoryAmmo <= 0) {
			invetoryAmmoText.color = Color.red;
		} else {
			invetoryAmmoText.color = Color.white;
		}
	}
}
