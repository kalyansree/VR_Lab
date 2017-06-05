using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeObject : MonoBehaviour {
    // Use this for initialization
    public OVRInput.Controller LController = OVRInput.Controller.LTouch;
    public OVRInput.Controller RController = OVRInput.Controller.RTouch;
    private float initDist, currDist = -1;
    private Vector3 initScale;
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) && OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
        {
            Vector3 Lcontroller_loc = OVRInput.GetLocalControllerPosition(LController);
            Vector3 Rcontroller_loc = OVRInput.GetLocalControllerPosition(RController);
            if (initDist == -1 && currDist == -1 || initDist == 0 && currDist == -1)
            {
                initDist = Vector3.Distance(Lcontroller_loc, Rcontroller_loc);
                initScale = gameObject.transform.localScale;
            }
            else
            {
                currDist = Vector3.Distance(Lcontroller_loc, Rcontroller_loc);
                gameObject.transform.localScale = initScale *  currDist / initDist;
                //print("initDist: " + initDist);
                //print("currDist: " + currDist);
            }
            
        }
        if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger) || OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger))
        {
            print("up");
            initDist = -1;
            currDist = -1;
            initScale = gameObject.transform.localScale;
        }

    }
}
