using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeObject : MonoBehaviour {
    // Use this for initialization
    public OVRInput.Controller LController = OVRInput.Controller.LTouch;
    public OVRInput.Controller RController = OVRInput.Controller.RTouch;
    private float initDist, currDist = -1;
    private Vector3 initScale;
    private Vector3 currScale;
    private float totalMagnification;
    void Start()
    {
        initScale = gameObject.transform.localScale;
        totalMagnification = 1;
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
                currScale = gameObject.transform.localScale;
            }
            else
            {
                currDist = Vector3.Distance(Lcontroller_loc, Rcontroller_loc);
                gameObject.transform.localScale = currScale *  currDist / initDist;
                //print("initDist: " + initDist);
                //print("currDist: " + currDist);
            }
            
        }
        if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger) || OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger))
        {
            initDist = -1;
            currDist = -1;

            totalMagnification = gameObject.transform.localScale.x / initScale.x;
            if(totalMagnification >= 8)
            {
                gameObject.transform.localScale = initScale * 8;
                totalMagnification = 8;
            }
            else if(totalMagnification <= 0.05)
            {
                gameObject.transform.localScale = initScale * 0.05F;
                totalMagnification = 0.05F;
            }

            //print(totalMagnification);
        }

    }

    public float getMagnification()
    {
        return totalMagnification;
    }
}
