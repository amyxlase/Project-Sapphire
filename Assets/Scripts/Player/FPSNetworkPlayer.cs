using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

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
    public Slider HP;

    public bool isActive;

    public Gun gun;

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

        //Configure crosshair
        crosshair = GameObject.Find("ScopedCrosshairImage");
        crosshair2 = GameObject.Find("DefaultCrosshairImage");
        crosshair.SetActive(false);

        //Configure leaderboard
        leaderboard = GameObject.Find("LeaderBoard");
        leaderboard.SetActive(false);

        //Find dead screen
        Dead = transform.GetChild(3).gameObject;

        //Enable camera & audio listener
        Transform fpc = transform.Find("FirstPersonCharacter");
        fpc.GetComponent<Camera>().enabled = true;
        fpc.GetComponent<AudioListener>().enabled = true;

        //configureHP();
        isActive = true;
    }

    void Update() {
        if (!hasAuthority) return;

        if (HP == null) {
            Debug.Log("HP is null");
        }

        ScopeToggle();              // Bound to Q and right click
        Shoot();                    // Bound to F and left click
        ViewBoard();                // Bound to Tab
        DieOutOfBounds();
        DetectGunToAnimate();
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
        bool gunGood = this.gun != null && gun.getAmmo() > 0;

        if (shootInput && gunGood) {

            //Start animation
            networkAnimator.ResetTrigger("Shoot");
            networkAnimator.SetTrigger("Shoot");

            // Draw Raycast
            RaycastHit hit;
            Ray fromCamera =  Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(fromCamera, out hit, Mathf.Infinity)) {
                if (hit.transform.gameObject.name == "FPSNetworkPlayerController(Clone)"
                    || hit.transform.gameObject.name == "FPSNetworkBotController(Clone)") {
                    Debug.Log("Fired at object named " + hit.transform.gameObject.name);
                    CmdDealDamage(hit.transform);
                }
            }

            //Ammo
            gun.DecrementAmmo();
            Debug.Log(gun.getAmmo());
        }
    }

    public void ViewBoard(){
         if (Input.GetKey(KeyCode.Tab)) {
            leaderboard.SetActive(true);
         } else {
            leaderboard.SetActive(false);
         }
    }

    public void DieOutOfBounds() {
        if (this.transform.position.y < -10) {
            Dead.SetActive(true);
            Destroy(this.gameObject);
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
        Debug.Log("Asked server to deal damage");
        NetworkIdentity targetIdentity = target.gameObject.GetComponent<NetworkIdentity>();
        if (target.gameObject.name == "FPSNetworkPlayerController(Clone)") {
            Debug.Log("player target");
            Damageable playerDamage = target.gameObject.GetComponent<Damageable>();
            Health playerHealth = target.gameObject.GetComponent<Health>();
            FPSNetworkPlayer targetPlayer = target.gameObject.GetComponent<FPSNetworkPlayer>();
            playerDamage.dealDamage(20);
            Debug.Log(targetPlayer);
            Debug.Log(playerHealth);
            //targetPlayer.configureHP();
            //targetPlayer.setHP(playerHealth.getHealth());
            Debug.Log("player has " + playerHealth.getHealth() + " health left");
        } else {
            BotDamageable playerDamage = target.gameObject.GetComponent<BotDamageable>();
            playerDamage.dealDamage(20);

            BotHealth botHealth = target.gameObject.GetComponent<BotHealth>();
            Debug.Log("bot has " + botHealth.getHealth() + " health left");
        }
        
    }

}