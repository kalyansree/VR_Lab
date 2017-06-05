using UnityEngine;
using System.Collections;

public class ProximityDimmer : MonoBehaviour {

    // Use this for initialization
    GameObject player;
    private Light lt;

    void Start () {
        player = GameObject.FindWithTag("Player");
        lt = GameObject.Find("mylight").GetComponent<Light>();
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 playerPosition = player.transform.position; //player's position
        Vector3 origin = new Vector3(0, 0);//origin

        float dist = (playerPosition - origin).magnitude; //dist

        lt.intensity = 8 - 2 * dist;
        Debug.Log("The current dist " + dist);
        Debug.Log("The current intensity " + lt.intensity);


    }
}
