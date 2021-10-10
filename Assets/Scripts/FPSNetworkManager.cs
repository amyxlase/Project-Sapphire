using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class FPSNetworkManager : NetworkManager
{
    public GameObject playerSpawn;

    [AddComponentMenu("")]
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        Transform start = playerSpawn.transform;
        start.position += Vector3.right * numPlayers;
        GameObject player = Instantiate(playerPrefab, start.position, start.rotation);
        NetworkServer.AddPlayerForConnection(conn, player);

        //Activate various things
        FPSNetworkPlayer playerScript = player.GetComponent<FPSNetworkPlayer>();
        playerScript.enabled = true;
        NetworkIdentity identity = player.GetComponent<NetworkIdentity>();
        identity.AssignClientAuthority(conn);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        // call base functionality (actually destroys the player)
        base.OnServerDisconnect(conn);
    }

}
