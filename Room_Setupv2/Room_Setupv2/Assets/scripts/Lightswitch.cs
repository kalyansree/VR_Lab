using UnityEngine;
using System.Collections;

public class Lightswitch : MonoBehaviour {

    public Light pointLight;

	// Use this for initialization
	void Start () {
        pointLight = GetComponent<Light>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log("switched");
            pointLight.enabled = !pointLight.enabled;
        }
	}
}
