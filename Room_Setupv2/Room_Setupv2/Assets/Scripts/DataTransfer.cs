using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System;
using System.IO;
using System.Text;
public class DataTransfer : MonoBehaviour
{
    // Use this for initialization
    TcpListener listener;
    StreamWriter theWriter;
    String msg;
    void Start()
    {
        listener = new TcpListener(55001);
        listener.Start();
        print("is listening");
    }

    void Update()
    {
        if (testConnection())
        {
            SendData("Hello");
        }
    }
    public bool testConnection()
    {
        if (!listener.Pending())
        {
            return false;
        }
        return true;
    }
    public void SendData(string data)
    {

        print("socket comes");
        TcpClient client = listener.AcceptTcpClient();
        NetworkStream ns = client.GetStream();
        StreamReader reader = new StreamReader(ns);
        theWriter = new StreamWriter(ns);
        theWriter.AutoFlush = true;
        theWriter.WriteLine(data);
        Debug.Log("socket is sent");
    }
}
