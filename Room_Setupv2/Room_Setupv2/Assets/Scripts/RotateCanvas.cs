using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCanvas : MonoBehaviour {
    public Camera myCamera;

	
	// Update is called once per frame
	void Update () {
        Vector3 v = myCamera.transform.position - transform.position;

        v.x = v.z = 0.0f;

        transform.LookAt(myCamera.transform.position - v);

        transform.rotation = (myCamera.transform.rotation); // Take care about camera rotation
    }
}
