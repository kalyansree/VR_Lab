using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeasibilityCheck : MonoBehaviour {
    public GameObject domain;
	// Update is called once per frame
	void Update () {
        bool feasible = true;
        foreach (Transform transform in domain.transform)
        { 
            if (transform.gameObject.CompareTag("Input") || transform.gameObject.CompareTag("Output"))
            {
                if (!transform.gameObject.GetComponent<InputOutputInfo>().checkFeasibility())
                {
                    feasible = false;
                }
            }
        }
	}
}
