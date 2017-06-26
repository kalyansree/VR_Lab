using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
/*
 * This script is applied the the PointTypeSwitcher Game Object.
 * It handles switching between the different types of points/spheres.
 * This is achieved through the SwitchTo() function, which is called whenever the VRTK Radial Menu object triggers an OnClick event.
 * Look at VRTKSDK -> LeftController -> RadialMenu -> RadialMenuUI -> Panel, and the VRTK_Radial Menu script attached to it for a better idea of how SwitchTo() is called.
 */
public class PointTypeSwitcher : MonoBehaviour {

    public List<Transform> allTransformList;

    private void Start()
    {
        allTransformList = new List<Transform>();
        int buttonNo = 0;
        int i = 0;
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

        int i = 0;
        foreach (Transform joint in transform)
        {
            if (i == buttonNo)
                joint.gameObject.SetActive(true);
            else
                joint.gameObject.SetActive(false);
            i++;
        }
        //print(buttonNo);
        if (buttonNo == 4)
        {
            transform.parent.GetComponent<VRTK_Pointer>().enabled = true;
            transform.parent.GetComponent<VRTK_StraightPointerRenderer>().enabled = true;
        }
            
        else
        {
            transform.parent.GetComponent<VRTK_Pointer>().enabled = false;
            transform.parent.GetComponent<VRTK_StraightPointerRenderer>().enabled = false;
        }
    }
}
