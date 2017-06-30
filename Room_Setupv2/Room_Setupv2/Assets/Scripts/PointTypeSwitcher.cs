using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    public List<Transform> allTransformList;
    public GameObject domain;
    private bool messageSet;
    //Networking
    TcpListener listener;
    StreamWriter theWriter;
    String msg;

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


        //Networking
        listener = new TcpListener(55001);
        listener.Start();
        print("is listening");
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
            msg = ConvertToString(allTransformList, true);
            print(msg);
            msg = ConvertToString(domain.GetComponent<InitLines>().lineTransformList, false);
            print(msg);
            //messageSet = true;
        }

        if (testConnection() && messageSet)
        {
            SendData(msg);
            messageSet = false;
        }
    }

    public string ConvertToString(List<Transform> transformList, bool vertexList)
    {

        StringBuilder sb = new StringBuilder();
        if (vertexList) //aka first string
        { 
            int index = 1;
            foreach (Transform transform in transformList)
            {
                Vector3 translate = transform.localPosition;
                translate.x += 0.5F;
                translate.y += 0.5F;
                translate.z += 0.5F;
                sb.Append(index++);
                index++;
                string substring = translate.ToString("F4");
                sb.Append(substring);
                if (transform.gameObject.CompareTag("Input"))
                    sb.Append("I ");
                else if (transform.gameObject.CompareTag("Output"))
                    sb.Append("O ");
                else if (transform.gameObject.CompareTag("Intermediate"))
                    sb.Append("T ");
                else if (transform.gameObject.CompareTag("Fixed"))
                    sb.Append("F ");
            }
            return sb.ToString();
        }
        else //second string
        {
            foreach (Transform transform in transformList)
            {
                for(int i = 0; i < allTransformList.Count; i++)
                {
                    if(transform.position == allTransformList[i].position)
                    {
                        sb.Append(i+1);
                        break;
                    }
                }
                sb.Append(' ');
            }
            return sb.ToString();
        }
    }

    public void SendData(string data)
    {
        while (!listener.Pending())
        {
            //wait
        }
        print("socket comes");
        TcpClient client = listener.AcceptTcpClient();
        NetworkStream ns = client.GetStream();
        StreamReader reader = new StreamReader(ns);
        theWriter = new StreamWriter(ns);
        theWriter.AutoFlush = true;
        theWriter.WriteLine(data);
        Debug.Log("socket is sent");
    }

    public bool testConnection()
    {
        if (!listener.Pending())
        {
            return false;
        }
        return true;
    }
}

