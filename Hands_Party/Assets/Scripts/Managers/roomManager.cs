using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;
using UnityEngine.Events;

public class roomManager : MonoBehaviourPunCallbacks
{
  public enum RoomProperties
  { 
    AllPlayerReady,
    AllGamesFinish,
    CurrentGame,
    CurrentGameIndex,
    Time,
  }
  #region Room variables
  public bool allPlayersReady;
  public bool allGamesFinish;
  bool timerRunning;
  public int time;
  public GameLoader.Games currentGame = GameLoader.Games.lobby;
  public int currentGameIndex;
  #endregion
  bool syncing;

  Hashtable roomProps = new Hashtable();

  public List<GameLoader.Games> gameList;

  public UnityEvent OnEndMiniGames;

  private void Awake()
  {
    allPlayersReady = false;
    allGamesFinish = false;
    currentGame = GameLoader.Games.lobby;
    currentGameIndex = -1;
    time = 0;

    roomProps[RoomProperties.AllPlayerReady.ToString()] = allPlayersReady;
    roomProps[RoomProperties.AllGamesFinish.ToString()] = allGamesFinish;
    roomProps[RoomProperties.CurrentGame.ToString()] = currentGame;
    //roomProps[RoomProperties.CurrentGameIndex.ToString()] = currentGameIndex;
    roomProps[RoomProperties.Time.ToString()] = time;
  }

  // Start is called before the first frame update
  void Start()
  {
    
  }

  // Update is called once per frame
  void Update()
  {
    if (PhotonNetwork.LocalPlayer.IsMasterClient == false)
    {
      return;
    }

    if (currentGame == GameLoader.Games.lobby && allPlayersReady == true)
    {
      if (currentGameIndex < gameList.Count)
      {
        currentGameIndex++;
        currentGame = gameList[currentGameIndex];
        roomProps[RoomProperties.CurrentGame.ToString()] = currentGame;
        StartGameCountDown();
      }
    }

    if (currentGame == GameLoader.Games.lobby && currentGameIndex >= gameList.Count)
    {
      allGamesFinish = true;
      roomProps[RoomProperties.AllGamesFinish.ToString()] = allGamesFinish;
    }


    if (currentGame != GameLoader.Games.lobby)
    {

      if (timerRunning == false)
      {
        StartCoroutine(timer());
        timerRunning = true;
      }

      roomProps[RoomProperties.Time.ToString()] = time;

      if (time <= 0)
      {
        currentGame = GameLoader.Games.lobby;
        roomProps[RoomProperties.CurrentGame.ToString()] = currentGame;

        if (OnEndMiniGames != null)
        {
          OnEndMiniGames.Invoke();
        }
      }
    }

    if (syncing == false)
    {
      StartCoroutine(syncRoomProps());
      syncing = true;
    }
  }

  void StartGameCountDown()
  {
    switch (currentGame)
    {
      case GameLoader.Games.burger:
        time = 300;
        break;
      case GameLoader.Games.dart:
        break;
      case GameLoader.Games.gestures:
        time = 180;
        break;
    }
  }

  IEnumerator timer()
  {
    yield return new WaitForSecondsRealtime(1);
    time--;
    timerRunning = false;
  }

  IEnumerator syncRoomProps()
  {
    yield return new WaitForSecondsRealtime(1);
    PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);
    syncing = false;
  }

  bool checkAllPlayerReady()
  {
    foreach (Player player in PhotonNetwork.PlayerList)
    {
      if ((bool)player.CustomProperties[localProperties.PropertiesName.ready.ToString()] == false)
      {
        return false;
      }
    }
    return true;
  }

  public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
  {
    //roomProperties.AllPlayerReady = checkAllPlayerReady();
    allPlayersReady = checkAllPlayerReady();
    roomProps[RoomProperties.AllPlayerReady.ToString()] = checkAllPlayerReady();
    PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);
    Debug.Log(checkAllPlayerReady());
  }

  void RandomGameList()
  {
    while (gameList.Count < (Enum.GetNames(typeof(GameLoader.Games)).Length - 1))
    {
      int randomNum = Random.Range(0, Enum.GetNames(typeof(GameLoader.Games)).Length);
      if ((GameLoader.Games)randomNum != GameLoader.Games.lobby && gameList.Contains((GameLoader.Games)randomNum) == false)
      {
        gameList.Add((GameLoader.Games)randomNum);
      }
    }
  }

  public override void OnCreatedRoom()
  {
    if (PhotonNetwork.LocalPlayer.IsMasterClient == false)
    {
      return;
    }
    RandomGameList();
  }

  public override void OnJoinedRoom()
  {
    if (PhotonNetwork.LocalPlayer.IsMasterClient == true)
    {
      PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);
    }
  }

}
