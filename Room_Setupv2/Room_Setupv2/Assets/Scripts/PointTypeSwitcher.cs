using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
public class PointTypeSwitcher : MonoBehaviour {
    public Component script;
    private void Start()
    {
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
            GameObject.Find("RightController").GetComponent<VRTK_Pointer>().enabled = true;
            GameObject.Find("RightController").GetComponent<VRTK_StraightPointerRenderer>().enabled = true;
        }
            
        else
        {
            GameObject.Find("RightController").GetComponent<VRTK_Pointer>().enabled = false;
            GameObject.Find("RightController").GetComponent<VRTK_StraightPointerRenderer>().enabled = false;
        }
    }
}
