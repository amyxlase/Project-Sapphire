using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PickUp : MonoBehaviour
{
    public Transform destination;
    private bool InPickupMode = false;
    private bool isPickedUp = false;

    public void enterPickupMode(GameObject player)
    {
        destination = player.GetComponent<Transform>();
        InPickupMode = true;
    }

    public void exitPickupMode()
    {
        InPickupMode = false;
        this.transform.parent = null;
        gameObject.GetComponent<Rigidbody>().useGravity = true;
    }

    public void pickUp()
    {
        if (Input.GetKeyDown(KeyCode.V) && InPickupMode) {
            NetworkIdentity targetIdentity = destination.GetComponent<NetworkIdentity>();
            NetworkIdentity gunIdentity = gameObject.GetComponent<NetworkIdentity>();
            Gun gun = gameObject.GetComponent<Gun>();
            print(targetIdentity.netId + " picked up gun " + gunIdentity.netId);
            print("gun has shooting speed " + gun.getShootingSpeed());
            isPickedUp = true;
            exitPickupMode();
        }
    }

    void Update() 
    {
        pickUp();
        if (isPickedUp) {
            gameObject.GetComponent<Rigidbody>().useGravity = false;
            this.transform.position = destination.position;
            this.transform.parent = GameObject.Find("Destination").transform;
        }
    }
}
