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
    public GameObject domain;

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
    void Update()
    {
        if(OVRInput.GetDown(OVRInput.Button.Start))
        {
            string message = ConvertToString(allTransformList, true);
            print(message);
            message = ConvertToString(domain.GetComponent<InitLines>().lineTransformList, false);
            print(message);


            //TODO: Send data as client to Matlab
        }
    }

    public string ConvertToString(List<Transform> transformList, bool includeType)
    {
        int maxChars = transformList.Count * 3 * 10;
        char[] message = new char[maxChars];
        int i = 0;
        foreach(Transform transform in transformList)
        {
            Vector3 translate = transform.localPosition;
            translate.x += 0.5F;
            translate.y += 0.5F;
            translate.z += 0.5F;

            string substring = translate.ToString("F4");
            foreach(char c in substring)
            {
                message[i] = c;
                i++;
            }
            if(includeType)
            {
                if (transform.gameObject.CompareTag("Input"))
                    message[i] = 'I';
                else if (transform.gameObject.CompareTag("Output"))
                    message[i] = 'O';
                else if (transform.gameObject.CompareTag("Intermediate"))
                    message[i] = 'T';
                else if (transform.gameObject.CompareTag("Fixed"))
                    message[i] = 'F';
                i++;
            }

            message[i] = ' ';
            i++;
        }
        string finalMessage = new string(message);
        return finalMessage;
        
    }
}

