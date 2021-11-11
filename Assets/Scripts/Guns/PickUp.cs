using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PickUp : NetworkBehaviour
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
        }

        if (Input.GetKeyDown(KeyCode.V) && InPickupMode) {
            if (gameObject.tag == "gun" && destination.gameObject.tag == "Player") {
                FPSNetworkPlayer player = destination.gameObject.GetComponent<FPSNetworkPlayer>();

                if (player.gun == null) {
                    transferParent(player);
                    IsPickedUp = true;
                    exitPickupMode();
                }
            }
        }

    }

    public void transferParent(FPSNetworkPlayer player) {

        player.gun = gameObject.GetComponent<Gun>();

        //Rigidbody changes
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.angularVelocity = Vector3.zero;

        //Transform changes
        this.transform.parent = getDestination(player).transform;
        //this.transform.position = destination.position;
        this.transform.localPosition = Vector3.zero;
        this.transform.localEulerAngles = new Vector3(0, 0, 0);
    }

    public GameObject getDestination(FPSNetworkPlayer player) {
        Gun gunScript = player.gun.gameObject.GetComponent<Gun>();
        if (gunScript is AR) {
            return player.RifleDestination;
        }
        return player.PistolDestination;
    }

    public void drop(GameObject target) {
        target.transform.parent = null;
        gameObject.GetComponent<Rigidbody>().useGravity = true;
    }

    void Update() 
    {

        pickUp();
    }
}
