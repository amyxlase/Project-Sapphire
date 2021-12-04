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
        Debug.LogFormat("Syncvar called on {0}, {1}", oldOwner, newOwner);

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
        AR rifle = this.gameObject.GetComponent<AR>();
        if (rifle != null) {
            destination = player.RifleDestination;
        } else {
            destination = player.PistolDestination;
        }

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
        GameObject destination = bot.RifleDestination;

        //Add source
        ConstraintSource source = new ConstraintSource();
        source.sourceTransform = destination.transform;
        source.weight = 1;
        constraint.AddSource(source);
    }

    [Command(requiresAuthority = false)]
    public void playerPickUp(GameObject player) {
        Debug.Log("server gun pickup");
        this.owner = player;
        this.transferParent(player.GetComponent<FPSNetworkPlayer>());
    }

    [Command(requiresAuthority = false)]
    public void playerDrop() {
        Debug.Log("server gun drop");
        this.owner = null;
        this.drop();
    }
}