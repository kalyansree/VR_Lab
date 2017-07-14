using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System;
using System.IO;
using System.Text;

/*
 * This script is applied the the PointTypeSwitcher Game Object.
 * It handles switching between the different types of points/spheres.
 * This is achieved through the SwitchTo() function, which is called whenever the VRTK Radial Menu object triggers an OnClick event.
 * Look at VRTKSDK -> LeftController -> RadialMenu -> RadialMenuUI -> Panel, and the VRTK_Radial Menu script attached to it for a better idea of how SwitchTo() is called.
 */
public class PointTypeSwitcher : MonoBehaviour { 
    int activeSphere;
    enum Type:int
    {
        INPUT,
        OUTPUT,
        INTERMEDIATE,
        FIXED,
        DELETE,
        FORCE
    };

    public GameObject RadialMenu;
    private void Start()
    {
        
        int buttonNo = 0;
        int i = 0;
        activeSphere = 0;
        foreach (Transform joint in transform)
        {
            if (i == buttonNo)
                joint.gameObject.SetActive(true);
            else
                joint.gameObject.SetActive(false);
            i++;
        }
    }
    public void SwitchTo(int buttonNo)
    {        
        if(buttonNo == (int)Type.DELETE || buttonNo == (int)Type.INTERMEDIATE || buttonNo == (int)Type.FORCE) 
        {
            RadialMenu.SetActive(false);
        }
        else
        {
            RadialMenu.SetActive(true);
        }
        int i = 0;
        foreach (Transform joint in transform)
        {
            if (i == buttonNo)
            {
                joint.gameObject.SetActive(true);
                if(!(i == (int)Type.DELETE || i == (int)Type.INTERMEDIATE || i == (int)Type.FORCE))
                {
                    FixedDirections fd = joint.gameObject.GetComponent(typeof(FixedDirections)) as FixedDirections;
                    if (i == (int)Type.FIXED)
                    {
                        fd.fixedX = true;
                        fd.fixedY = true;
                        fd.fixedZ = true;
                    }
                    fd.reapplyColor();
                }
            }
            else
                joint.gameObject.SetActive(false);
            i++;
        }
        activeSphere = buttonNo;
    }
    
    public void toggleActiveSphereFixedDir(int dir)
    {
        FixedDirections fd = transform.GetChild(activeSphere).gameObject.GetComponent(typeof(FixedDirections)) as FixedDirections;
        fd.toggleDirection(dir);

    }
}

