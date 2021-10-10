using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FPSNetworkPlayer : NetworkBehaviour
{
    public float speed;
    public Animator anim;
    public NetworkAnimator networkAnimator;
    public Collider collider;
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

        if (Input.GetKeyDown(KeyCode.F)) {
            networkAnimator.ResetTrigger("Shoot");
            networkAnimator.SetTrigger("Shoot");
            Debug.Log("Shots fired");


            CapsuleCollider[] colliders = FindObjectsOfType<CapsuleCollider>();
            foreach (Collider collider in colliders) {
                Debug.DrawRay(collider.transform.position, Vector3.up*1000000, Color.green, 1000, true);

                RaycastHit hit;
                Ray fromCamera = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (collider.Raycast(fromCamera, out hit, Mathf.Infinity)) {
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
    }

    public override void OnStartAuthority() {
        Transform fpc = transform.Find("FirstPersonCharacter");
        fpc.GetComponent<Camera>().enabled = true;
        fpc.GetComponent<AudioListener>().enabled = true;

        isActive = true;
    }

    [ClientRpc]
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

        Damageable playerDamage = target.gameObject.GetComponent<Damageable>();
        Health playerHealth = target.gameObject.GetComponent<Health>();
        Shield playerShield = target.gameObject.GetComponent<Shield>();
        playerDamage.dealDamage(20);
        print("health: " + playerHealth.getHealth());
        print("shield: " + playerShield.getShield());
    }

}
