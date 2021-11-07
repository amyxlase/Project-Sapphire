using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class Leaderboard : MonoBehaviour
{    

    public Text Test;
    public GameObject rowPrefab;
    public Transform rowsParent;

    void OnEnable (){
    
        //Get array of all players
        //NetworkIdentity [] players = NetworkServer.spawned.Values.ToArray();
        //Set up list elements for each player
    

        var players = FindObjectsOfType<FPSNetworkPlayer>();
        foreach (FPSNetworkPlayer player in players){
            uint ID = player.GetComponent<NetworkIdentity>().netId;
            //TextMeshPro [] texts = newRow.GetComponentsInChildren<TextMeshPro>();
            //get tags find object of rag
            //texts[0].text = ID.ToString();
            //texts[1].text = player.kills.ToString();
            GameObject [] texts = GameObject.FindGameObjectsWithTag("rowComponent");
            GameObject newRow = Instantiate(rowPrefab, rowsParent);
            for (int i = 0; i < texts.Length; i++){
                int s = i / 4;
                if (i % 4 == 0){
                    texts[i].transform.localPosition = new Vector3(-140, 60 + s*(-40), 0);
                    texts[i].GetComponent<TMPro.TextMeshProUGUI>().text = ID.ToString();
                }
                else if (i % 4 == 1){
                    texts[i].transform.localPosition = new Vector3(70, 60 + s*(-40), 0);
                    texts[i].GetComponent<TMPro.TextMeshProUGUI>().text = player.kills.ToString();
                }
                else if (i % 4 == 2)
                    texts[i].transform.localPosition = new Vector3(140, 60 + s*(-40), 0);
                else if (i % 4 == 3)
                    texts[i].transform.localPosition = new Vector3(10, 60 + s*(-40), 0);
            }


    

            Debug.Log("Num Kills: " + player.kills + " ID: " + ID);
        }
    }


    // void OnDisable()
    //{  

    //}
}
