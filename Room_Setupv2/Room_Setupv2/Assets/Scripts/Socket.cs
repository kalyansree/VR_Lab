using System.Collections;
using System.Net;
using System.Net.Sockets;
using System;
using System.IO;
using UnityEngine;

public class Socket : MonoBehaviour
{
    // Use this for initialization
    internal Boolean socketReady = false;
    TcpClient mySocket;
    NetworkStream theStream;
    StreamWriter theWriter;
    StreamReader theReader;
    String Host = "localhost";
    Int32 Port = 55000;
    void Start()
    {
        setupSocket();
        Debug.Log("socket is set up");
    }


    public void setupSocket()
    {
        try
        {
            mySocket = new TcpClient(Host, Port);
            theStream = mySocket.GetStream();
            theWriter = new StreamWriter(theStream);
            theWriter.AutoFlush = true;
            socketReady = true;
            theWriter.WriteLine("yah!! it works");
            Debug.Log("socket is sent");
        }
        catch (Exception e)
        {
            Debug.Log("Socket error: " + e);
        }
    }
}