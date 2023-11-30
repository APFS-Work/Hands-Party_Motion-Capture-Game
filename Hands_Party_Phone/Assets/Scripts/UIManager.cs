using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        SettingsPage.SetActive(false);
        ConnectionPage.SetActive(false);
        ImagePage.SetActive(false);
        LogPage.SetActive(false);
    }

    #region Settings related
    public GameObject SettingIcon;
    public GameObject SettingsPage;
    public GameObject SettingsChoose;

    public GameObject ConnectionPage;

    public GameObject ImagePage;

    public GameObject LogPage;
    #endregion

    public void Open_SettingsPage()
    {
        SettingIcon.SetActive(false);
        SettingsPage.SetActive(true);
    }

    public void Close_SettingPage()
    {
        SettingIcon.SetActive(true);
        SettingsPage.SetActive(false);
    }

    public void Open_ConnectionPage()
    {
        SettingsChoose.SetActive(false);
        ConnectionPage.SetActive(true);
    }

    public void Open_ImagePage()
    {
        SettingsChoose.SetActive(false);
        ImagePage.SetActive(true);
    }

    public void Open_LogPage()
    {
        SettingsChoose.SetActive(false);
        LogPage.SetActive(true);
    }

    public void BackToSettings()
    {
        ConnectionPage.SetActive(false);
        ImagePage.SetActive(false);
        LogPage.SetActive(false);
        SettingsChoose.SetActive(true);
    }
}
