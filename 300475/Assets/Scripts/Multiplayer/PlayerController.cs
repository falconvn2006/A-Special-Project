using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    public Transform camTrans;

    public float stamina = 100.0f;
	public float maxStamina = 100.0f;

	// Stamina values
	public float staminaDecreasePerFrame = 1.0f;
	public float staminaIncreasePerFrame = 5.0f;
	public float staminaTimeToRegen = 3.0f;

    private float staminaTimer = 0.0f;

    private void Update(){
    }

    private void FixedUpdate()
    {
        if (stamina < maxStamina)
		{
			if (staminaTimer >= staminaTimeToRegen) {
				stamina = Mathf.Clamp (stamina + (staminaIncreasePerFrame * Time.deltaTime), 0.0f, maxStamina);
			}
			else
				staminaTimer += Time.deltaTime;
		}

        SendInputToServer();
    }

    /// <summary>Sends player input to the server.</summary>
    private void SendInputToServer()
    {
        bool[] _inputs = new bool[]
        {
            Input.GetKey(KeyCode.W), // Index 0
            Input.GetKey(KeyCode.S), // Index 1
            Input.GetKey(KeyCode.A), // Index 2
            Input.GetKey(KeyCode.D), // Index 3
            Input.GetKey(KeyCode.Space), // Index 4
            Input.GetKey(KeyCode.LeftShift), // Index 5
            Input.GetKeyDown(KeyCode.C) // Index 6
        };

        if(Input.GetKey(KeyCode.LeftShift) && Input.GetAxisRaw("Vertical") > 0 && stamina > 0){
            _inputs[5] = true;
            stamina = Mathf.Clamp (stamina - (staminaDecreasePerFrame * Time.deltaTime), 0.0f, maxStamina);
            staminaTimer = 0.0f;
        }
        
        if(stamina <= 0)
            _inputs[5] = false;

        ClientSend.PlayerMovement(_inputs);
    }
}
