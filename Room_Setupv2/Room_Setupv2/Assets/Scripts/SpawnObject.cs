using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObject : MonoBehaviour {

	// Update is called once per frame
	void Update () {
        if (OVRInput.GetUp(OVRInput.Button.One))
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = gameObject.transform.position;
            sphere.transform.rotation = gameObject.transform.rotation;
            sphere.transform.localScale = gameObject.transform.localScale;
            Renderer rend = sphere.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material = gameObject.GetComponent<Renderer>().material;
            }
        }

    }
}
