using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intensity : MonoBehaviour
{
    // want an audio source
    // function determine intensity 
    // distance from you and all bots
    // how much health you have left
    // maxhealth - health
    // decay of intensity
    // intensity min 0
    // function intensity to track
    // there should be intensity thresholds
    // array of tracks to play
    
    private AudioSource m_AudioSource;
    private FPSNetworkPlayer player;
    [SerializeField] protected AudioClip[] tracks;

    [SerializeField] private float intensityValue = 0;
    [SerializeField] private float intensityMax = 1000;

    private float prevHealth = 100f;

    public void Start() {
        m_AudioSource = GetComponent<AudioSource>();
        player = GetComponent<FPSNetworkPlayer>();
        m_AudioSource.loop = true;
    }

    public void Update() {

        intensityValue = calculateIntensity();
        Debug.Log("intensity: " + intensityValue);

        // switching track if necessary
        if (getIntensityTrack() != null) {
            if (getIntensityTrack() != m_AudioSource.clip) {
                //m_AudioSource.Stop();
                m_AudioSource.clip = getIntensityTrack();
                m_AudioSource.time = m_AudioSource.time;
                m_AudioSource.Play();
            }
            if (intensityValue < 2) {
                while (m_AudioSource.volume > 0.2f) {
                    m_AudioSource.volume -= 0.0001f * Time.deltaTime;
                }
                m_AudioSource.volume = 0.2f;
            }
            else { 
                while (m_AudioSource.volume < 1f) {
                    m_AudioSource.volume += 0.0001f * Time.deltaTime;
                }
                m_AudioSource.volume = 1f;
            }
        }
        else {
            m_AudioSource.Stop();
            m_AudioSource.clip = null;
        }
        //intensityValue = intensityValue - (0.01f * Time.deltaTime);
    }

    public float calculateIntensity() {
        if (intensityValue > 8.8) return intensityValue - (0.01f * Time.deltaTime);
        float newVal = intensityValue;

        // intensity determined by # of bots around
        float totalDist = 0;

        GameObject[] bots = GameObject.FindGameObjectsWithTag("Bot");
        int botsCount = 1;
        for (int i = 0; i < bots.Length; i++)
        {
            float dist = Vector3.Distance(bots[i].transform.position, transform.position);
            if (dist < 50) {
                totalDist += dist;
                botsCount++;
            }
        }
        float avgDist = 16 - (totalDist / botsCount);
        newVal = avgDist;

        // intensity changes when taking damage
        Health playerHealth = gameObject.GetComponent<Health>();
        float healthDiff = prevHealth - playerHealth.getHealth();
        prevHealth = playerHealth.getHealth();
        newVal += healthDiff;

        newVal = Mathf.Max(newVal, 0);
        newVal = Mathf.Min(newVal, intensityMax);
        return newVal;
    }

    public AudioClip getIntensityTrack() {
        //float threshold1 = 1;
        float threshold2 = 8;
        if (intensityValue < threshold2)
        {
            //Debug.Log("threshold 1 track");
            return tracks[0];
        }
        else if (threshold2 <= intensityValue)
        {
            //Debug.Log("threshold 2 track");
            return tracks[1];
        }
        //Debug.Log("no track");
        return null;
    }

}
