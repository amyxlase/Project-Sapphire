using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Leaderboard : MonoBehaviour
{    
    void OnEnable (){
    
        //Get array of all players
        //NetworkIdentity [] players = NetworkServer.spawned.Values.ToArray();
        //Set up list elements for each player
    
        var players = FindObjectsOfType<FPSNetworkPlayer>();
        foreach (FPSNetworkPlayer player in players){
            uint ID = player.GetComponent<NetworkIdentity>().netId;
            Debug.Log("Num Kills: " + player.kills + " ID: " + ID);
        }
    }

    // void OnDisable()
    //{  

    //}
}
