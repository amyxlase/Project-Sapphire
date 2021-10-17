﻿using System.Collections;
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

    public bool isActive;

    [AddComponentMenu("")]

    void Awake() {
        crosshair = GameObject.Find("ScopedCrosshairImage");
        crosshair2 = GameObject.Find("DefaultCrosshairImage");
        crosshair.SetActive(false);
    }

    void Update() {
        if (!hasAuthority) return;

        ScopeToggle();              // Bound to Q
        Shoot();                    // Bound to F
    }

    public override void OnStartAuthority() {
        Transform fpc = transform.Find("FirstPersonCharacter");
        fpc.GetComponent<Camera>().enabled = true;
        fpc.GetComponent<AudioListener>().enabled = true;
        isActive = true;
    }

    public void ScopeToggle() {
        if (Input.GetKeyDown(KeyCode.Q)) {
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
        if (Input.GetKeyDown(KeyCode.F)) {

            //Start animation
            networkAnimator.ResetTrigger("Shoot");
            networkAnimator.SetTrigger("Shoot");

            // Draw Raycast
            RaycastHit hit;
            Ray fromCamera =  Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(fromCamera, out hit, Mathf.Infinity)) {
                if (hit.transform.gameObject.name == "FPSNetworkPlayerController(Clone)") {
                    CmdDealDamage(hit.transform);
                    UpdateHealth(hit.transform);
                }
            }
        }
    }

    [Command]
    public void CmdDealDamage(Transform target) {
        NetworkIdentity targetIdentity = target.gameObject.GetComponent<NetworkIdentity>();
        Damageable playerDamage = target.gameObject.GetComponent<Damageable>();
        Health playerHealth = target.gameObject.GetComponent<Health>();
        Shield playerShield = target.gameObject.GetComponent<Shield>();
        playerDamage.dealDamage(20);
        print("health: " + playerHealth.getHealth());
        print("shield: " + playerShield.getShield());
        Debug.Log("Hit player " + targetIdentity.netId + "with remaining health " + playerHealth.getHealth());
    }

<<<<<<< Updated upstream
}
=======
    public void UpdateHealth(Transform target) {
        NetworkIdentity targetIdentity = target.gameObject.GetComponent<NetworkIdentity>();
        Slider HP = target.gameObject.GetComponent<Slider>();
        Health playerHealth = target.gameObject.GetComponent<Health>();
        HP.value = playerHealth.getHealth();
        Debug.Log("Updating Health of " + targetIdentity.netId + "with remaining health " + playerHealth.getHealth());
    }   

    

}
>>>>>>> Stashed changes
