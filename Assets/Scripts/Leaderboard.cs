using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using static System.Math; 

public class ITEM
{
    public string itemID {get; set;}
    public string itemKills {get; set;}
}

public class Leaderboard : MonoBehaviour
{    

    public Text Test;
    public GameObject rowPrefab;
    public Transform rowsParent;
    bool setUp = false;
    int countPlayers = 0;

    void OnEnable (){

        //Setup row game objects
        if (!setUp){
                for (int i = 0; i < 8; i++){
                    GameObject newRow = Instantiate(rowPrefab, rowsParent);
                } 
                setUp = true;
            }
    

        //Loop Through Player Data
        var players = FindObjectsOfType<FPSNetworkPlayer>();
        // public PriorityQueue<uint, int> topKills = new PriorityQueue<uint, int>(new KillComparer());
        public List<ITEM> topKills = new List<ITEM>();
        foreach (FPSNetworkPlayer player in players){
            uint ID = player.GetComponent<NetworkIdentity>().netId;
            topKills.Add(new ITEM {itemID = ID, itemKills = player.kills});
            countPlayers += 1;
            Debug.Log("Num Kills: " + player.kills + " ID: " + ID);
            }

        //Sort Player Data
        topKills = topKills.OrderBy(w => w.itemKills).ToList();

        GameObject [] texts = GameObject.FindGameObjectsWithTag("rowComponent");
        // Draw 8 pink rows
        for (int i = 0; i < texts.Length; i++){
                int s = i / 4;
                if (i % 4 == 3)
                    texts[i].transform.localPosition = new Vector3(10, 60 + s*(-40), 0);
            }
        // Display Top players
        for (int i = 0; i < Min(countPlayers*4, 32); i++){
            int s = i / 4;
            //name
            if (i % 4 == 0){
                texts[i].transform.localPosition = new Vector3(-140, 60 + s*(-40), 0);
                texts[i].GetComponent<TMPro.TextMeshProUGUI>().text = topKills[s].itemID.ToString();
            }
            //num kills
            else if (i % 4 == 1){
                texts[i].transform.localPosition = new Vector3(70, 60 + s*(-40), 0);
                texts[i].GetComponent<TMPro.TextMeshProUGUI>().text = topKills[s].itemKills.ToString();
            }
            //Time in Game
            else if (i % 4 == 2)
                texts[i].transform.localPosition = new Vector3(140, 60 + s*(-40), 0);
        }
    }


    // void OnDisable()
    //{  

    //}
}
