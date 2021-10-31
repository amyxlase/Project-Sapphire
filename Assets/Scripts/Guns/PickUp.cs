using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PickUp : MonoBehaviour
{
    public Transform destination;
    private bool InPickupMode = false;
    private bool IsPickedUp = false;

    // objects that can be picked up have two states:
    // able to be picked up and not

    // pickup-able state:
    // if it's not equipped by anyone &
    // if someone within the pickup radius &
    // if the player is not holding anything (implemented in player, not here)

    // not pickup-able state:
    // is already equipped or
    // no one is around

    // once object is picked up:
    // it goes into not pickup-able state
    // it follows the thing that picked it up

    // once object is dropped: 
    // it goes into pickup mode for players near it
    // it just sits there

    public void enterPickupMode(GameObject player)
    {
        destination = player.GetComponent<Transform>();
        InPickupMode = true;
    }

    public void exitPickupMode()
    {
        InPickupMode = false;
    }

    public void pickUp()
    {
        // to make sure you don't immediately re-equip after you drop
        bool didSomething = false; 

        // press v again to unequip
        if (Input.GetKeyDown(KeyCode.V) && IsPickedUp) {
            if (gameObject.tag == "gun" && destination.gameObject.tag == "Player") {
                FPSNetworkPlayer player = destination.gameObject.GetComponent<FPSNetworkPlayer>();
                Gun playerGun = player.gun;
                print("dropping current gun");
                drop(playerGun.gameObject);
                player.gun = null;
                IsPickedUp = false;
            }
            didSomething = true;
        }

        if (Input.GetKeyDown(KeyCode.V) && InPickupMode && !didSomething) {
            if (gameObject.tag == "gun" && destination.gameObject.tag == "Player") {
                FPSNetworkPlayer player = destination.gameObject.GetComponent<FPSNetworkPlayer>();
                Gun playerGun = player.gun;

                if (playerGun == null) {
                    print("picking up gun");
                    NetworkIdentity targetIdentity = destination.GetComponent<NetworkIdentity>();
                    NetworkIdentity gunIdentity = gameObject.GetComponent<NetworkIdentity>();
                    Gun gun = gameObject.GetComponent<Gun>();
                    player.gun = gun;

                    print(targetIdentity.netId + " picked up gun " + gunIdentity.netId);
                    print("gun has shooting speed " + gun.getShootingSpeed());
                    IsPickedUp = true;
                }
            }
        }

    }

    public void drop(GameObject target) {
        target.transform.parent = null;
        gameObject.GetComponent<Rigidbody>().useGravity = true;
    }

    void Update() 
    {
        pickUp();
        if (IsPickedUp) {
            gameObject.GetComponent<Rigidbody>().useGravity = false;
            this.transform.position = destination.position;
            this.transform.parent = GameObject.Find("Destination").transform;
            exitPickupMode();
        }
    }
}
