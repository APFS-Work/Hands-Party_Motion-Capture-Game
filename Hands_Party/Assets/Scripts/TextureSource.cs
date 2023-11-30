using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mediapipe.Unity;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class TextureSource : ImageSource
{
  [Tooltip("For the default resolution, the one whose width is closest to this value will be chosen")]
  [SerializeField] private int _preferableDefaultWidth = 1280;

  private const string _TAG = nameof(WebCamSource);

  [SerializeField] private ResolutionStruct[] _defaultAvailableResolutions;

  private static readonly object _PermissionLock = new object();
  private static bool _IsPermitted = false;

  public override SourceType type => SourceType.Camera;

  private Texture2D _camTexture;
  private Texture2D camTexture
  {
    get => _camTexture;
    set
    {
      _camTexture = value;
    }
  }

  public override int textureWidth => !isPrepared ? 0 : camTexture.width;
  public override int textureHeight => !isPrepared ? 0 : camTexture.height;

  //public override bool isVerticallyFlipped => isPrepared && webCamTexture.videoVerticallyMirrored;
  //public override bool isFrontFacing => isPrepared && (webCamDevice is WebCamDevice valueOfWebCamDevice) && valueOfWebCamDevice.isFrontFacing;

  //public override RotationAngle rotation => !isPrepared ? RotationAngle.Rotation0 : (RotationAngle)webCamTexture.videoRotationAngle;

  //private WebCamDevice? _webCamDevice;
  /*
  private WebCamDevice? webCamDevice
  {
    get => _webCamDevice;
    set
    {
      if (_webCamDevice is WebCamDevice valueOfWebCamDevice)
      {
        if (value is WebCamDevice valueOfValue && valueOfValue.name == valueOfWebCamDevice.name)
        {
          // not changed
          return;
        }
      }
      else if (value == null)
      {
        // not changed
        return;
      }
      _webCamDevice = value;
      resolution = GetDefaultResolution();
    }
  }
  */
  public override string sourceName => (camTexture == null) ? "Cam Texture Null?" : null;

  private WebCamDevice[] _availableSources;
  private WebCamDevice[] availableSources
  {
    get
    {
      if (_availableSources == null)
      {
        _availableSources = WebCamTexture.devices;
      }

      return _availableSources;
    }
    set => _availableSources = value;
  }

  public override string[] sourceCandidateNames => availableSources?.Select(device => device.name).ToArray();

#pragma warning disable IDE0025
  
  public override ResolutionStruct[] availableResolutions
  {
    get
    {
      return camTexture == null ? null : _defaultAvailableResolutions;
    }
  }
  
#pragma warning restore IDE0025

  public override bool isPrepared => camTexture != null;
  public override bool isPlaying => camTexture != null && _isPlaying;
  private bool _isPlaying;
  private bool _isInitialized;

  private IEnumerator Start()
  {
    SetUP();

    yield return new WaitUntil(() => connected);

    _isInitialized = true;

    //Play();
  }

  public override void SelectSource(int sourceId)
  {
    if (sourceId < 0 || sourceId >= availableSources.Length)
    {
      throw new ArgumentException($"Invalid source ID: {sourceId}");
    }

  }

  public override IEnumerator Play()
  {
    yield return new WaitUntil(() => _isInitialized);

    
    _isPlaying = true;

    yield return WaitForWebCamTexture();

  }

  public override IEnumerator Resume()
  {
    if (!isPrepared)
    {
      Debug.LogWarning("Texture is not prepared yet");
      throw new InvalidOperationException("WebCamTexture is not prepared yet");
    }
    if (!_isPlaying)
    {
      _isPlaying = true;
    }
    yield return WaitForWebCamTexture();
  }

  public override void Pause()
  {
    if (isPlaying)
    {
      //webCamTexture.Pause();
      _isPlaying = false;
    }
  }

  public override void Stop()
  {
    Pause();
    /*
    if (webCamTexture != null)
    {
      webCamTexture.Stop();
    }
    webCamTexture = null;
    */
  }

  public override Texture GetCurrentTexture()
  {
    return camTexture;
  }

  private ResolutionStruct GetDefaultResolution()
  {
    var resolutions = availableResolutions;
    return resolutions == null || resolutions.Length == 0 ? new ResolutionStruct() : resolutions.OrderBy(resolution => resolution, new ResolutionStructComparer(_preferableDefaultWidth)).First();
  }

  private void InitializeWebCamTexture()
  {
    /*
    Stop();
    if (webCamDevice is WebCamDevice valueOfWebCamDevice)
    {
      webCamTexture = new WebCamTexture(valueOfWebCamDevice.name, resolution.width, resolution.height, (int)resolution.frameRate);
      return;
    }
    throw new InvalidOperationException("Cannot initialize WebCamTexture because WebCamDevice is not selected");
    */
  }

  private IEnumerator WaitForWebCamTexture()
  {
    //const int timeoutFrame = 500;
    //var count = 0;
    //Mediapipe.Unity.Logger.LogVerbose("Waiting for WebCamTexture to start");
    yield return new WaitForSeconds(1);
    //ApplyTexture();
    //yield return new WaitUntil(() => count++ > timeoutFrame || webCamTexture.width > 16);

    /*
    if (webCamTexture.width <= 16)
    {
      throw new TimeoutException("Failed to start WebCam");
    }
    */
    //throw new TimeoutException("Failed to getTexture");
  }

  private class ResolutionStructComparer : IComparer<ResolutionStruct>
  {
    private readonly int _preferableDefaultWidth;

    public ResolutionStructComparer(int preferableDefaultWidth)
    {
      _preferableDefaultWidth = preferableDefaultWidth;
    }

    public int Compare(ResolutionStruct a, ResolutionStruct b)
    {
      var aDiff = Mathf.Abs(a.width - _preferableDefaultWidth);
      var bDiff = Mathf.Abs(b.width - _preferableDefaultWidth);
      if (aDiff != bDiff)
      {
        return aDiff - bDiff;
      }
      if (a.height != b.height)
      {
        // prefer smaller height
        return a.height - b.height;
      }
      // prefer smaller frame rate
      return (int)(a.frameRate - b.frameRate);
    }
  }









  #region TextureServer


  IPEndPoint iPEndPoint;
  UdpClient udpClient;
  Thread receiveThread;
  byte[] receiveByte;


  byte[] receiveTextureByte;
  Texture2D receiveTexture;

  public bool testMode;

  public static bool connected = false;

  byte[] sendByte;
  string receiveData = "";

  public static int Image_port;
  void ReceiveData()
  {
    while (true)
    {
      receiveByte = udpClient.Receive(ref iPEndPoint);

      if (receiveByte.Length > 0)
      {
        connected = true;
        SendStringData("Connected");
      }
      else
      {
        Debug.LogWarning("No data received or not connected");
        return;
      }

      receiveTextureByte = receiveByte;

    }
  }

  void SendStringData(string tempData)
  {
    sendByte = System.Text.Encoding.UTF8.GetBytes(tempData);
    udpClient.Send(sendByte, sendByte.Length, iPEndPoint);
  }

  void ApplyTexture()
  {
    if (receiveTextureByte == null || _isPlaying == false)
    {
      //_isPlaying = false;
      return;
    }

    if (receiveTexture.LoadImage(receiveTextureByte))
    {
      //_isPlaying = true;
      //Debug.Log("Received a Texture   " + receiveTextureByte.Length);
      receiveTextureByte = null;
      camTexture = receiveTexture;
    }
    else
    {
      //Debug.Log("It is not a Texture");
      receiveData = System.Text.Encoding.UTF8.GetString(receiveTextureByte);
    }

  }

  private void SetUP()
  {
    receiveTexture = new Texture2D(800, 480);

    if (testMode == true)
    {
      Image_port = 25659;
    }
    else
    {
      Image_port = UnityEngine.Random.Range(25000, 26000);
    }
    iPEndPoint = new IPEndPoint(IPAddress.Any, Image_port);
    udpClient = new UdpClient(iPEndPoint.Port);
    Debug.Log(iPEndPoint.Port);

    receiveThread = new Thread(ReceiveData);
    receiveThread.IsBackground = true;
    receiveThread.Start();

    camTexture = receiveTexture;

    Debug.Log("Your IP : " + GetAddress());
  }

  // Update is called once per frame
  void Update()
  {
    ApplyTexture();
    if (receiveData != "")
    {
      Debug.Log("Received : " + receiveData);
      receiveData = "";
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

  public static string GetAddress()
  {
    var host = Dns.GetHostEntry(Dns.GetHostName());
    foreach (var ip in host.AddressList)
    {
      if (ip.AddressFamily == AddressFamily.InterNetwork)
      {
        return ip.ToString();
      }
    }
    throw new Exception("No network adapters with an IPv4 address in the system!");
  }



  #endregion
}
