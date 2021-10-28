using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launchpad : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("COLLISION ENTERED");
        CharacterController controller = other.gameObject.GetComponent<CharacterController>();
        Vector3 upwards = new Vector3(0f, 50f, 0f);
        controller.Move(upwards);
    }
}
