using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public int id;
    public string username;
    public float health;
    public float maxHealth;
    public MeshRenderer model;

    public int kills;
    public int deaths;

    private Text killsText;
    private Text deathsText;
    private Text healthText;

    private Text pingText;

    private Text grenadeText;
    private Text lethalText;

    private float currentTime = 0f;

    private void Awake(){
        killsText = GameObject.Find("KillText").GetComponent<Text>();
        deathsText = GameObject.Find("DeathText").GetComponent<Text>();
        healthText = GameObject.Find("HealthText").GetComponent<Text>();
        grenadeText = GameObject.Find("GrenadeText").GetComponent<Text>();
        pingText = GameObject.Find("PingText").GetComponent<Text>();
    }

    private void Update(){
        currentTime += Mathf.Round(Time.deltaTime * 1000);
        ClientSend.Ping(id);

        if(this.gameObject.tag == "LocalPlayer")
            pingText.text = currentTime + " ms";
    }

    public void ResetPing(){
        currentTime = 0f;
    }

    public void Initialize(int _id, string _username){
        id = _id;
        username = _username;
        health = maxHealth;
    }

    public void SetHealth(float _health){
        health = _health;

        if(this.gameObject.tag == "LocalPlayer")
            healthText.text = health.ToString();

        if(health <= 0f){
            Die();
        }
    }

    public void SetKDText(){
        if(this.gameObject.tag == "LocalPlayer"){
            killsText.text = kills.ToString();
            deathsText.text = deaths.ToString();
        }
    }

    public void SetLethal(int _grenade){
        if(this.gameObject.tag == "LocalPlayer"){
            grenadeText.text = _grenade.ToString();
        }
    }

    public void SetHitMarker(){
        if(this.gameObject.tag == "LocalPlayer"){
            UIManager.instance.HitMarker();
        }
    }

    public void Die(){
        model.enabled = false;
    }

    public void Respawn(){
        model.enabled = true;
        GetComponentInChildren<WeaponHolder>().ResetWeapon();
        grenadeText.text = "4";
        SetHealth(maxHealth);
    }
}
