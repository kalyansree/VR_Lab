using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setup : MonoBehaviour {
    /*
     * This class is used to set up anything in the scene that might need to run outside of all other scripts.
     */
	void Awake () {
        Hemisphere.CreateHemisphereMesh();
    }
}
