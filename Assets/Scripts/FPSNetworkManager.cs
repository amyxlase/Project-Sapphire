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
        Debug.Log("Called OnServerAddPlayer");
        Debug.Log(numPlayers);
        // add player at correct spawn position
        Transform start = playerSpawn.transform;
        start.position += Vector3.right * numPlayers;
        GameObject player = Instantiate(playerPrefab, start.position, start.rotation);
        NetworkServer.AddPlayerForConnection(conn, player);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        // call base functionality (actually destroys the player)
        base.OnServerDisconnect(conn);
    }

}
