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
    public Canvas canvas;
    private bool messageSet;
    //Networking
    internal Boolean socketReady = false;
    TcpClient mySocket;
    NetworkStream theStream;
    StreamWriter theWriter;
    StreamReader theReader;
    String Host = "localhost";
    Int32 Port = 55000;
    String msg1;
    String msg2;

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
    }
    public void Submit()
    {
        setupSocket();
        Debug.Log("socket is set up");
        msg1 = ConvertToString(allTransformList, true);
        print(msg1);
        msg2 = ConvertToString(domain.GetComponent<InitLines>().lineTransformList, false);
        print(msg2);
        messageSet = true;
        canvas.transform.Find("PendingText").gameObject.SetActive(true);
    }

    void Update()
    {
        if (messageSet)
        {
            SendData(msg1);
            SendData(msg2);
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
        theWriter.Write(data);
        theWriter.Write('|');
        canvas.transform.Find("PendingText").gameObject.SetActive(false);
        canvas.transform.Find("SentText").gameObject.SetActive (true);
    }

    public void setupSocket()
    {
        try
        {
            mySocket = new TcpClient(Host, Port);
            theStream = mySocket.GetStream();
            theWriter = new StreamWriter(theStream);
            socketReady = true;
            theWriter.AutoFlush = true;
            Debug.Log("socket is sent");
        }
        catch (Exception e)
        {
            Debug.Log("Socket error: " + e);   
        }
    }
}

