using System.Collections;
using System.Collections.Generic;
using UnityEngine.Animations;
using UnityEngine;
using Mirror;

public class PickUp : NetworkBehaviour
{
    public ParentConstraint constraint;
    public Rigidbody rb;
    public Collider col;

    [SyncVar(hook = nameof(OnOwnerChanged))]
    public GameObject owner = null;

    void OnOwnerChanged(GameObject oldOwner, GameObject newOwner) {


        Debug.Log("--");
        if (oldOwner != null) {
            Debug.Log(oldOwner.name);
        } else {
            Debug.Log("oldowner null");
        }

        if (newOwner != null) {
            Debug.Log(newOwner.name);
        } else {
            Debug.Log("newOwner null");
        }


        if (newOwner == null) {
            drop();
            return;
        }

        FPSNetworkPlayer ownerPlayer = newOwner.GetComponent<FPSNetworkPlayer>();
        FPSNetworkBot ownerBot = newOwner.GetComponent<FPSNetworkBot>();

        if (ownerPlayer != null) {
            transferParent(ownerPlayer);
        } else if (ownerBot != null) {
            transferParentBot(ownerBot);
        }
    }

    public void drop() {
        
        //Unconstrain
        if (constraint.sourceCount > 0) {
            constraint.RemoveSource(0);
        }
        constraint.constraintActive = false;

        //Configure rigidbody
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.angularVelocity = Vector3.zero;

        //Turn collider on
        col.enabled = true;
    }

    public void transferParent(FPSNetworkPlayer player) {

        player.gun = gameObject.GetComponent<Gun>();

        //Turn collider off
        col.enabled = false;

        //Rigidbody changes
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.angularVelocity = Vector3.zero;

        //Configure parent constraint
        constraint.constraintActive = true;
        if (constraint.sourceCount > 0){
            constraint.RemoveSource(0);
        }

        //Find destination
        GameObject destination;
        Gun gunScript = this.gameObject.GetComponent<Gun>();
        if (gunScript is AR) {
            destination = player.RifleDestination;
        }
        destination = player.PistolDestination;

        //Add source
        ConstraintSource source = new ConstraintSource();
        source.sourceTransform = destination.transform;
        source.weight = 1;
        constraint.AddSource(source);
    }

    public void transferParentBot(FPSNetworkBot bot) {

        //Turn collider off
        col.enabled = false;

        //Rigidbody changes
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.angularVelocity = Vector3.zero;

        //Configure parent constraint
        constraint.constraintActive = true;
        if (constraint.sourceCount > 0){
            constraint.RemoveSource(0);
        }

        //Find destination
        GameObject destination;
        Gun gunScript = this.gameObject.GetComponent<Gun>();
        if (gunScript is AR) {
            destination = bot.RifleDestination;
        }
        destination = bot.PistolDestination;

        //Add source
        ConstraintSource source = new ConstraintSource();
        source.sourceTransform = destination.transform;
        source.weight = 1;
        constraint.AddSource(source);
    }
}