using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoordCanvasUpdate : MonoBehaviour {

    public GameObject preview;
    public GameObject domain;
    public Text coordText;
	// Update is called once per frame
	void Update () {
        //canvas coordinates
        if (preview.activeSelf)
        {
            Vector3 localCoord = domain.transform.InverseTransformPoint(preview.transform.position);
            localCoord.x += 0.5F;
            localCoord.y += 0.5F;
            localCoord.z += 0.5F;
            localCoord.z = -localCoord.z;
            localCoord.z += 1.0F;
            coordText.text = localCoord.ToString("F4");
        }
    }
}
