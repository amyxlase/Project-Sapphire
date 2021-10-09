using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
                Debug.Log("Scoping out");
            }
            else {
                Camera.main.fieldOfView -= 35;
                Camera.main.orthographicSize -= 0.5f;
                zoomToggle = true;
                crosshair2.SetActive(false);
                crosshair.SetActive(true);
                Debug.Log("Scoping in");
            }
        }
    }

    public void Shoot() {
        if (Input.GetKeyDown(KeyCode.F)) {

            Debug.Log("Shooting from netid " + netId + "and client authority is " + hasAuthority);


            networkAnimator.ResetTrigger("Shoot");
            networkAnimator.SetTrigger("Shoot");
            Debug.Log("Shots fired");


           
            RaycastHit hit;
            Ray fromCamera = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(fromCamera, out hit, Mathf.Infinity)) {
                Debug.LogFormat("hit registered {0}", hit.transform.gameObject.name);
                if (hit.transform.gameObject.name == "FPSNetworkPlayerController(Clone)") {
                    DealDamage(hit.transform);
                    Debug.Log("FPS Player Shot");
                    Debug.DrawRay(fromCamera.origin, fromCamera.direction*1000000, Color.blue, 1000, true);
                }
            } else {
                Debug.Log("Nothing was shot");
            }
            
            
        } 
    }

    public void DealDamage(Transform target) {
        NetworkIdentity targetIdentity = target.gameObject.GetComponent<NetworkIdentity>();
        Health playerHealth = target.gameObject.GetComponent<Health>();
        playerHealth.Remove(20);
        Debug.Log("Hit player " + targetIdentity.netId + "with remaining health " + playerHealth.getHealth());
        BroadcastDamage();
    }

    [Command]
    public void BroadcastDamage() {
        Debug.Log("i hit u");
    }

}
