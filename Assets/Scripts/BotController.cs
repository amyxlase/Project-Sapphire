using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BotController : NetworkBehaviour
{
    public float speed;
    public Animator anim;
    public NetworkAnimator networkAnimator;

    public bool isActive;

    void Awake() {

    }

    void Update() {
        if (!hasAuthority) return;
        Shoot();
    }

    public override void OnStartAuthority() {
        isActive = true;
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
}
