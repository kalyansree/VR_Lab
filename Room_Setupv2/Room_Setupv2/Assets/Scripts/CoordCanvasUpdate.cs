using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoordCanvasUpdate : MonoBehaviour {

    public GameObject preview;
    public GameObject domain;
    public GameObject PointTypeSwitcher;
    public Text coordText;
   
	// Update is called once per frame
	void Update () {
        //canvas coordinates
        Vector3 localCoord;
        if (preview.activeSelf)
        {
            localCoord = domain.transform.InverseTransformPoint(preview.transform.position);
        }
        else
        {
            Vector3 pos = PointTypeSwitcher.GetComponent<PointTypeSwitcher>().GetPosition();
            localCoord = domain.transform.InverseTransformPoint(pos);
        }
        
        localCoord.x += 0.5F;
        localCoord.y += 0.5F;
        localCoord.z += 0.5F;
        localCoord.z = -localCoord.z;
        localCoord.z += 1.0F;
        coordText.text = localCoord.ToString("F4");
    }
}
