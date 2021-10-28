using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FPSNetworkBot : NetworkBehaviour
{
    private CharacterController m_BotController;
    private Vector3 direction;

    void Start() {
        m_BotController = GetComponent<CharacterController>();
        direction = Vector3.zero;
    }

    private void FixedUpdate() {
        
        ApplyGravity();
        AI();

        //Temporary for beta
        BadAI();
    }

    private void ApplyGravity() {
        float multiplier = 55;
        m_BotController.Move(Physics.gravity * multiplier * Time.deltaTime * Time.deltaTime);
    }

    //Walk in a random direction & change that direction every 100 frames
    private void BadAI() {

        //Randomly generate direction every 100 frames
        if (Time.frameCount % 100 == 0) {
            float x = Random.Range(-1f, 1f);
            float z = Random.Range(-1f, 1f);
            direction = new Vector3(x, 0f, z);
        }

        //Move with speed multiplier
        float multiplier = 1f;
        m_BotController.Move(multiplier * direction * Time.deltaTime);
    }

    //AI code should go here, these steps are a rough guideline but totally optional
    private void AI() {
        //1. Collect information on where other players are, health, etc

        //2. Logic to decide action (walk in direction of players, avoid obstacles, shoot, etc)

        //3. Execute chosen action
    }
}
