using System.Collections;
using System.Collections.Generic;
using UnityEngine.Animations;
using UnityEngine;
using Mirror;

public class PickUp : NetworkBehaviour
{
    public ParentConstraint constraint;

    public void drop() {
        //Unconstrain
        constraint.RemoveSource(0);

        //Configure rigidbody
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.angularVelocity = Vector3.zero;
    }

    public void transferParent(FPSNetworkPlayer player) {

        player.gun = gameObject.GetComponent<Gun>();

        //Rigidbody changes
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
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

        //Rigidbody changes
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
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
