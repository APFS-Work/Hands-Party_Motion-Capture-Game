using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class roomProperties : MonoBehaviourPunCallbacks
{
  GameLoader gameLoader;

  public bool AllPlayerReady = false;

  public TMP_Text burgerTimer;
  public TMP_Text gestureTimer;
  public int time;


  public GameLoader.Games game = GameLoader.Games.lobby;

  // Start is called before the first frame update
  void Start()
  {
    gameLoader = gameObject.GetComponent<GameLoader>();
  }

  // Update is called once per frame
  void Update()
  {

  }

  void changeTime()
  {
    TimeSpan t = TimeSpan.FromSeconds(time);

    string TimerText = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);

    if (game == GameLoader.Games.burger)
    {
      burgerTimer.text = TimerText;
    }

    if (game == GameLoader.Games.gestures)
    {
      gestureTimer.text = TimerText;
    }
  }


  public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
  {
    time = (int)propertiesThatChanged[roomManager.RoomProperties.Time.ToString()];
    changeTime();

    if (game != (GameLoader.Games)propertiesThatChanged[roomManager.RoomProperties.CurrentGame.ToString()])
    { 
      game = (GameLoader.Games)propertiesThatChanged[roomManager.RoomProperties.CurrentGame.ToString()];
      gameLoader.ChangeGame(game);
    }

  }

}
