using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameLoader : MonoBehaviourPunCallbacks
{
  public CameraControl camControl;

  public enum Games
  { 
    lobby,
    gestures,
    burger,
    dart
  }

  public TMP_Text gameLog;

  public GameObject lobby;
  public Dictionary<Player, int> ranking;
  public List<TMP_Text> PlayerNameList;
  public List<TMP_Text> PlayerScoreList;
  public List<TMP_Text> PlayerReadyList;

  public GameObject burgerPageUI;
  public GameObject burger;
  public GameObject burgerTempObj;

  public GameObject gesturePageUI;
  public GameObject gestures;
  public MonsterCreator monCreate;
  public GameObject gestureTempObj;

  // Start is called before the first frame update
  void Start()
  {
    ChangeGame(Games.lobby);
  }

  // Update is called once per frame
  void Update()
  {

  }

  void startLobby()
  {
    lobby.SetActive(true);
    camControl.locked = false;
    updateRanking();

    burgerPageUI.SetActive(false);
    burger.SetActive(false);
    deleteTempObj(burgerTempObj);

    gesturePageUI.SetActive(false);
    monCreate.gameRunning = false;
    monCreate.SendScore();
    monCreate.score = 0;
    gestures.SetActive(false);
    deleteTempObj(gestureTempObj);

    StartCoroutine(updateAfter(1));
  }

  void startBurger()
  {
    burger.SetActive(true);
    burgerPageUI.SetActive(true);

    lobby.SetActive(false);
    gestures.SetActive(false);
  }

  void startGesture()
  {
    camControl.LockCamera(Vector3.zero);
    camControl.locked = true;
    gestures.SetActive(true);
    gesturePageUI.SetActive(true);
    monCreate.gameRunning = true;

    lobby.SetActive(false);
  }

  void deleteTempObj(GameObject tempObj)
  {
    foreach (Transform child in tempObj.transform)
    {
      Destroy(child.gameObject);
    }
  }

  void updateRanking()
  {
    ranking = new Dictionary<Player, int>();
    Dictionary<Player, int> playerList = new Dictionary<Player, int>();
    foreach (Player player in PhotonNetwork.PlayerList)
    {
      playerList.Add(player, (int)player.CustomProperties[localProperties.PropertiesName.mark.ToString()]);
    }
    var result = playerList.OrderByDescending(i => i.Value);

    foreach (KeyValuePair<Player, int> kvp in result)
    {
      ranking.Add(kvp.Key, kvp.Value);
    }

    int count = Mathf.Clamp(ranking.Count, 0, 10);
    for (int x = 0; x < count; x++)
    {
      PlayerNameList[x].text = ranking.ElementAt(x).Key.NickName;
      PlayerScoreList[x].text = ranking.ElementAt(x).Value.ToString();
    }
    for (int x = count + 1; x < PlayerReadyList.Count; x++)
    {
      PlayerReadyList[x].text = "";
    }
  }

  IEnumerator updateAfter(int seconds)
  {
    yield return new WaitForSecondsRealtime(seconds);
    updateRanking();
  }


  public void ChangeGame(Games game)
  {
    switch (game)
    {
      case Games.lobby:
        startLobby();
        break;
      case Games.burger:
        startBurger();
        break;
      case Games.gestures:
        startGesture();
        break;
    }
  }


  public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
  {
    if (ranking.ContainsKey(targetPlayer))
    {
      KeyValuePair<Player, int> kvp = new KeyValuePair<Player, int>(targetPlayer, ranking[targetPlayer]);
      int index = ranking.ToList().IndexOf(kvp);
      if ((bool)changedProps[localProperties.PropertiesName.ready.ToString()])
      {
        PlayerReadyList[index].text = "Ready";
        PlayerReadyList[index].color = Color.green;
      }
      else
      {
        PlayerReadyList[index].text = "Not Ready";
        PlayerReadyList[index].color = Color.red;
      }
    }
  }

  public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
  {
    if ((bool)propertiesThatChanged[roomManager.RoomProperties.AllGamesFinish.ToString()] == true)
    {
      gameLog.text = "Game Over" + "\nWinner is " + ranking.ElementAt(0).Key.NickName;
    }
  }

  public override void OnJoinedRoom()
  {
    StartCoroutine(updateAfter(1));
  }

  public override void OnPlayerEnteredRoom(Player newPlayer)
  {
    updateRanking();
  }

  public override void OnPlayerLeftRoom(Player otherPlayer)
  {
    updateRanking();
  }
}
