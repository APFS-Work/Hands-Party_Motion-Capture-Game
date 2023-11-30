using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UDPClientPhone : MonoBehaviour
{
    #region UDP Client
    IPEndPoint iPEndPoint = null;
    UdpClient udpClient = null;
    bool connected;
    int port;
    byte[] sendByte;
    Thread receiveThread = null;
    byte[] receiveByte;
    string receiveData = "";

    List<string> iP = new List<string>();
    #endregion

    #region UIs
    public TMP_InputField IPInput;
    public TMP_InputField PortInput;
    public TMP_InputField FPSInput;
    public TMP_InputField QualityInput;

    public TMP_Text Connection_State;
    public TMP_Text Connection_Log;
    public TMP_Text Image_State;
    public TMP_Text Image_Log;

    public TMP_Dropdown CameraChoose;
    public TMP_Text CameraChoose_Label;

    public TMP_Text Log;

    public RawImage background;
    #endregion

    #region Camera
    bool camAvailable;
    WebCamTexture camTexture;
    Texture2D Tex;
    int FPS;
    int quality;
    void camSetUp()
    {
        background.gameObject.SetActive(false);
        if (camTexture != null)
        {
            camTexture.Stop();
            camTexture = null;
        }

        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            camAvailable = false;
            Image_Log.text = "Camera: No available";
            return;
        }

        List<string> option = new List<string>();
        for (int x = 0; x < devices.Length; x++)
        {
            if (!devices[x].isFrontFacing)
            {
                option.Add("Back Camera");
            }
            else
            {
                option.Add("Front Camera");
            }
        }
        CameraChoose.AddOptions(option);

        for (int x = 0; x < devices.Length; x++)
        {
            if (CameraChoose_Label.text == "Front Camera" && devices[x].isFrontFacing)
            {
                camTexture = new WebCamTexture(devices[x].name, Screen.width, Screen.height);
                Image_Log.text = "Camera: Front Camera";
                
            }
            else if (CameraChoose_Label.text == "Back Camera" && !devices[x].isFrontFacing)
            {
                camTexture = new WebCamTexture(devices[x].name, Screen.width, Screen.height);
                Image_Log.text = "Camera: Back Camera";
            }
        }

        if (camTexture == null)
        {
            Image_Log.text = "Unable to initialize camera";
            return;
        }
        else
        {
            camTexture.requestedFPS = FPS;
            camTexture.requestedHeight = 1;
            camTexture.requestedWidth = 1;
        }

        camTexture.Play();
        background.gameObject.SetActive(true);
        background.texture = camTexture;

        Tex = new Texture2D(camTexture.width, camTexture.height);

        camAvailable = true;

        logger("FPS & Quality modify successful");
    }

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        FPS = 30;
        quality = 75;
        connected = false;
        background.gameObject.SetActive(false);
        camSetUp();
    }

    void Update()
    {
        if (receiveData != "")
        {
            logger("Receive: " + receiveData);
            action(receiveData);
            receiveData = "";
        }
        SendTexture();
    }

    #region receive
    void StartReceiveData()
    {
        if (udpClient == null || iPEndPoint == null) return;

        if (receiveThread != null)
        {
            receiveThread.Abort();
        }

        receiveThread = new Thread(ReceiveData);
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    void ReceiveData()
    {
        while (true)
        {
            if (udpClient == null || iPEndPoint == null) return;
            receiveByte = udpClient.Receive(ref iPEndPoint);

            receiveData = System.Text.Encoding.UTF8.GetString(receiveByte);
            Debug.Log("Receive: " + receiveData);
            
        }
    }

    private void OnDisable()
    {
        if (receiveThread != null)
        { 
            receiveThread.Start();
            receiveThread.Abort();
        }
    }
    private void OnApplicationQuit()
    {
        receiveThread.Abort();
    }
    #endregion

    #region send
    void SendData(byte[] Data)
    {
        udpClient.Send(Data, Data.Length, iPEndPoint);
    }

    void SendTexture()
    {
        if (connected == false)
        {
            return;
        }

        if (camAvailable == false)
        {
            //SendStringData("No Camera Available");
            return;
        }

        if (camTexture.didUpdateThisFrame == false)
        {
            return;
        }

        Tex.SetPixels32(camTexture.GetPixels32());
        Tex.Apply();
        SendData(Tex.EncodeToJPG(quality));

        //SendStringData("Sent " + Tex.EncodeToJPG(quality).Length + "byte data");
    }

    void SendStringData(string tempData)
    {
        sendByte = System.Text.Encoding.UTF8.GetBytes(tempData);
        Debug.Log("Send byte length : " + sendByte.Length);
        udpClient.Send(sendByte, sendByte.Length, iPEndPoint);
    }
    #endregion

    void action(string act)
    {
        switch (act)
        {
            case "Connected":
                connected = true;
                Connection_State.text = "State: Connected";
                logger("Connect successful");
                break;
            case "Disconnected":
                connected = false;
                Connection_State.text = "State: Disconnected";
                logger("Disconnected");
                break;
            default:
                Debug.Log(act);
                break;
        }
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

    void ConnectTo(IPAddress ip, int Port)
    {
        iPEndPoint = new IPEndPoint(ip, Port);
        udpClient = new UdpClient();
        SendStringData(GetAddress() + " Connected");
        logger("Try to connect to " + ip.ToString() + ":" + port.ToString());
        Debug.Log("Connect to " + ip.ToString());

        StartReceiveData();
    }

    void logger(string temp)
    {
        Log.text += "\n" + temp;
    }

    public void ChangePortAndIP()
    {
        IPAddress ip;
        if (IPAddress.TryParse(IPInput.text, out ip) == false)
        {
            Connection_Log.text = "Invalid IP";
            return;
        }

        if (Int32.TryParse(PortInput.text, out port) == false)
        {
            Connection_Log.text = "Invalid Port";
            return;
        }

        ConnectTo(ip, port);
    }

    public void ChangeQualityFPS()
    {
        if (Int32.TryParse(FPSInput.text, out FPS) == false)
        {
            FPS = 30;
            Image_Log.text = "Invalid FPS";
            return;
        }
        FPSInput.text = FPS.ToString();

        if (Int32.TryParse(QualityInput.text, out quality) == false)
        {
            quality = 75;
            Image_Log.text = "Invalid quality";
            return;
        }
        if(quality > 100 || quality <= 0)
        {
            quality = 75;
        }
        QualityInput.text = quality.ToString();

        Image_State.text = "Current FPS: " + FPS.ToString() + "\nCurrent Quality: " + quality.ToString();
        logger("Try to change to FPS: " + FPS.ToString() + " Quality: " + quality.ToString());

        camSetUp();
        if (camAvailable == false)
        {
            logger("Camera is not available");
            if (connected)
            { 
                SendStringData("No Camera Available");
            }
        }
    }
}
