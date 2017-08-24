using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleView : MonoBehaviour {

    // Update is called once per frame
    public GameObject domain;
    bool visible;

    void Start()
    {
        visible = true;
    }
    void Update () {
        
        foreach (Transform transform in domain.transform)
        {
            if (transform.gameObject.CompareTag("Input") || transform.gameObject.CompareTag("Output"))
            {
                transform.gameObject.GetComponent<InputOutputInfo>().toggleHemisphereView(visible);
            }
            if (transform.gameObject.CompareTag("Intermediate"))
            {
                transform.gameObject.GetComponent<IntermediateInfo>().toggleHemisphereView(visible);
            }
        }
        if (OVRInput.GetDown(OVRInput.RawButton.X))
        {
            if (visible)
            {
                visible = false;
            }
            else
            {
                visible = true;
            }
        }
	}
}
