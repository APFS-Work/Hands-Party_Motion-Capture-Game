using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon;
using Photon.Pun;

public class PunConnection : MonoBehaviourPunCallbacks
{
  public UIManager uIManager;

  public TMP_Text RoomLog;

  public TMP_InputField CreateRoomInput;
  public TMP_InputField JoinRoomInput;

  // Start is called before the first frame update
  void Start()
  {
    PhotonNetwork.ConnectUsingSettings();
  }

  // Update is called once per frame
  void Update()
  {
      
  }


  public void CreateRoom()
  {
    if (PhotonNetwork.CreateRoom(CreateRoomInput.text))
    {
      RoomLog.text = "Create " + CreateRoomInput.text + "Successful";
    }
    else
    {
      RoomLog.text = "Unable to create " + CreateRoomInput.text;
    }
  }

  public void JoinRoom()
  {
    if (PhotonNetwork.JoinRoom(JoinRoomInput.text))
    {
      RoomLog.text = "Join " + JoinRoomInput.text + "Successful";
    }
    else
    {
      RoomLog.text = "Unable to join " + JoinRoomInput.text;
    }
  }

  public override void OnJoinedRoom()
  {
    uIManager.StartGame();
  }

  public override void OnConnectedToMaster()
  {
    PhotonNetwork.JoinLobby();
  }
}
