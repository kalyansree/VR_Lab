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
using Vectrosity;

public class Networking : MonoBehaviour {
    internal Boolean socketReady = false;
    TcpClient mySocket;
    NetworkStream theStream;
    StreamWriter theWriter;
    StreamReader theReader;
    String Host = "localhost";
    Int32 Port = 55000;
    String msg1;
    String msg2;
    String msg3;
    private bool messageSet;
    public List<Transform> allTransformList;
    public List<Transform> forceTransformList;
    public List<Transform> deformedTransformList;


    public GameObject domain;
    public GameObject force;
    public Canvas submitCanvas;

    private string errorMsg;

    private int inputCount;
    private int outputCount;

    // Use this for initialization
    void Start () {
        allTransformList = new List<Transform>();
        forceTransformList = new List<Transform>();
        deformedTransformList = new List<Transform>();
    }
	
	// Update is called once per frame
	void Update () {
        if (messageSet)
        {
            SendData(msg1);
            SendData(msg2);
            SendData(msg3);
            messageSet = false;
            String retMsg = theReader.ReadToEnd();
            plotDeformedCoords(retMsg);
        }
    }

    public void Submit()
    {
        if(!checkConditions())
        {
            submitCanvas.transform.Find("WarningText").GetComponent<Text>().text = errorMsg;
            submitCanvas.transform.Find("WarningText").gameObject.SetActive(true);
            submitCanvas.transform.Find("PendingText").gameObject.SetActive(false);
            submitCanvas.transform.Find("SentText").gameObject.SetActive(false);
            return;
        }
        if (!setupSocket())
        {
            submitCanvas.transform.Find("WarningText").GetComponent<Text>().text = errorMsg;
            submitCanvas.transform.Find("WarningText").gameObject.SetActive(true);
            submitCanvas.transform.Find("PendingText").gameObject.SetActive(false);
            submitCanvas.transform.Find("SentText").gameObject.SetActive(false);
            return;
        }
        Debug.Log("socket is set up");
        msg1 = ConvertToString(allTransformList, true);
        msg2 = ConvertToString(domain.GetComponent<InitLines>().lineTransformList, false);
        msg3 = force.GetComponent<DrawForceVector>().ToString();
        messageSet = true;
        submitCanvas.transform.Find("PendingText").gameObject.SetActive(true);
    }

    private string ConvertToString(List<Transform> transformList, bool vertexList)
    {

        StringBuilder sb = new StringBuilder();
        if (vertexList) //aka first string
        {
            int index = 1;
            foreach (Transform transform in transformList)
            {
                if(transform.gameObject.CompareTag("Input") ||
                    transform.gameObject.CompareTag("Output") ||
                    transform.gameObject.CompareTag("Intermediate") ||
                    transform.gameObject.CompareTag("Fixed"))
                {
                    Vector3 translate = transform.localPosition;
                    translate.x += 0.5F;
                    translate.y += 0.5F;
                    translate.z += 0.5F;
                    translate.z = -translate.z;
                    translate.z += 1.0F;
                    sb.Append(index++);
                    string substring = translate.ToString("F4");
                    sb.Append(substring);
                    if (transform.gameObject.CompareTag("Input"))
                        sb.Append("I;");
                    else if (transform.gameObject.CompareTag("Output"))
                        sb.Append("O;");
                    else if (transform.gameObject.CompareTag("Intermediate"))
                        sb.Append("T;");
                    else if (transform.gameObject.CompareTag("Fixed"))
                        sb.Append("F;");
                }                
            }
            return sb.ToString();
        }
        else //second string
        {
            foreach (Transform transform in transformList)
            {
                for (int i = 0; i < allTransformList.Count; i++)
                {
                    if (transform.position == allTransformList[i].position)
                    {

                        sb.Append(i + 1);
                        break;
                    }
                }
                sb.Append(' ');
            }
            return sb.ToString();
        }
    }

    private void SendData(string data)
    {
        print(data);
        theWriter.Write(data);
        theWriter.Write('|');
        submitCanvas.transform.Find("PendingText").gameObject.SetActive(false);
        submitCanvas.transform.Find("SentText").gameObject.SetActive(true);
    }

    private bool setupSocket()
    {
        try
        {
            mySocket = new TcpClient(Host, Port);
            theStream = mySocket.GetStream();
            theWriter = new StreamWriter(theStream);
            theReader = new StreamReader(theStream);
            theReader.BaseStream.ReadTimeout = 5000;
            socketReady = true;
            theWriter.AutoFlush = true;
            Debug.Log("socket is sent");
            return true;
        }
        catch (Exception e)
        {
            Debug.Log("Socket error: " + e);
            errorMsg = "Looks like we weren't able to connect to Matlab. Please double check that the Matlab Server is running.";
            return false;
        }
    }

    private bool checkConditions()
    {
        if(allTransformList.Count < 3 || inputCount <= 0 || outputCount <= 0)
        {
            errorMsg = "You do not have enough points! Make sure you have at least one input, one output, and one intermediate point.";
            return false;
        }
        
        if(domain.GetComponent<InitLines>().lineTransformList.Count < 3)
        {
            errorMsg = "There doesn't seem to be enough connections. Make sure there are at least two connection before submitting.";
            return false;
        }
        if (domain.GetComponent<InitLines>().forceLineTransformList.Count !=  inputCount * 2)
        {
            errorMsg = "Make sure there is one force vector for every input node.";
            return false;
        }
        return true;
    }

    public void addToList(GameObject obj)
    {
        allTransformList.Add(obj.transform);
        if(obj.CompareTag("Input"))
        {
            inputCount += 1;
        }
        else if (obj.CompareTag("Output"))
        {
            outputCount += 1;
        }
    }

    public void removeFromList(GameObject obj)
    {
        allTransformList.Remove(obj.transform);
        if (obj.CompareTag("Input"))
        {
            inputCount -= 1;
        }
        else if (obj.CompareTag("Output"))
        {
            outputCount -= 1;
        }
    }

    private void plotDeformedCoords(string str)
    {
        string[] strArr = str.Split('|');
        string[] coords = strArr[0].Split(';');
        for(int i = 0; i < coords.Length-1; i++)
        {
            Vector3 newCoords = getLocalCoords(coords[i]);
            GameObject newObj = GameObject.Instantiate(allTransformList[i].gameObject, domain.transform);
            newObj.transform.localScale = allTransformList[i].localScale;
            Color color = ((Renderer)newObj.GetComponent<Renderer>()).material.color;
            color.a = 1F;
            ((Renderer)newObj.GetComponent<Renderer>()).material.color = color;
            deformedTransformList.Add(newObj.transform);
            newObj.transform.localPosition = newCoords;
        }
        string[] indices = strArr[1].Split(' ');
        //Modify lineTransformList
        for (int i = 0; i < indices.Length - 1; i++)
        {
            if (string.Compare(indices[i], "") != 0)
            {
                domain.GetComponent<InitLines>().deformedLineTransformList.Insert(i,deformedTransformList[int.Parse(indices[i])-1]);
                domain.GetComponent<InitLines>().deformedLine.points3.Add(deformedTransformList[int.Parse(indices[i]) - 1].position);
            }
            
        }

        domain.GetComponent<InitLines>().deformedLine.SetColor(Color.red);
    }

    private Vector3 getLocalCoords(string coordString)
    {
        Vector3 retVec = new Vector3();
        int start = coordString.IndexOf('(');
        int end = coordString.IndexOf(')');
        string[] coords = (coordString.Substring(start + 1, end - start - 1)).Split(',');
        
        retVec.x = float.Parse(coords[0]) - 0.5F;
        retVec.y = float.Parse(coords[1]) - 0.5F;
        retVec.z = float.Parse(coords[2]);
        retVec.z -= 1.0F;
        retVec.z = -retVec.z;
        retVec.z -= 0.5F;
        return retVec;

    }
}
