using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * This class is to be attached to an empty game object to perform the feasibility check.
 * In the current version this script is unused.
 */
public class FeasibilityCheck : MonoBehaviour {
public GameObject domain;

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
