using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
  public GameObject MainCan;

  public GameObject Menu;
  public GameObject CreateOrJoinGame;
  public GameObject StartGameText;

  public GameObject SettingPage;
  public GameObject SettingChoice;
  public GameObject ConnectionPage;
  public GameObject OtherPage;

  #region UIs
  public Toggle showCamView;

  public TMP_Text IP_Text;
  public TMP_Text PortText;
  public TMP_Text Connection_State_text;


  #endregion
  bool connected;

  void Start()
  {
    Menu.SetActive(true);
    StartGameText.SetActive(true);
    SettingChoice.SetActive(true);
    CreateOrJoinGame.SetActive(false);
    SettingPage.SetActive(false);
    ConnectionPage.SetActive(false);
    OtherPage.SetActive(false);

    IP_Text.text = "IP: " + TextureSource.GetAddress();
    PortText.text = "Port: " + TextureSource.Image_port;

    if (connected == true)
    {
      Connection_State_text.text = "State: Connected";
    }
    else
    {
      Connection_State_text.text = "State: Disconnected";
    }
  }

  // Update is called once per frame
  void Update()
  {
    if (TextureSource.connected == false) return;

    if (CreateOrJoinGame.activeSelf == false)
    {
      StartGameText.SetActive(false);
      CreateOrJoinGame.SetActive(true);
    }

    if (connected != TextureSource.connected)
    {
      connected = TextureSource.connected;
      if (connected == true)
      {
        Connection_State_text.text = "State: Connected";
      }
      else
      {
        Connection_State_text.text = "State: Disconnected";
      }
    }
  }

  #region Button functions
  public void Open_Setting()
  {
    SettingPage.SetActive(true);
  }
  public void Close_Setting()
  {
    SettingPage.SetActive(false);
  }

  public void Open_Connection()
  {
    ConnectionPage.SetActive(true);
    SettingChoice.SetActive(false);
  }

  public void Open_Other()
  {
    OtherPage.SetActive(true);
    SettingChoice.SetActive(false);
  }

  public void BackToSetChoice()
  {
    SettingChoice.SetActive(true);
    ConnectionPage.SetActive(false);
    OtherPage.SetActive(false);
  }

  public void StartGame()
  {
    Menu.SetActive(false);
  }

  public void CamView(bool boolean)
  {
    MainCan.SetActive(showCamView.isOn);
    //Debug.Log(showCamView.isOn);
  }

  public void QuitGame()
  {
    Application.Quit();
  }
  #endregion


}
