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
        crosshair = GameObject.Find("ScopedCrosshairImage");
        crosshair2 = GameObject.Find("DefaultCrosshairImage");
        crosshair.SetActive(false);
    }

    void Update() {
        uint playerNumber = this.gameObject.GetComponent<NetworkIdentity>().netId;
        Debug.Log("Updating from netid " + netId);
        
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

            uint playerNumber = this.gameObject.GetComponent<NetworkIdentity>().netId;
            Debug.Log("Shooting from netid " + netId);


            networkAnimator.ResetTrigger("Shoot");
            networkAnimator.SetTrigger("Shoot");
            Debug.Log("Shots fired");


            RaycastHit hit;
            Ray fromCamera = Camera.main.ScreenPointToRay(Input.mousePosition);
            Collider collider = this.GetComponent<Collider>();
            if (Physics.Raycast(fromCamera, out hit, Mathf.Infinity)) {
                Debug.LogFormat("hit registered {0} of origin {1} collider position {3} and direction/length {2}", hit.transform.gameObject.name, fromCamera.origin, fromCamera.direction, collider.transform.position);

                if (hit.transform.gameObject.name == "FPSNetworkPlayerController(Clone)") {
                    Debug.Log("FPS Player Shot");
                    Debug.DrawRay(fromCamera.origin, fromCamera.direction*1000000, Color.blue, 1000, true);
                } else {
                    Debug.Log("no FPS Player Shot");
                    Debug.DrawRay(fromCamera.origin, fromCamera.direction*1000000, Color.red, 1000, true);
                }

                changeColorOnShot(hit.transform);

            } else {
                Debug.Log("Nothing was shot");
            }
            
        } 
    }


    //CHECK THAT TRANSFORM  IS A VALID ARGUMENT

    [Command]
    public void changeColorOnShot(Transform target) {
        SkinnedMeshRenderer smr = target.GetChild(1).GetChild(1).GetComponent<SkinnedMeshRenderer>();

        if (smr != null) {
            Debug.LogFormat("SKinned mesh renderer not null");
            Material[] mats = smr.materials;
            mats[0] = (Material)Resources.Load("Prefabs/Red");
            smr.materials = mats;
        } else {
            Debug.Log("Nothing was shot");
        }

        Health playerHealth = target.gameObject.GetComponent<Health>();
        playerHealth.Remove(20);
        print(playerHealth.getHealth());
    }

}
