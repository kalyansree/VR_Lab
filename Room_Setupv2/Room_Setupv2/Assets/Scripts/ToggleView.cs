using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleView : MonoBehaviour {

    // Update is called once per frame
    public GameObject domain;
	void Update () {
		if(OVRInput.GetDown(OVRInput.RawButton.X))
        {
            foreach (Transform transform in domain.transform)
            {
                if(transform.gameObject.CompareTag("Input") || transform.gameObject.CompareTag("Output"))
                {
                    transform.gameObject.GetComponent<InputOutputInfo>().toggleHemisphereView();
                }
                if(transform.gameObject.CompareTag("Intermediate"))
                {
                    transform.gameObject.GetComponent<IntermediateInfo>().toggleHemisphereView();
                }
            }
        }
	}
}
