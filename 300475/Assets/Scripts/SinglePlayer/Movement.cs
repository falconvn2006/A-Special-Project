using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class Movement : MonoBehaviour {

	public static Movement instance;

	private CharacterController controller;

	public float playerHeight;

	[Header("Movement values")]
	public float speed = 12f;
	public float gravity = -9.81f;
	public float groundDistance = 0.4f;
	public float vaultDistance = 1f;
	public float jumpHeight = 3f;
	public float sprintMultiplier = 2f;
	public float sprintPovMul = 1.1f;
	public float slideMultiplier = 4f;
	public float crouchSpeedMultiplier = 0.5f;

	[Header("Stamina Values")]
	public float stamina = 100.0f;
	public float maxStamina = 100.0f;
	[Range(0f, 1f)] public float povTransSpeed = 0.5f;

	// Stamina values
	public float staminaDecreasePerFrame = 1.0f;
	public float staminaIncreasePerFrame = 5.0f;
	public float staminaTimeToRegen = 3.0f;

	[Header("Slide Values")]
	public float slideTimerMax = 2.5f; // time while sliding
	private Vector3 slideForward; // direction of slide
	private float slideTimer = 0.0f;

	[Header("UI Refrences")]
	public GameObject vaultObj;
	public GameObject outOfStamObj;
	public GameObject staminaSlider;

	[Header("References In The Inspector")]
	public GameObject graphicsObj;
	public Transform groundCheck;
	public LayerMask groundMask;
	public LayerMask vaultableWalls;
	public Camera playerCam;
	public Transform headTransform;
	public Transform crouchTrans;

	Vector3 velocity;

	bool isGrounded;
	bool isSliding;
	bool isVaultable;
	bool isCrouching;

	Transform defaultTrans;

	// Hidden values
	private float sprintSpeed;
	private float slideSpeed;
	private float defaultPov;
	private float sprintPov;
	private float staminaTimer = 0.0f;
	private Vector3 defaultTransform;
	private float defaultSpeed;

	Vector3 headPos;

	void Awake(){
		instance = this;
	}

	// Use this for initialization
	void Start () {
		// Set float values
		defaultSpeed = speed;
		sprintSpeed = speed * sprintMultiplier;
		slideSpeed = speed * slideMultiplier;
		defaultPov = playerCam.fieldOfView;
		sprintPov = defaultPov * sprintPovMul;
		defaultTransform = transform.localScale;

		controller = GetComponent<CharacterController>();
		playerCam = GetComponentInChildren<Camera> ();
		defaultTrans = transform;
		headPos = headTransform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
		// Check if is grounded by the ground mask
		// isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

		isGrounded = controller.isGrounded; // Check if is grounded by the character controller

		// Check if close to a wall
		isVaultable = Physics.CheckSphere (transform.position, vaultDistance, vaultableWalls);

		if (stamina <= 0)
			outOfStamObj.SetActive (true);
		else
			outOfStamObj.SetActive (false);

		Crouch();

		if (isSliding) {

			// // Set the height
			// Vector3 h = transform.localScale;
			// h.y *= 0.5f; // Downscale it
			// transform.localScale = h; // Set it

			Vector3 gfxh = graphicsObj.transform.localScale;
			gfxh.y *= 0.5f; // Downscale it
			headTransform.localPosition = crouchTrans.localPosition; // Set the position

			// Move the player forward
			controller.Move (slideForward * Time.deltaTime * slideSpeed);
			slideTimer += Time.deltaTime; // Run the timer

			// Check if timer reach max
			if (slideTimer > slideTimerMax){
				isSliding = false;

				headTransform.localPosition = headPos;
				graphicsObj.transform.localScale = defaultTrans.localScale;
			}
			
			return;
		}

		// Check for input to slide
		if (Input.GetKeyDown(KeyCode.X)) {
			slideTimer = 0.0f; // Start timer
			isSliding = true;
			slideForward = transform.forward; // Slide forward
		}
		else
			isSliding = false;

		// if grounded and gravity is low
		if(isGrounded && velocity.y < 0)
			velocity.y = -2f; // Set the gravity

		float x = Input.GetAxis("Horizontal"); // Get input w-s or up-down
		float z = Input.GetAxis("Vertical"); // Get input a-d or right-left
	
		// Calculate direction
		Vector3 move = transform.right * x + transform.forward * z;

		Vault(move);

		if (Input.GetKey(KeyCode.LeftShift) && !FindObjectOfType<WeaponHolder>().isAiming && !FindObjectOfType<Gun>().isReloading && isGrounded && stamina > 0) {
			if (Input.GetAxisRaw ("Vertical") > 0) {
				staminaSlider.SetActive (true);
				controller.Move (move * Time.deltaTime * sprintSpeed);
				playerCam.fieldOfView = Mathf.Lerp (playerCam.fieldOfView, sprintPov, povTransSpeed);
				stamina = Mathf.Clamp (stamina - (staminaDecreasePerFrame * Time.deltaTime), 0.0f, maxStamina);
				staminaSlider.GetComponent<Slider> ().value = stamina;
				staminaTimer = 0.0f;
			}
		}
		else {
			controller.Move(move * Time.deltaTime * speed);
			if (playerCam.fieldOfView != defaultPov)
				playerCam.fieldOfView = Mathf.Lerp (playerCam.fieldOfView, defaultPov, povTransSpeed);
		}

		if(Input.GetButtonDown("Jump") && isGrounded)
			velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);


		if (stamina < maxStamina)
		{
			if (staminaTimer >= staminaTimeToRegen) {
				stamina = Mathf.Clamp (stamina + (staminaIncreasePerFrame * Time.deltaTime), 0.0f, maxStamina);
				staminaSlider.GetComponent<Slider> ().value = stamina;
				StartCoroutine (StaminaClose ());
			}
			else
				staminaTimer += Time.deltaTime;
		}

		if (stamina <= 0)
			Debug.Log ("Out of stamina");

		velocity.y += gravity * Time.deltaTime;
		controller.Move(velocity * Time.deltaTime);
	}

	IEnumerator StaminaClose(){
		yield return new WaitForSeconds (2f);
		staminaSlider.SetActive (false);
	}

	void OnDrawGizmosSelected(){
		Gizmos.color = Color.red;
		Gizmos.DrawRay (headTransform.position, headTransform.forward);
	}

	void Vault(Vector3 move){
		// if close to a vaultable wall
		if (isVaultable) {
			// Activate UI
			vaultObj.SetActive (true);

			// Check if key is pressed
			if (Input.GetKeyDown (KeyCode.V)) {
				Debug.Log ("Vaulting");
				Vector3 dir = move.normalized;

				// Head position
				//Vector3 maxVaultPos = transform.position + Vector3.up * 1.5f; test code
				Vector3 maxVaultPos = headTransform.position;

				// Return if there is no place to vault
				if (Physics.Raycast (maxVaultPos, dir, vaultDistance, groundMask))
					return;

				// Position of the landing
				Vector3 hoverPos = maxVaultPos + dir * 2;
			
				// Raycast to find ground
				RaycastHit hit;
				if (!Physics.Raycast (hoverPos, Vector3.down, out hit, 3f, groundMask))
					return;
			
				// Calculate the land pos
				Vector3 landPos = hit.point + (Vector3.up * playerHeight * 0.5f);
				transform.position = landPos;
				Debug.Log ("Finish Vaulting");
			}
		} else {
			vaultObj.SetActive (false);
		}
	}

	void Crouch(){
		if(Input.GetKeyDown(KeyCode.C) && !isCrouching){
			isCrouching = true;
			Vector3 gfxh = graphicsObj.transform.localScale;
			gfxh.y *= 0.5f; // Downscale it
			headTransform.localPosition = crouchTrans.localPosition; // Set the position
			speed *= crouchSpeedMultiplier;
		}
		else if(Input.GetKeyDown(KeyCode.C) && isCrouching){
			isCrouching = false;

			headTransform.localPosition = headPos;
			graphicsObj.transform.localScale = defaultTrans.localScale;
			speed = defaultSpeed;
		}
	}
}
