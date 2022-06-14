using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponHolder : MonoBehaviour {

	public enum GameModeType {
		SinglePlayer,
		Multiplayer
	}

	public GameModeType gameModeType;

	public LayerMask weaponMask;
	public Transform cameraTrans;

	[Header("Aim Down Sight")]
	public Camera mainCam;
	public float aimFOV = 30f;
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
	public Sprite lethalIcon;

	public Text grenadeAmmountText;
	public Text lethalAmmountText;

	[Header("Other values")]
	// Other info
	public int grenadeAmount = 0;
	public int lethalAmmount = 0;
	public float throwForce = 40f;
	public GameObject grenadePrefab;
	public GameObject lethalPrefab;
	public Transform grenadeThrowPos;

	// weapon index
	public int selectedWeapon = 0;

	// transform of the current weapon
	private Transform currentWeapon;

	private float defaultFOV;

	// Use this for initialization
	void Start () {
		defaultFOV = mainCam.fieldOfView;
		if(gameModeType == GameModeType.Multiplayer)
			defaultPos = transform.localPosition;
			

		if(gameModeType == GameModeType.SinglePlayer){
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

			if(lethalAmmount == 0) {
				Color color = lethalAmmountText.color;
				color.a = 0.5f;
				lethalAmmountText.color = color;
			} else {
				Color color = lethalAmmountText.color;
				color.a = 1f;
				lethalAmmountText.color = color;
			}

			grenadeAmmountText.text = grenadeAmount.ToString ();
			lethalAmmountText.text = lethalAmmount.ToString ();

		}

		if(gameModeType == GameModeType.Multiplayer){
			crossHair = GameObject.Find ("CrossHair");
			currentAmmoText = GameObject.Find ("CurrentAmmoText").GetComponent<Text> ();
			invetoryAmmoText = GameObject.Find("InventoryAmmoText").GetComponent<Text>();
			weaponIcon = GameObject.Find ("WeaponIcon").GetComponent<Image> ();
		}

		// Set the weapon before start
		SelectWeapon ();
	}
	
	// Update is called once per frame
	void Update () {
		
		int previousSelectedWeapon = selectedWeapon;

		if(gameModeType == GameModeType.SinglePlayer){
			// Picking up a weapon
			// Check for weapon on the ground
			RaycastHit hit;
			if (Physics.Raycast (cameraTrans.position, -transform.right, out hit, 10f, weaponMask)) {
				if (hit.transform.GetComponent<Gun> () != null) {
					if(hit.transform.GetComponent<Gun>().isMelee)
						return;

					pickupHolder.SetActive (true); // Set the ui
					weaponNamePickupText.text = hit.transform.GetComponent<Gun> ().weaponName;
					pickUpWeaponIcon.sprite = hit.transform.GetComponent<Gun> ().gunImage;

					if (Input.GetKeyDown (KeyCode.F)) {
						// Set the transform to a varible
						Transform pickTrans = hit.transform;
						Vector3 pickPos = hit.transform.position;

						// Get the layer
						int picklayer = pickTrans.gameObject.layer;
						int currentlayer = currentWeapon.gameObject.layer;

						// Turn on kinematic for the pickup weapon on the rigidbody
						pickTrans.GetComponent<Rigidbody> ().isKinematic = true;
						pickTrans.GetComponent<Collider> ().isTrigger = true; // Turn on trigger for the pickup weapon collider
						pickTrans.SetParent (transform); // Set the parent to the weapon holder
						pickTrans.position = currentWeapon.position; // Set the position
						pickTrans.rotation = currentWeapon.rotation; // Set the rotation
						currentWeapon.parent = null; // Detach the current weapon from the holder
						currentWeapon.GetComponent<Rigidbody> ().isKinematic = false; // Turn off kinematic for the current weapon
						currentWeapon.GetComponent<Collider>().isTrigger = false; // Turn off trigger for the current weapon collider
						currentWeapon.position = pickPos;
						pickTrans.GetComponent<Gun> ().enabled = true; // Turn on the weapon script for the pickup weapon
						currentWeapon.GetComponent<Gun> ().enabled = false; // Turn off the current weapon script

						pickTrans.gameObject.layer = currentlayer; // Set the layer
						currentWeapon.gameObject.layer = picklayer; // Set the layer

						weaponIcon.sprite = pickTrans.GetComponent<Gun> ().gunImage; // Set the icon
						currentAmmoText.text = pickTrans.GetComponent<Gun> ().currentAmmo.ToString (); // Set the ammo
						invetoryAmmoText.text = pickTrans.GetComponent<Gun> ().inventoryAmmo.ToString ();

						currentWeapon = pickTrans; // Set the current weapon
					}
				}

				if (hit.transform.GetComponent<Grenade> () != null && hit.transform.GetComponent<Grenade>().type == Grenade.GrenadeType.Grenade) {
					pickupHolder.SetActive (true);
					weaponNamePickupText.text = "Grenade";

					pickUpWeaponIcon.gameObject.SetActive (false);
					pickUpWeaponIcon.sprite = grenadeIcon;
					pickUpGrenadeIcon.gameObject.SetActive (true);
					pickUpGrenadeIcon.sprite = grenadeIcon;

					if (Input.GetKeyDown (KeyCode.F)) {
						grenadeAmount += hit.transform.GetComponent<Grenade> ().amount;;
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
				else if (hit.transform.GetComponent<Grenade> () != null && hit.transform.GetComponent<Grenade> ().type == Grenade.GrenadeType.Lethal) {
						pickupHolder.SetActive (true);
						weaponNamePickupText.text = "Smoke Grenade";

						pickUpWeaponIcon.gameObject.SetActive (false);
						pickUpWeaponIcon.sprite = grenadeIcon;
						pickUpGrenadeIcon.gameObject.SetActive (true);
						pickUpGrenadeIcon.sprite = lethalIcon;

						if (Input.GetKeyDown (KeyCode.F)) {
							lethalAmmount += hit.transform.GetComponent<Grenade> ().amount;
							lethalAmmountText.text = lethalAmmount.ToString ();

							if (lethalAmmount == 0) {
								Color color = lethalAmmountText.color;
								color.a = 0.5f;
								lethalAmmountText.color = color;
							} else {
								Color color = lethalAmmountText.color;
								color.a = 1f;
								lethalAmmountText.color = color;
							}
							Destroy (hit.transform.gameObject);
						}
					}
			}
			else {
				pickupHolder.SetActive (false); // Turn off the pickup icon if the ray didn't hit a weapon
				pickUpWeaponIcon.gameObject.SetActive(true);
				pickUpGrenadeIcon.gameObject.SetActive (false);
			}


			ThrowGrenade ();
			ThrowLethal();

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

		if (Input.GetKeyDown (KeyCode.Alpha3) && transform.childCount >= 3) {
			selectedWeapon = 2;
		}

		if (previousSelectedWeapon != selectedWeapon) {
			SelectWeapon ();
		}
	}

	void Aim(){
		if (isAiming && !currentWeapon.GetComponent<Gun>().isReloading) {
			if(transform.localPosition != aimDownSightPos)
				transform.localPosition = Vector3.Lerp (transform.localPosition, aimDownSightPos, Time.deltaTime * 10f);

			//transform.localPosition = aimDownSightPos;
			if(mainCam.fieldOfView != aimFOV && gameModeType == GameModeType.SinglePlayer)
				//mainCam.fieldOfView = Mathf.Lerp (mainCam.fieldOfView, aimFOV, Time.deltaTime * 10f);
				mainCam.fieldOfView = aimFOV;

			crossHair.SetActive (false);
		} else {
			if(transform.localPosition != defaultPos)
				transform.localPosition = Vector3.Lerp (transform.localPosition, defaultPos, Time.deltaTime * 10f);

			if(mainCam.fieldOfView != defaultFOV && gameModeType == GameModeType.SinglePlayer)
				//mainCam.fieldOfView = Mathf.Lerp (mainCam.fieldOfView, defaultFOV, Time.deltaTime * 10f);
				mainCam.fieldOfView = defaultFOV;

			//transform.localPosition = defaultPos;
			crossHair.SetActive (true);
		}

		if(gameModeType == GameModeType.SinglePlayer)
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

	void ThrowLethal(){
		if (Input.GetKeyDown (KeyCode.Alpha4) && lethalAmmount > 0) {

			int defaultLayer = LayerMask.NameToLayer ("Default");

			GameObject lethal = Instantiate (lethalPrefab, grenadeThrowPos.position, Quaternion.identity);
			lethal.layer = defaultLayer;
			lethal.transform.parent = null;
			Rigidbody rb = lethal.GetComponent<Rigidbody> ();
			lethal.GetComponent<Grenade> ().isThrown = true;
			rb.AddForce (grenadeThrowPos.forward * throwForce, ForceMode.VelocityChange);
			lethalAmmount--;
			lethalAmmountText.text = lethalAmmount.ToString ();

			if(lethalAmmount == 0) {
				Color color = lethalAmmountText.color;
				color.a = 0.5f;
				lethalAmmountText.color = color;
			} else {
				Color color = lethalAmmountText.color;
				color.a = 1f;
				lethalAmmountText.color = color;
			}
		}
	}

	void SelectWeapon(){

		int i = 0;
		foreach (Transform weapon in transform) {
			if (i == selectedWeapon) {
				if(weapon.GetComponent<Gun>().isMelee)
					// Change the weapon animator state to the idel state
					weapon.GetComponent<Animator> ().Play("Base Layer.Idle");

				weapon.gameObject.SetActive (true);
				weaponIcon.sprite = weapon.GetComponent<Gun> ().gunImage;
				weapon.GetComponent<Gun> ().enabled = true;

				currentAmmoText.text = weapon.GetComponent<Gun> ().currentAmmo.ToString();
				
				invetoryAmmoText.text = weapon.GetComponent<Gun> ().inventoryAmmo.ToString ();

				
				RenewText (weapon.GetComponent<Gun> ());

				if(gameModeType == GameModeType.Multiplayer)
					ClientSend.WeaponDamage(weapon.GetComponent<Gun>().damage);				

				currentWeapon = weapon;
			} else {
				weapon.gameObject.SetActive (false);
				weapon.GetComponent<Gun> ().enabled = false;
			}
			i++;
		}
	}

	void RenewText(Gun gun){
		if(gun.isMelee)
			return;

		if (gun.currentAmmo > 0) {
			currentAmmoText.color = Color.white;
		} else if (gun.currentAmmo <= 0) {
			currentAmmoText.color = Color.red;
		}

		if (gun.inventoryAmmo > 0) {
			invetoryAmmoText.color = Color.white;
		} else if(gun.inventoryAmmo <= 0) {
			invetoryAmmoText.color = Color.red;
		}
	}
}
