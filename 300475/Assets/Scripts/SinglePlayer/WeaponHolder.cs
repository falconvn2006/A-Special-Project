using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponHolder : MonoBehaviour {

	public LayerMask weaponMask;

	[Header("Aim Down Sight")]
	public Vector3 aimDownSightPos;
	private Vector3 defaultPos = new Vector3(0.15f, -0.1159999f, 0.266f);
	public bool isAiming;

	[Header("UI")]
	public Image weaponIcon;
	public Sprite defaultImage;
	public Text currentAmmoText;
	public Text invetoryAmmoText;
	public GameObject pickupHolder;
	public Text weaponNamePickupText;

	// weapon index
	public int selectedWeapon = 0;

	// transform of the current weapon
	private Transform currentWeapon;

	// Use this for initialization
	void Start () {
		//defaultPos = transform;

		// Set the weapon before start
		SelectWeapon ();
	}
	
	// Update is called once per frame
	void Update () {
		
		int previousSelectedWeapon = selectedWeapon;

		// Check for weapon on the ground
		RaycastHit hit;
		if (Physics.Raycast (transform.position, -transform.right, out hit, 10f, weaponMask)) {
			pickupHolder.SetActive (true); // Set the ui
			weaponNamePickupText.text = hit.transform.GetComponent<Gun> ().weaponName;
			if (Input.GetKeyDown (KeyCode.F)) {
				// Set the transform to a varible
				Transform pickTrans = hit.transform;

				// Turn on kinematic for the pickup weapon on the rigidbody
				pickTrans.GetComponent<Rigidbody> ().isKinematic = true;
				pickTrans.position = currentWeapon.position; // Set the position
				pickTrans.rotation = currentWeapon.rotation; // Set the rotation
				pickTrans.SetParent (transform); // Set the parent to the weapon holder
				currentWeapon.parent = null; // Detach the current weapon from the holder
				currentWeapon.GetComponent<Rigidbody> ().isKinematic = false; // Turn of kinematic for the current weapon
				pickTrans.GetComponent<Gun> ().enabled = true; // Turn on the weapon script for the pickup weapon
				currentWeapon.GetComponent<Gun> ().enabled = false; // Turn off the current weapon script

				weaponIcon.sprite = pickTrans.GetComponent<Gun> ().gunImage; // Set the icon
				currentAmmoText.text = pickTrans.GetComponent<Gun> ().currentAmmo.ToString (); // Set the ammo
				invetoryAmmoText.text = pickTrans.GetComponent<Gun>().inventoryAmmo.ToString();

				currentWeapon = pickTrans; // Set the current weapon
			}
		} else {
			pickupHolder.SetActive (false); // Turn off the pickup icon if the ray didn't hit a weapon
		}

		isAiming = Input.GetButton ("Fire2");
		Aim ();

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
		if (isAiming)
			//transform.position = Vector3.Lerp (transform.position, aimDownSightPos, Time.deltaTime * 2);
			transform.localPosition = aimDownSightPos;
		else
			//transform.position = Vector3.Lerp (aimDownSightPos, defaultPos.position, Time.deltaTime * 2);
			transform.localPosition = defaultPos;

		if (transform.childCount <= 0){
			weaponIcon.sprite = defaultImage;
			currentAmmoText.text = "--";
			invetoryAmmoText.text = "--";
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

				currentWeapon = weapon;
			} else {
				weapon.gameObject.SetActive (false);
				weapon.GetComponent<Gun> ().enabled = false;
			}
			i++;
		}
	}
}
