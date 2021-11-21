using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// different footstep audio
// how it works:
// make a trigger collision on the ground type, and add tag name to ground type
// (for instance, grass island ground vs. bridge ground)
// make sure the material of the ground is in the materials list
// change audio file in firstpersoncontroller playfootstepaudio function

public class TerrainDetect : MonoBehaviour
{
    //public Transform playerTransform;

    //public Terrain t;

    public GameObject ground;

	//public int posX;
	//public int posZ;
	//public float[] textureValues;

    [SerializeField] public Material[] Materials;

    public Material groundMaterial;

    /*
    void Start()
    {
        t = Terrain.activeTerrain;
    }
    */

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Island")
        {
            Debug.Log("on ground");
            ground = other.gameObject;
            groundMaterial = ground.GetComponent<Renderer>().material;
            Debug.Log(groundMaterial);
        }

        if(other.tag == "Bridge")
        {
            Debug.Log("on bridge");
            ground = other.gameObject;
            groundMaterial = ground.GetComponent<Renderer>().material;
            Debug.Log(groundMaterial);
        }
    }

    /*
    public void GetTerrainTexture()
    {
        ConvertPosition(playerTransform.position);
        CheckTexture();
    }

    void ConvertPosition(Vector3 playerPosition)
    {
        Vector3 terrainPosition = playerPosition - t.transform.position;

        Vector3 mapPosition = new Vector3 
        (terrainPosition.x / t.terrainData.size.x, 0, 
        terrainPosition.z / t.terrainData.size.z);

        float xCoord = mapPosition.x * t.terrainData.alphamapWidth;
        float zCoord = mapPosition.z * t.terrainData.alphamapHeight;

        posX = (int)xCoord;
        posZ = (int)zCoord;
    }

    void CheckTexture()
    {
        float[,,] aMap = t.terrainData.GetAlphamaps (posX, posZ, 1, 1);
        
        textureValues[0] = aMap[0,0,0];
        textureValues[1] = aMap[0,0,1];
        textureValues[2] = aMap[0,0,2];
    }
    */

}
