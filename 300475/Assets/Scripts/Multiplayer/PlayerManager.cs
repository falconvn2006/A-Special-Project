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

    private Text grenadeText;
    private Text lethalText;

    private void Awake(){
        killsText = GameObject.Find("KillText").GetComponent<Text>();
        deathsText = GameObject.Find("DeathText").GetComponent<Text>();
        healthText = GameObject.Find("HealthText").GetComponent<Text>();
        grenadeText = GameObject.Find("GrenadeText").GetComponent<Text>();
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
        SetHealth(maxHealth);
    }
}
