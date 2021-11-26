﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
using UnityStandardAssets.Characters.FirstPerson;

public class FPSNetworkPlayer : NetworkBehaviour
{
    public float speed;
    public Animator anim;
    public NetworkAnimator networkAnimator;
    
    public bool zoomToggle = false;
    public GameObject crosshair;
    public GameObject crosshair2;
    public GameObject Dead;
    public GameObject leaderboard;
    public GameObject healthUI;
    public GameObject HUD;
    public Slider HP;
    public float startTime;
    public TextMeshProUGUI timer;
    public TextMeshProUGUI ammoCount;

    public bool isActive;

    public Gun gun;
    public GameObject RifleDestination;
    public GameObject PistolDestination;

    [AddComponentMenu("")]

    public void configureHP() {
        // Configure health bar
        healthUI = transform.GetChild(4).gameObject;
        healthUI.SetActive(true);
        HP = healthUI.transform.GetChild(0).gameObject.GetComponent<Slider>();
        HP.maxValue = 100f;
        HP.value = 100f;
        Debug.Log(HP);
        Debug.Log(HP.maxValue);
        Debug.Log(HP.value);
    }

    public override void OnStartAuthority() {

        //Find leaderboard
        leaderboard = GameObject.Find("Canvas").transform.GetChild(3).gameObject;

        //Configure crosshair
        crosshair = GameObject.Find("ScopedCrosshairImage");
        crosshair2 = GameObject.Find("DefaultCrosshairImage");
        crosshair.SetActive(false);

        //Configure timer
        startTime = Time.time;

        //Find HUD
        this.HUD = this.transform.GetChild(2).GetChild(0).gameObject;
        timer = HUD.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();

        //Get ammo text
        ammoCount = HUD.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        UpdateAmmoText();

        //Find death screen
        Dead = GameObject.Find("Canvas").transform.GetChild(4).GetChild(3).gameObject;

        //Enable camera & audio listener
        Transform fpc = transform.Find("FirstPersonCharacter");
        fpc.GetComponent<Camera>().enabled = true;
        fpc.GetComponent<AudioListener>().enabled = true;

        //configureHP();
        isActive = true;
    }

    void Update() {
        if (!hasAuthority) return;

        ScopeToggle();              // Bound to Q and right click
        Shoot();                    // Bound to F and left click
        Reload();                   // Bound to R
        ViewBoard();                // Bound to Tab
        DieOutOfBounds();
        DetectGunToAnimate();
        UpdateTimer();
    }

    public void UpdateTimer() {
        float delta = Time.time - startTime;
        int minutes = (int) delta / 60 ;
        int seconds = (int) delta - 60 * minutes;
        string timeString = string.Format("{0:00}:{1:00}", minutes, seconds);
        timer.text = timeString;
    }

    public void ScopeToggle() {
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetMouseButtonDown(1)) {
            if (zoomToggle) {
                Camera.main.fieldOfView += 35;
                Camera.main.orthographicSize += 0.5f;
                zoomToggle = false;
                crosshair.SetActive(false);
                crosshair2.SetActive(true);
            }
            else {
                Camera.main.fieldOfView -= 35;
                Camera.main.orthographicSize -= 0.5f;
                zoomToggle = true;
                crosshair2.SetActive(false);
                crosshair.SetActive(true);
            }
        }
    }

    public void Shoot() {

        bool shootInput = Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(0);
        bool gunGood = this.gun != null && gun.getQueuedAmmo() > 0;

        if (shootInput && gunGood) {

            //Art
            networkAnimator.ResetTrigger("Shoot");
            networkAnimator.SetTrigger("Shoot");
            gun.getVFX().Play();
            gun.playGunSound();

            // Draw Raycast
            RaycastHit hit;
            Ray fromCamera =  Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(fromCamera, out hit, Mathf.Infinity)) {
                if (hit.transform.gameObject.name == "FPSNetworkPlayerController(Clone)"
                    || hit.transform.gameObject.name == "FPSNetworkBotController(Clone)") {
                    Debug.Log("Fired at object named " + hit.transform.gameObject.name);
                    CmdDealDamage(hit.transform);
                } else {
                    CapsuleCollider[] colliders = FindObjectsOfType<CapsuleCollider>();
                    Debug.Log("Missed targets " + colliders.Length);
                }
            }

            //Ammo
            gun.DecrementAmmo();
            UpdateAmmoText();
        }
    }

    public void UpdateAmmoText() {
        float q = this.gun.getQueuedAmmo();
        float t = this.gun.getTotalAmmo();
        this.ammoCount.text = string.Format("{0}|{1}", q, t - q);
    }

    public void Reload() {
        if (Input.GetKey(KeyCode.R)) {
            this.gun.Reload();
            UpdateAmmoText();
        }
    }

    public void ViewBoard(){
         if (Input.GetKey(KeyCode.Tab)) {
            leaderboard.SetActive(true);
         } else {
            leaderboard.SetActive(false);
         }
    }

    public void Die() {
        Dead.SetActive(true);
        FirstPersonController controller = this.gameObject.GetComponent<FirstPersonController>();
        controller.m_MouseLook.SetCursorLock(false);
        NetworkServer.Destroy(this.gameObject);
        crosshair.SetActive(true);
        crosshair2.SetActive(true);

    }

    public void DieOutOfBounds() {
        if (this.transform.position.y < -10) {
            Die();
        }
    }

    public void setHP(float newValue) {
        if (HP == null) {
            Debug.Log("HP is null");
        }
        HP.value = newValue;
    }

    //Check if gun is pistol or rifle
    private void DetectGunToAnimate() {
        bool truth = false;
        if (this.gun != null) {
            truth = this.gun.gameObject.GetComponent<AR>() != null;
        }

        anim.SetBool("Rifle", truth);
    }

    [Command]
    public void CmdDealDamage(Transform target) {
        NetworkIdentity targetIdentity = target.gameObject.GetComponent<NetworkIdentity>();
        if (target.gameObject.name == "FPSNetworkPlayerController(Clone)") {
            Damageable playerDamage = target.gameObject.GetComponent<Damageable>();
            playerDamage.dealDamage(20);
        } else {
            BotDamageable playerDamage = target.gameObject.GetComponent<BotDamageable>();
            playerDamage.dealDamage(20);
            BotHealth health = target.gameObject.GetComponent<BotHealth>();
            Debug.Log("bot has " + health.getHealth() + " health left");
        }
        
    }

}