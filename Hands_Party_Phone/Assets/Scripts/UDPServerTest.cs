using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class UDPServerTest : MonoBehaviour
{
    IPEndPoint iPEndPoint;
    UdpClient udpClient;
    Thread receiveThread;
    byte[] receiveByte;
    string receiveData = "";

    float timer;

    byte[] receiveTextureByte;
    Texture2D receiveTexture;

    List<string> iP = new List<string>();
    bool connected;
    void ReceiveData()
    {
        while (true)
        {
            receiveByte = udpClient.Receive(ref iPEndPoint);
            

            receiveData = System.Text.Encoding.UTF8.GetString(receiveByte);
            Debug.LogWarning("Client: " + receiveData);

            if (receiveData != "")
            {
                connected = true;
            }
        }
    }

    void SendStringData(string tempData)
    {
        byte[] sendByte;
        sendByte = System.Text.Encoding.UTF8.GetBytes(tempData);
        udpClient.Send(sendByte, sendByte.Length, iPEndPoint);
    }

    // Start is called before the first frame update
    void Start()
    {
        receiveTexture = new Texture2D(800, 480);

        iPEndPoint = new IPEndPoint(IPAddress.Any, 9999);
        udpClient = new UdpClient(iPEndPoint.Port);
        Debug.LogWarning(iPEndPoint.Port);

        receiveThread = new Thread(ReceiveData);
        receiveThread.IsBackground = true;
        receiveThread.Start();
        GetAddress();
        Debug.LogWarning(iP[0]);
    }


    bool sent;
    // Update is called once per frame
    void Update()
    {
        if (connected == true && sent == false)
        {
            SendStringData("Connected");
            sent = true;
        }
    }

    private void OnDisable()
    {
        udpClient.Close();
        receiveThread.Join();
        receiveThread.Abort();
    }

    private void OnApplicationQuit()
    {
        receiveThread.Abort();
    }

    string GetAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                iP.Add(ip.ToString());
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }
}

