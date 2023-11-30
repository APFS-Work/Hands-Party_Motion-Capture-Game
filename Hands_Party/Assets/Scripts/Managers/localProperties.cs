using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class localProperties : MonoBehaviourPunCallbacks
{
  public PhyButton readyBtn;

  public TMP_InputField nickNameInput;
  public TMP_Text burgerScore;

  public enum PropertiesName
  {
    mark,
    ready,
  }

  Hashtable playerProperties = new Hashtable();

  public int mark = 0;
  public string playerNickName = "";
  bool ready = false;

  private void Awake()
  {
    readyBtn.OnReleased.AddListener(GetisReady);
  }


  // Start is called before the first frame update
  void Start()
  {
    playerNickName = "Player";
    nickNameInput.text = playerNickName;
    resetPlayerProps();
    burgerScore.text = "Score: " + mark;
  }

  // Update is called once per frame
  void Update()
  {
  
  }

  void resetPlayerProps()
  {
    mark = 0;
    ready = false;    

    playerProperties[PropertiesName.mark.ToString()] = mark;
    playerProperties[PropertiesName.ready.ToString()] = ready;
  }

  void updatePlayerNickName()
  {
    PhotonNetwork.LocalPlayer.NickName = playerNickName;
  }

  void GetisReady()
  {
    ready = readyBtn.isReady;
    playerProperties[PropertiesName.ready.ToString()] = ready;
    PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
    //Debug.Log("ready " + ready);
  }

  public void AddScore(int add)
  {
    mark += add;
    playerProperties[PropertiesName.mark.ToString()] = mark;
    burgerScore.text = "Score: " + mark;
    PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
  }

  public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
  {
    if ((GameLoader.Games)propertiesThatChanged[roomManager.RoomProperties.CurrentGame.ToString()] != GameLoader.Games.lobby)
    {
      if (ready == true)
      {
        GetisReady();
        readyBtn.isReady = false;
      }
    }
  }

  public override void OnJoinedRoom()
  {
    resetPlayerProps();
    PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
    updatePlayerNickName();
  }

  public void changeNickName(string str)
  {
    playerNickName = nickNameInput.text;
    //Debug.Log(playerNickName);
  }

}
